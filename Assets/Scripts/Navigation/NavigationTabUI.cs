using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavigationTabUI : MonoBehaviour, IPointerClickHandler
{
    public Room room;
    public Image image;
    public Color originalColour;
    public Color highlightColour;
    public TextMeshProUGUI tabNameText;

    private void Start()
    {
        GetInfestedRoomName();
    }

    private void OnEnable()
    {
        NavigationManager.OnNavigationTabSelected += ShowRoomInformation;
    }

    private void OnDisable()
    {
        NavigationManager.OnNavigationTabSelected -= ShowRoomInformation;

        if (CameraFollowPlayer.instance.target != GameState.instance.player.transform)
        {
            CameraFollowPlayer.instance.target = GameState.instance.player.transform;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked navigation tab UI");

        NavigationManager.instance.selectedNavigationTab = this;
        NavigationManager.OnNavigationTabSelected?.Invoke(room);
    }

    public void ShowRoomInformation(Room selectedRoom)
    {
        if (selectedRoom != room) { DeselectTab(); return; }

        SelectTab();
        UIManager.instance.navigationRoomInfoText.text = string.Empty;

        bool firstLine = false;

        for (int i = 0; i < room.roomInfoList.Count; i++)
        {
            // only wanting to show infested states for room information
            if (room.roomInfoList[i].showOnState != Room.RoomStates.infested) { continue; }
            if (firstLine == false)
            {
                UIManager.instance.navigationRoomInfoText.text += room.roomInfoList[i].title + "\n" + room.roomInfoList[i].extraText;
                firstLine = true;
                continue;
            }
            else
            {
                UIManager.instance.navigationRoomInfoText.text += "\n" + "\n" + room.roomInfoList[i].title + "\n" + room.roomInfoList[i].extraText;
            }
        }
    }

    public void HideRoomInformation(Room selectedRoom)
    {
        //if (selectedRoom == room) { return; }

        UIManager.instance.navigationRoomInfoText.text = string.Empty;
    }

    public void SelectTab()
    {
        image.color = highlightColour;

        CameraFollowPlayer.instance.target = room.transform;

        UIManager.instance.navigationTravelButton.onClick.RemoveAllListeners();

        UIManager.instance.navigationTravelButton.gameObject.SetActive(true);
        UIManager.instance.navigationTravelButton.onClick.AddListener(TravelToRoom);
    }

    public void DeselectTab()
    {
        image.color = originalColour;
    }

    public void TravelToRoom()
    {
        GameState.instance.player.GetComponent<Transform>().position = room.spawning.checkpoint.respawnPoint.position;
    }

    public void GetInfestedRoomName()
    {
        for (int i = 0; i < room.roomInfoList.Count; i++)
        {
            if (room.roomInfoList[i].isName == true && room.roomInfoList[i].showOnState == Room.RoomStates.infested)
            {
                tabNameText.text = room.roomInfoList[i].title;
                Debug.Log("Got Name");
            }
        }
    }
}
