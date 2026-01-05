using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform playerEntryPoint;
    [SerializeField] private Transform playerExitPoint;

    public void OnEndInteraction()
    {
        Debug.Log("Left door interaction");
    }

    public void OnInteract()
    {
        GameState.instance.ChangeStateToRoomClear();
        GetComponent<Spawning>().enabled = true;
        if (GameState.instance.player != null)
        {
            GameState.instance.player.transform.position = playerEntryPoint.position;
        }
    }
}
