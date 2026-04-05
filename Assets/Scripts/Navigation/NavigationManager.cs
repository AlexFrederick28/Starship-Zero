using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public List<Room> infestedEasyRoomList;
    public List<Room> infestedMediumRoomList;
    public List<Room> infestedHardRoomList;
    public List<Room> questRoomList;

    public GameObject roomNavigationUIPrefab;
    public NavigationTabUI selectedNavigationTab;

    public static Action<Room> OnNavigationTabSelected;

    public static NavigationManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        // active quests appear in navigation quest list
        //QuestManager.OnActivateNewQuest +=

        // complete quests removed from navigation quest list
        //QuestManager.OnQuestCompletion += 
    }

    private void OnDisable()
    {
        if (instance != this)
        {
            // turns off duplicate instances if there are more than one enabled
            gameObject.SetActive(false);
        }

        // active quests appear in navigation quest list
        //QuestManager.OnActivateNewQuest -=

        // complete quests removed from navigation quest list
        //QuestManager.OnQuestCompletion -=
    }

    public void AddInfestedRoomToNavigationList(Room roomToAdd)
    {
        // populate all random infested rooms when the game is made active, however ensure only the rooms that are unlocked (not in a locked area of the map) populate 
        // need an event that plays when a new area is unlocked for new areas with infested rooms to automatically populate 

        // making sure the room is actually infested, and if the area has been unlocked and is not a quest
        if (roomToAdd.currentState != Room.RoomStates.infested) { return; }
        if (roomToAdd.roomAreaLocked == true) { return; }
        if (roomToAdd.isQuestRoom == true) { return; }

        // making sure the room hasnt already been added to a list
        if (infestedEasyRoomList.Any(r => r == roomToAdd)) { return; }
        if (infestedMediumRoomList.Any(r => r == roomToAdd)) { return; }
        if (infestedHardRoomList.Any(r => r == roomToAdd)) { return; }

        // adding the room to a list
        if (roomToAdd.roomDifficulty == Room.RoomDifficulty.easy)
        {
            infestedEasyRoomList.Add(roomToAdd);
            GameObject newNavigationUI = Instantiate(roomNavigationUIPrefab, UIManager.instance.navigationEasyUIParent.transform);
            newNavigationUI.GetComponent<NavigationTabUI>().room = roomToAdd;
        }
        else if (roomToAdd.roomDifficulty == Room.RoomDifficulty.medium)
        {
            infestedMediumRoomList.Add(roomToAdd);
            GameObject newNavigationUI = Instantiate(roomNavigationUIPrefab, UIManager.instance.navigationMediumUIParent.transform);
            newNavigationUI.GetComponent<NavigationTabUI>().room = roomToAdd;
        }
        else if (roomToAdd.roomDifficulty == Room.RoomDifficulty.hard)
        {
            infestedHardRoomList.Add(roomToAdd);
            GameObject newNavigationUI = Instantiate(roomNavigationUIPrefab, UIManager.instance.navigationHardUIParent.transform);
            newNavigationUI.GetComponent<NavigationTabUI>().room = roomToAdd;
        }
    }

    public void RemoveInfestedRoomFromNavigation(Room roomToRemove)
    {

    }

    public void AddQuestRoomToNavigationList()
    {

    }
}
