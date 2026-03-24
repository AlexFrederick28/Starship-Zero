using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{

    public Material spriteUnlitDefault;
    public Material spriteLitDefault;
    public List<RoomData> allRoomData; // rooms will subscribe by themselves

    [Serializable]
    public class RoomData
    {
        public Room roomParent;
        public TilemapRenderer[] tilemapRenderers;
    }

    public static RoomManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnDisable()
    {
        if (instance != this)
        {
            // turns off duplicate instances if there are more than one enabled
            gameObject.SetActive(false);
        }
    }

    public void UnlockRoomViewOnMap(RoomData room)
    {
        if (allRoomData.Any(r => r.roomParent == room.roomParent))
        {
            for (int i = 0; i < room.tilemapRenderers.Length; i++)
            {
                room.tilemapRenderers[i].material = spriteUnlitDefault;
            }
        }
    }
}
