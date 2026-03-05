using UnityEngine;

public class PermanentUpgrades : MonoBehaviour, IInteractable
{
    public bool upgradesEnabled = false;

    public void DisableInteractionComponent()
    {
        upgradesEnabled = false;
    }

    public void EnableInteractionComponent()
    {
        upgradesEnabled = true;
    }

    public void OnEndInteraction()
    {
        throw new System.NotImplementedException();
    }

    public void OnInteract()
    {
        if (upgradesEnabled == false) { return; }


    }
}
