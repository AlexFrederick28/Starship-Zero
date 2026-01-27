using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InfestedDoor : Door
{
    public override void OnInteract()
    {
        if (GameState.instance.currentState != GameState.States.RoomClear && GetComponentInParent<Room>().playerInsideRoom == false)
        {
            GameState.instance.ChangeStateToRoomClear();
            GetComponent<Spawning>().enabled = true;
            GetComponent<RespawnCheckpoint>().respawnActive = true;
        }
        else
        {
            GameState.instance.ChangeStateToMain();
            GetComponent<Spawning>().enabled = false;
            GetComponent<RespawnCheckpoint>().respawnActive = false;
        }

        base.OnInteract();
    }
}
