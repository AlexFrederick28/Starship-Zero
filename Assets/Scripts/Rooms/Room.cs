using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomStates { empty, infested, weapons, upgrade }
    [Header("Room States")]
    public RoomStates currentState;
    [Tooltip("What state the room will be in if it was infested and had been cleared by the player")]
    public RoomStates clearedState;
    public RoomStates State()
    {
        if (currentState == RoomStates.infested)
        {
            ChangeToInfestedRoom();
        }
        else if (currentState == RoomStates.empty)
        {
            ChangeToEmptyRoom();
        }
        else if (currentState == RoomStates.weapons)
        {
            ChangeToWeaponsRoom();
        }
        else if (currentState == RoomStates.upgrade)
        {
            ChangeToUpgradeRoom();
        }

        return currentState;
    }

    [SerializeField] private GameObject emptyRoom;
    [SerializeField] private GameObject infestedRoom;
    [SerializeField] private GameObject weaponsRoom;
    [SerializeField] private GameObject upgradeRoom;
    public bool playerInsideRoom = false;

    private void OnEnable()
    {
        if (GameState.instance != null)
        {
            Debug.Log(this.name + "Subscribed");
            GameState.instance.OnPlayerRespawn += PlayerOutsideRoomOnRespawn;
        }
    }

    private void OnDisable()
    {
        GameState.instance.OnPlayerRespawn -= PlayerOutsideRoomOnRespawn;
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
    }

    private void ChangeToEmptyRoom()
    {
        infestedRoom.SetActive(false);
        emptyRoom.SetActive(true);
        weaponsRoom.SetActive(false);
        upgradeRoom.SetActive(false);
    }

    private void ChangeToWeaponsRoom()
    {
        infestedRoom.SetActive(false);
        emptyRoom.SetActive(false);
        weaponsRoom.SetActive(true);
        upgradeRoom.SetActive(false);
    }

    private void ChangeToUpgradeRoom()
    {
        infestedRoom.SetActive(false);
        emptyRoom.SetActive(false);
        weaponsRoom.SetActive(false);
        upgradeRoom.SetActive(true);
    }

    private void PlayerOutsideRoomOnRespawn()
    {
        playerInsideRoom = false;
    }
}
