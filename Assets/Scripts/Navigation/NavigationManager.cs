using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    public List<Room> easyRoomList;
    public List<Room> mediumRoomList;
    public List<Room> hardRoomList;

    public void PopulateNavigationList()
    {
        // populate quest rooms upon quest activation
        // populate all random infested rooms when the game is made active, however ensure only the rooms that are unlocked (not in a locked area of the map) populate 
    }
}
