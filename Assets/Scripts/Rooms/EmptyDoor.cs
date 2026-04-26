using UnityEngine;

public class EmptyDoor : Door
{
    public override void OnInteract()
    {
        if (GameState.instance.currentState == GameState.States.RoomClear) { return; }
        base.OnInteract();
    }
}
