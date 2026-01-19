using UnityEngine;

public interface IInteractable 
{
    /// <summary>
    /// Interaction starts when the player has entered range and pressed the interact key
    /// </summary>
    public void OnInteract();
    /// <summary>
    /// Interaction ends when the player has left the vicinity by default
    /// </summary>
    public void OnEndInteraction();
}
