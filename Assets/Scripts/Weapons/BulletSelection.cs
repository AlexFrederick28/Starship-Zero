using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BulletSelection : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image projectileSelectButtonImage;
    public Image lockedImage;
    public ProjectileScriptableObject projectileType;
    public bool unlocked = false;
    public Image backgroundImage;
    public Color highlightColor;
    public Color originalColor;

    private void OnEnable()
    {
        if (GameState.instance.playerInventory.unlockedProjectileTypes.Any(p => p.projectileName == projectileType.projectileName))
        {
            // if the player has unlocked this projectile type, continue
            unlocked = true;
            lockedImage.enabled = false;
        }
        else
        {
            // otherwise if the player has not unlocked this projectile type, disable it
            unlocked = false;
            lockedImage.enabled = true;
            Debug.Log("Player does not have the projectile unlocked");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (unlocked == false) { return; }
        if (GameState.instance.playerInventory.weaponLoadoutList.Any(w => w.equipID == GameState.instance.playerInventory.selectedSlot.equipID))
        {
            Inventory.EquippedWeapon weapon = GameState.instance.playerInventory.weaponLoadoutList.FirstOrDefault(w => w.equipID == GameState.instance.playerInventory.selectedSlot.equipID);
            GameState.instance.playerInventory.playerWeapons[weapon.equipID].GetComponent<WeaponBase>().ChangeProjectileType(projectileType.projectilePrefabToFire);
            projectileSelectButtonImage.sprite = projectileType.displaySprite;
            UIManager.instance.projectileSelectButton.GetComponentInChildren<TextMeshProUGUI>().text = "Bullet Type: " + projectileType.projectileName;
            UIManager.instance.projectileSelectButtonImage.sprite = projectileType.displaySprite;

            SoundManager.instance.PlayUISound(1.5f);
            UIManager.instance.projectileSelectDropDown.SetActive(false);
        }
        else
        {
            Debug.Log("No weapon with the same equip ID could be found!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        backgroundImage.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        backgroundImage.color = originalColor;
    }
}
