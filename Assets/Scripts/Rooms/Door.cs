using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform playerEntryPoint;
    [SerializeField] private Transform playerExitPoint;
    private bool enteredRoom = false;

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
                GameState.instance.player.transform.position = playerEntryPoint.position;
            }
            else
            {
                GameState.instance.player.transform.position = playerExitPoint.position;
            }
        }
    }
}
