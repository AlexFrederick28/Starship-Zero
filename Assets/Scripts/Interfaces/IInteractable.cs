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

    /// <summary>
    /// Disables the component that inherits from IInteractable
    /// </summary>
    public void DisableInteractionComponent();

    /// <summary>
    /// Enables the component that inherits from IInteractable
    /// </summary>
    public void EnableInteractionComponent();
}
