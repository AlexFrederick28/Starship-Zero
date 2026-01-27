using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform playerEntryPoint;
    [SerializeField] private Transform playerExitPoint;
    public bool enteredRoom = false;

    public virtual void OnEndInteraction()
    {
        // interaction ends when the player has left the distance of the door
    }

    public virtual void OnInteract()
    {
        if (GameState.instance.player != null && GameState.instance.currentState == GameState.States.Main)
        {
            if (enteredRoom == false)
            {
                // enter infested room
                GameState.instance.player.transform.position = playerEntryPoint.position;
                enteredRoom = true;
            }
        }
        else if (GameState.instance.player != null && GameState.instance.currentState == GameState.States.RoomClear)
        {
            // leave cleared infested room
            if (enteredRoom == true && Spawning.instance.timerReachedMaxLength == true)
            {
                GameState.instance.player.transform.position = playerExitPoint.position;
            }
        }
    }
}
