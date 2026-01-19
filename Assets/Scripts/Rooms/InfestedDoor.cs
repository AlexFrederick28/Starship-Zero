using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InfestedDoor : Door
{
    public override void OnInteract()
    {
        base.OnInteract();

        GameState.instance.ChangeStateToRoomClear();
        GetComponent<Spawning>().enabled = true;
    }
}
