using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BulletSelection : MonoBehaviour, IPointerClickHandler
{

    public ProjectileScriptableObject projectileType;

    private void OnEnable()
    {
        if (GameState.instance.playerInventory.unlockedProjectileTypes.Any(p => p.projectileName == projectileType.projectileName))
        {
            // if the player has unlocked this projectile type, continue
        }
        else
        {
            // otherwise if the player has not unlocked this projectile type, disable it
            gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameState.instance.playerInventory.weaponLoadoutList.Any(w => w.equipID == GameState.instance.playerInventory.selectedSlot.equipID))
        {
            // TODO: finish this, still need to set the weapons projectile prefab
        }
        else
        {
            Debug.Log("No weapon with the same equip ID could be found!");
        }

        UIManager.instance.projectileListParent.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
