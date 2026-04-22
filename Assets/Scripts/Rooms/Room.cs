using JetBrains.Annotations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [Serializable]
    public class RoomInformation
    {
        public RoomStates showOnState;
        public bool isName = false;
        public bool isTimer = false;
        public string title;
        public string extraText;
        public GameObject childObject;
    }

    [Tooltip("Whether or not this room is in an accessible area for the player (Allows them to teleport here if so)")]
    public bool roomAreaLocked = false;
    public bool isQuestRoom = false;
    public int questID;
    public List<RoomInformation> roomInfoList;
    public RoomManager.RoomData roomData;
    public Spawning spawning;
    [SerializeField] private GameObject roomInformationPanel;
    private bool withinInteractionRadius = false;

    public enum RoomStates { empty, infested, weapons, upgrade }
    [Header("Room States")]
    public RoomStates currentState;
    [Tooltip("What state the room will be in if it was infested and had been cleared by the player")]
    public RoomStates clearedState;
    public RoomStates State()
    {
        if (currentState == RoomStates.infested)
        {
            ChangeLightColourToInfested();
            ChangeToInfestedRoom();
        }
        else if (currentState == RoomStates.empty)
        {
            ChangeLightColourToCleared();
            ChangeToEmptyRoom();
        }
        else if (currentState == RoomStates.weapons)
        {
            ChangeLightColourToCleared();
            ChangeToWeaponsRoom();
        }
        else if (currentState == RoomStates.upgrade)
        {
            ChangeLightColourToCleared();
            ChangeToUpgradeRoom();
        }

        return currentState;
    }
    public RoomDifficulty roomDifficulty;
    public enum RoomDifficulty { easy, medium, hard }

    [Header("Room Parents")]
    [SerializeField] private GameObject emptyRoom;
    [SerializeField] private GameObject infestedRoom;
    [SerializeField] private GameObject weaponsRoom;
    [SerializeField] private GameObject upgradeRoom;
    public bool playerInsideRoom = false;

    [Header("Door Animation")]
    public Animator animator;

    [Header("Lighting")]
    public Light2D[] lights;
    public Color infestedColour;
    public Color clearedColour;

    public Action OnRoomStateChange;

    private void Start()
    {
        AddRoomToRoomManager();
        SpawnRoomInformationOnStart();
        UpdateRoomInformation();
        HideRoomInformation();

        if (NavigationManager.instance != null)
        {
            NavigationManager.instance.AddInfestedRoomToNavigationList(this);
        }
    }

    private void OnEnable()
    {
        animator.StopPlayback();
        if (GameState.instance != null)
        {
            //Debug.Log(this.name + "Subscribed");
            GameState.instance.OnPlayerRespawn += PlayerOutsideRoomOnRespawn;
        }

        OnRoomStateChange += UpdateRoomInformation;

        // active quests appear in navigation quest list
        QuestManager.OnActivateNewQuest += AddQuestRoomToNavigationMenu;

        // complete quests removed from navigation quest list
        QuestManager.OnQuestCompletion += RemoveQuestRoomFromNavigationList;
    }

    private void OnDisable()
    {
        GameState.instance.OnPlayerRespawn -= PlayerOutsideRoomOnRespawn;

        OnRoomStateChange -= UpdateRoomInformation;

        // active quests appear in navigation quest list
        QuestManager.OnActivateNewQuest -= AddQuestRoomToNavigationMenu;

        // complete quests removed from navigation quest list
        QuestManager.OnQuestCompletion -= RemoveQuestRoomFromNavigationList;
    }

    private void Update()
    {
        if (State() != currentState)
        {
            Debug.Log("Updated room state");
            State();
        }
    }

    private void ChangeToInfestedRoom()
    {
        infestedRoom.SetActive(true);
        emptyRoom.SetActive(false);
        weaponsRoom.SetActive(false);
        upgradeRoom.SetActive(false);
        OnRoomStateChange?.Invoke();
    }

    private void ChangeToEmptyRoom()
    {
        infestedRoom.SetActive(false);
        emptyRoom.SetActive(true);
        weaponsRoom.SetActive(false);
        upgradeRoom.SetActive(false);
        OnRoomStateChange?.Invoke();
    }

    private void ChangeToWeaponsRoom()
    {
        infestedRoom.SetActive(false);
        emptyRoom.SetActive(false);
        weaponsRoom.SetActive(true);
        upgradeRoom.SetActive(false);
        OnRoomStateChange?.Invoke();
    }

    private void ChangeToUpgradeRoom()
    {
        infestedRoom.SetActive(false);
        emptyRoom.SetActive(false);
        weaponsRoom.SetActive(false);
        upgradeRoom.SetActive(true);
        OnRoomStateChange?.Invoke();
    }

    private void PlayerOutsideRoomOnRespawn()
    {
        playerInsideRoom = false;
    }

    // spawn all room information on start
    private void SpawnRoomInformationOnStart()
    {
        for (int i = 0; i < roomInfoList.Count; i++)
        {
            GameObject newInfo = new GameObject();
            newInfo.transform.SetParent(roomInformationPanel.transform);
            roomInfoList[i].childObject = newInfo;
            newInfo.name = roomInfoList[i].title;
            TextMeshProUGUI textComponent = newInfo.AddComponent<TextMeshProUGUI>();
            textComponent.color = Color.black;
            textComponent.enableAutoSizing = true;
            textComponent.fontSizeMin = 0.2f;
            textComponent.fontSizeMax = 0.3f;

            if (roomInfoList[i].showOnState == RoomStates.infested && roomInfoList[i].isTimer == true)
            {
                //float roomDifficulty = (spawning.timerLength / spawning.difficultyMultiplier) / 60;
                textComponent.text = roomInfoList[i].title + " " + spawning.timerLength.ToString(); //+ "\n" + roomInfoList[i].extraText + " " + (int)roomDifficulty;
            }
            else
            {
                textComponent.text = roomInfoList[i].title + " " + roomInfoList[i].extraText;
            }
        }
    }

    // update the room information when the states change
    private void UpdateRoomInformation()
    {
        for (int i = 0; i < roomInfoList.Count; i++)
        {
            if (roomInfoList[i].showOnState != currentState)
            {
                roomInfoList[i].childObject.SetActive(false);
            }
            else
            {
                roomInfoList[i].childObject.SetActive(true);
            }
        }
    }

    // hide the room information when already inside the room
    private void HideRoomInformation()
    {
        roomInformationPanel.SetActive(false);
    }

    private void ShowRoomInformation()
    {
        roomInformationPanel.SetActive(true);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // when inside an infested room, the room info canvas will stay active
        if (collision.gameObject.GetComponent<PlayerBase>() == null || GameState.instance.currentState == GameState.States.RoomClear) { withinInteractionRadius = false; return; }
        else
        {
            withinInteractionRadius = true;
        }

        ShowRoomInformation();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerBase>() == null || withinInteractionRadius == false) { return; }

        HideRoomInformation();
        //Debug.Log("exited");

        withinInteractionRadius = false;
    }

    public virtual void OnDoorAnimationComplete()
    {
        animator.SetBool("isUsed", false);
    }

    public void ChangeLightColourToCleared()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].color = clearedColour;
        }
    }

    public void ChangeLightColourToInfested()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].color = infestedColour;
        }
    }

    public void AddRoomToRoomManager()
    {
        RoomManager.RoomData newRoomData = new RoomManager.RoomData();
        newRoomData.roomParent = this;
        newRoomData.tilemapRenderers = roomData.tilemapRenderers;
        RoomManager.instance.allRoomData.Add(newRoomData);
    }

    public void AddQuestRoomToNavigationMenu(int questID)
    {
        if (questID != this.questID) { return; }

        // making sure the room is actually infested, and if the area has been unlocked and IS a quest
        if (currentState != RoomStates.infested) { return; }
        if (roomAreaLocked == true) { return; }
        if (isQuestRoom == false) { return; }

        // making sure the room hasnt already been added to a list
        if (NavigationManager.instance.questRoomList.Any(r => r == this)) { return; }

        // adding the room to a list
        NavigationManager.instance.questRoomList.Add(this);
        GameObject newNavigationUI = Instantiate(NavigationManager.instance.roomNavigationUIPrefab, UIManager.instance.navigationQuestParent.transform);
        NavigationTabUI newTab = newNavigationUI.GetComponent<NavigationTabUI>();
        newTab.room = this;
        NavigationManager.instance.navigationTabUIList.Add(newTab);

        //Debug.Log("ADDED QUEST ROOM TO NAVIGATION");
    }

    public void RemoveQuestRoomFromNavigationList(int questID)
    {
        if (questID != this.questID) { return; }

        // making sure the room is in a list
        if (!NavigationManager.instance.questRoomList.Any(r => r == this)) { return; }

        // removing the room from the list
        NavigationManager.instance.questRoomList.Remove(this);
        NavigationTabUI tabToRemove = NavigationManager.instance.navigationTabUIList.FirstOrDefault(r => r.room == this);
        NavigationManager.instance.navigationTabUIList.Remove(tabToRemove);
        Destroy(tabToRemove.gameObject);
    }
}
