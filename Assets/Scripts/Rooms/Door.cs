using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform playerEntryPoint;
    [SerializeField] private Transform playerExitPoint;
    [SerializeField] protected Room room;
    private bool discoveredRoomOnEnter = false;
    protected bool nearDoor = false;  

    public void DisableInteractionComponent()
    {
        return;
    }

    public void EnableInteractionComponent()
    {
        return;
    }

    public virtual void OnEndInteraction()
    {
        // interaction ends when the player has left the distance of the door
    }

    public virtual void OnInteract()
    {
        if (nearDoor == false) { return; }
        if (GameState.instance.player != null && room.playerInsideRoom == false)
        {
            // enter room
            room.animator.SetBool("isUsed", true);
            GameState.instance.player.transform.position = playerEntryPoint.position;
            room.playerInsideRoom = true;

            if (discoveredRoomOnEnter == false)
            {
                RoomManager.instance.UnlockRoomViewOnMap(room.roomData);
                discoveredRoomOnEnter = true;
            }
        }
        else if (room.playerInsideRoom == true && GameState.instance.player != null && GameState.instance.currentState != GameState.States.RoomClear)
        {
            // leave room
            room.animator.SetBool("isUsed", true);
            GameState.instance.player.transform.position = playerExitPoint.position;
            room.playerInsideRoom = false;
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        nearDoor = true;
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        nearDoor = false;
    }
}
