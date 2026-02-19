using UnityEngine;

public class UpgradeDoor : Door
{
    public override void OnInteract()
    {
        base.OnInteract();

        // open upgrade UI
        //if (UIManager.instance.weaponCraftingUIParent.activeSelf == true)
        //{
        //    UIManager.instance.weaponCraftingUIParent.SetActive(false);
        //}
        //else
        //{
        //    UIManager.instance.weaponCraftingUIParent.SetActive(true);
        //}
    }
}
