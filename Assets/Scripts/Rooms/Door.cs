using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform playerEntryPoint;
    [SerializeField] private Transform playerExitPoint;

    public virtual void OnEndInteraction()
    {
        // interaction ends when the player has left the distance of the door
    }

    public virtual void OnInteract()
    {
        if (GameState.instance.player != null && GetComponentInParent<Room>().playerInsideRoom == false)
        {
            // enter room
            GameState.instance.player.transform.position = playerEntryPoint.position;
            GetComponentInParent<Room>().playerInsideRoom = true;
        }
        else if (GetComponentInParent<Room>().playerInsideRoom == true && GameState.instance.player != null && GameState.instance.currentState != GameState.States.RoomClear)
        {
            // leave room
            GameState.instance.player.transform.position = playerExitPoint.position;
            GetComponentInParent<Room>().playerInsideRoom = false;
        }
    }
}
