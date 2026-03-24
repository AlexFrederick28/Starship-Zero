using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        public string title;
        public string extraText;
        public GameObject childObject;
    }

    [SerializeField] private List<RoomInformation> roomInfoList;
    public RoomManager.RoomData roomData;
    [SerializeField] private Spawning spawning;
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
    }

    private void OnDisable()
    {
        GameState.instance.OnPlayerRespawn -= PlayerOutsideRoomOnRespawn;

        OnRoomStateChange -= UpdateRoomInformation;
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

            if (roomInfoList[i].showOnState == RoomStates.infested && roomInfoList[i].isName == false)
            {
                float roomDifficulty = (spawning.timerLength / spawning.difficultyMultiplier) / 60;
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
        if (collision.gameObject.GetComponent<PlayerBase>() == null || GameState.instance.currentState == GameState.States.RoomClear) { withinInteractionRadius = false; return; }
        else
        {
            withinInteractionRadius = true;
        }

        ShowRoomInformation();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerBase>() == null || withinInteractionRadius == true) { return; }

        HideRoomInformation();
        Debug.Log("exited");
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
}
