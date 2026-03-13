using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Inventory : MonoBehaviour
{
    [Serializable]
    public class EquippedWeapon
    {
        // when equipping an item we need to track the inventory slot its attached too
        public int equipID; // id that tracks the correct item correlation (equals the loadout slot number)
        public InventorySlot equippedSlotItem; // the slot/item that has been equipped
    }

    public int maxInventorySlots;
    public InventorySlot selectedSlot;
    public List<InventorySlot> inventorySlots;

    private float collectionTimer;
    public float collectionRadius;
    public float collectionInterval;
    public float collectionSpeed;
    public LayerMask layerMask;
    public GameObject inventoryUI;

    public GameObject[] playerWeapons;
    public List<InventorySlot> weaponLoadoutSlotList;
    public List<EquippedWeapon> weaponLoadoutList;
    public List<ProjectileScriptableObject> unlockedProjectileTypes;

    public List<InventorySlot> multiSelectedSlots;
    public Action OnClearingMultiSelectedSlotsFromList;

    private void OnEnable()
    {
        // hiding the image because it is a blank white cube, and nothing is selected so you shouldnt see it anyway
        UIManager.instance.infoInventoryImage.enabled = false;
    }

    private void Start()
    {
        if (UIManager.instance != null)
        {
            SetInventoryOnStart();
            inventoryUI = UIManager.instance.inventoryParent;
            Debug.Log("Added inventory slots");
        }
    }

    private void Update()
    {
        CollectNearbyResource();
    }

    public void SetInventoryOnStart()
    {
        for (int i = 0; i < maxInventorySlots; i++)
        {
            GameObject newSlot = Instantiate(UIManager.instance.slot, UIManager.instance.inventorySlotContentParent.transform);
            newSlot.GetComponent<InventorySlot>().slotPosition = inventorySlots.Count;
            inventorySlots.Add(newSlot.GetComponent<InventorySlot>());
        }
    }

    public void SelectItem(InventorySlot slot)
    {
        if (selectedSlot != null)
        {
            selectedSlot = null;
        }

        selectedSlot = slot;
        selectedSlot.SelectSlot();
    }

    public void SortStacksInInventory()
    {
        for (int desiredPosition = 0; desiredPosition < inventorySlots.Count; desiredPosition++)
        {
            // loop through the max amount of slots, and for each slot, search for a fillable stack to combine
            for (int i = desiredPosition + 1; i < inventorySlots.Count; i++)
            {
                // nested for loop allows us to look ahead of the chosen stack (desiredPosition) rather than whats behind it - also helps avoid merging backwards
                if (inventorySlots[i].inventoryItem != null && inventorySlots[desiredPosition].inventoryItem != null)
                {
                    // if the current inventory slot (i) is not empty, then it is occupied 
                    if (i != desiredPosition)
                    {
                        if (inventorySlots[desiredPosition].inventoryItem.itemName == inventorySlots[i].inventoryItem.itemName)
                        {
                            // if there is a stack that is not full, add to it and remove it from the secondary stack
                            int spaceLeft = inventorySlots[desiredPosition].inventoryItem.maxStackSize - inventorySlots[desiredPosition].currentStackSize;
                            int amountToMove = 0;
                            // move onto the next desired position if the stack is already full
                            if (spaceLeft <= 0) { break; }
                            if (spaceLeft > 0 && inventorySlots[i].currentStackSize > 0)
                            {
                                // if there is enough space left in the desired position, then add to that stack without exceeding the max stack amount
                                amountToMove = Math.Min(inventorySlots[i].currentStackSize, Math.Max(0, inventorySlots[desiredPosition].inventoryItem.maxStackSize - inventorySlots[desiredPosition].currentStackSize));
                                inventorySlots[desiredPosition].currentStackSize += amountToMove;
                                inventorySlots[i].currentStackSize -= amountToMove;

                                inventorySlots[i].RefreshSlot();
                                inventorySlots[desiredPosition].RefreshSlot();
                                Debug.Log("Filled stacks");
                            }
                            if (inventorySlots[i].currentStackSize <= 0)
                            {
                                inventorySlots[i].RenewObject();
                                inventorySlots[i].RefreshSlot();
                            }
                            Debug.Log("Found item with same name in inventory at position: " + i + " Space Left: " + spaceLeft + " Amount to move: " + amountToMove);
                        }
                    }
                }
            }
        }
    }

    public void RemoveGapsFromInventory()
    {
        int desiredPosition = 0;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            // scan through the inventory
            if (inventorySlots[i].inventoryItem != null)
            {
                // if the current inventory slot (i) is not empty, then it is occupied 
                if (i != desiredPosition)
                {
                    // if the occupied position is not equal to the desired position, move slots
                    inventorySlots[desiredPosition].inventoryItem = inventorySlots[i].inventoryItem;
                    inventorySlots[desiredPosition].currentStackSize = inventorySlots[i].currentStackSize;
                    inventorySlots[desiredPosition].weaponEquipped = inventorySlots[i].weaponEquipped;
                    inventorySlots[desiredPosition].equipID = inventorySlots[i].equipID;

                    inventorySlots[i].RenewObject();
                    inventorySlots[desiredPosition].RefreshSlot();
                    inventorySlots[i].RefreshSlot();
                }

                // if the current occupied slot (i) is equal to the desired position keep looking for an empty slot
                desiredPosition++;
            }
        }
    }

    public void AddItemToInventory(InventoryItemPackage type)
    {
        // WEAPONS SHOULD NOT STACK 
        if (type.isWeapon == false)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].inventoryItem != null)
                {
                    if (inventorySlots[i].inventoryItem.itemName == type.itemName && inventorySlots[i].inventoryItem.maxStackSize > 1 && inventorySlots[i].currentStackSize < inventorySlots[i].inventoryItem.maxStackSize)
                    {
                        inventorySlots[i].currentStackSize++;
                        inventorySlots[i].stackNumberText.text = inventorySlots[i].currentStackSize.ToString();
                        Debug.Log("FOUND SAME TYPE AND ADDED TO DESIRED STACK");
                        return;
                    }
                }
            }
        }

        for (int x = 0; x < inventorySlots.Count; x++)
        {
            if (inventorySlots[x].inventoryItem == null)
            {
                inventorySlots[x].inventoryItem = type;
                inventorySlots[x].currentStackSize = 1;
                Debug.Log("Added item to NEW slot");
                return;
            }
        }
    }

    public void RemoveNullItemsFromList()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].inventoryItem == null)
            {
                inventorySlots[i].RenewObject();
                inventorySlots[i].RefreshSlot();
            }
        }
    }

    public void CollectNearbyResource()
    {
        collectionTimer += Time.deltaTime;

        if (collectionTimer > collectionInterval)
        {
            // needs to be placed in update although only every 
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, collectionRadius, transform.forward, layerMask);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.GetComponent<ICollectable>() != null)
                {
                    StartCoroutine(hits[i].transform.GetComponent<ICollectable>().Collect());
                    Debug.Log("Collected " + hits[i].transform.name);
                }
            }

            collectionTimer = 0f;
        }
    }

    public void ShowOrCloseInventory(InputAction.CallbackContext context)
    {
        if (GameState.instance.currentState != GameState.States.RoomClear && context.performed)
        {
            if (inventoryUI.activeSelf == true)
            {
                inventoryUI.SetActive(false);
                UIManager.instance.OnClosedUI?.Invoke();
                UIManager.instance.infoInventoryName.text = string.Empty;
                UIManager.instance.infoInventoryImage.sprite = null;
                UIManager.instance.infoInventoryText.text = string.Empty;
                GameState.instance.ChangeToPreviousState();
            }
            else if (inventoryUI.activeSelf == false && GameState.instance.currentState != GameState.States.OpenUI)
            {
                GameState.instance.ChangeStateToOpenUI();
                UIManager.instance.OnOpenedUI?.Invoke();
                SortStacksInInventory();
                RemoveGapsFromInventory();
                inventoryUI.SetActive(true);
            }

            Debug.Log("Inventory key pressed");
        }
    }

    public void SellSelectedItems()
    {
        int totalEarned = 0;
        if (multiSelectedSlots.Count != 0)
        {
            // sorting the slots to count down rather than up, as to remove any chance of changing slot positions and selling the wrong item
            // using the lambda expression rather than a custom function to set a rule to decide the sorting order (switching positions based on the numbers value)
            multiSelectedSlots.Sort((a, b) => b.slotPosition.CompareTo(a.slotPosition));
            for (int i = 0; i < multiSelectedSlots.Count; i++)
            {
                if (multiSelectedSlots[i] != null)
                {
                    UnequipMultipleWeapons(multiSelectedSlots[i]);
                    totalEarned += multiSelectedSlots[i].inventoryItem.sellAmount * multiSelectedSlots[i].currentStackSize;
                    Debug.Log("Removing item at position: " + multiSelectedSlots[i].slotPosition);
                    multiSelectedSlots[i].SellItem();
                    multiSelectedSlots[i].RefreshSlot();
                }
            }
            RemoveNullItemsFromList();
        }
        else if (selectedSlot.inventoryItem != null && selectedSlot.viewingSlot == true)
        {
            totalEarned += selectedSlot.inventoryItem.sellAmount * selectedSlot.amountFromStackToSell;
            selectedSlot.SellItem();
        }
        else
        {
            return;
        }

        OnClearingMultiSelectedSlotsFromList?.Invoke();

        StartCoroutine(UIManager.instance.NewNotification("Currency + " + totalEarned));
        RemoveGapsFromInventory();
        multiSelectedSlots.Clear();
    }

    /// <summary>
    /// Removes the amount of an item from an inventory. WARNING: you currently need to search if there are enough of the item before using this function
    /// </summary>
    /// <param name="package"></param>
    /// <param name="amount"></param>
    /// <param name="itemName"></param>
    public void RemoveItem(InventoryItemPackage package, int amount, string itemName)
    {
        List<int> slotsToRemove = new List<int>();
        int trackedAmount = amount;
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (trackedAmount == 0) { break; }
            if (inventorySlots[i].inventoryItem == package)
            {
                int currentItemAmount = inventorySlots[i].currentStackSize;
                int amountToTake = (int)MathF.Min(trackedAmount, currentItemAmount);
                trackedAmount -= amountToTake;
                Debug.Log("Removed Amount: " + amountToTake + " From Slot Numb: " + i + " Which had an amount of: " + inventorySlots[i].currentStackSize);
                inventorySlots[i].currentStackSize -= amountToTake;
                RefreshItemAndSlot(i);

                if (inventorySlots[i].currentStackSize <= 0)
                {
                    // adding the item positions to a list to remove the items later
                    slotsToRemove.Add(i);
                }
            }
        }
        Debug.Log("Iems to remove from slots: " + slotsToRemove.ToArray());

        slotsToRemove.Sort();
        slotsToRemove.Reverse(); // highest to lowest
        for (int i = 0; i < slotsToRemove.Count; i++)
        {
            // removing the slots that are empty (0 in a stack)
            Debug.Log("Removed item at position: " + slotsToRemove[i]);
            // reset the slot to be empty
            inventorySlots[slotsToRemove[i]].RenewObject(); // set the object to null essentially (empty slot)

            // refresh the UI to match
            inventorySlots[slotsToRemove[i]].RefreshSlot();
        }

        RemoveNullItemsFromList();
        RemoveGapsFromInventory();
    }

    public void RefreshItemAndSlot(int position)
    {
        inventorySlots[position].RefreshSlot();
    }

    public void ShowSelectedItem(InventorySlot slot)
    {
        // setting item values
        UIManager.instance.infoInventoryName.text = slot.inventoryItem.itemName;
        UIManager.instance.infoInventoryImage.sprite = slot.inventoryItem.sprite;
        UIManager.instance.infoInventoryImage.enabled = true;
        UIManager.instance.infoInventoryText.text = string.Empty;
        UIManager.instance.weaponStatInventoryText.text = string.Empty;

        // setting the description of a weapon
        if (slot.inventoryItem.isWeapon == true)
        {
            UIManager.instance.infoWeaponInventoryText.text = string.Empty;
            UIManager.instance.weaponStatInventoryText.text = string.Empty;

            // shows weapon equip/un-equip button and bullet selection
            // toggling the slider bar off if it is a weapon, as there can only be one in a stack
            UIManager.instance.weaponLoadoutButton.onClick.RemoveAllListeners();
            UIManager.instance.weaponLoadoutButton.onClick.AddListener(EquipAndUnequipWeapon);

            UIManager.instance.infoWeaponInventoryText.gameObject.SetActive(true);
            UIManager.instance.weaponLoadoutButton.gameObject.SetActive(true);

            UIManager.instance.selectedStackAmount.gameObject.SetActive(false);
            UIManager.instance.infoInventoryText.gameObject.SetActive(false);
            UIManager.instance.stackAmountSlider.gameObject.SetActive(false);

            if (selectedSlot.weaponEquipped == true)
            {
                UIManager.instance.bulletSelectParent.gameObject.SetActive(true);
                UIManager.instance.weaponLoadoutButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unequip";
            }
            else
            {
                UIManager.instance.bulletSelectParent.gameObject.SetActive(false);
                UIManager.instance.weaponLoadoutButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
            }

            for (int i = 0; i < slot.inventoryItem.weaponDescription.Length; i++)
            {
                if (i == 0)
                {
                    UIManager.instance.infoWeaponInventoryText.text += slot.inventoryItem.weaponDescription[i].description;
                    UIManager.instance.weaponStatInventoryText.text += slot.inventoryItem.weaponDescription[i].amount;
                }
                else
                {
                    UIManager.instance.infoWeaponInventoryText.text += "\n" + slot.inventoryItem.weaponDescription[i].description;
                    UIManager.instance.weaponStatInventoryText.text += "\n" + slot.inventoryItem.weaponDescription[i].amount;
                }
            }
        }
        else
        {
            // set the description of an item if it is not a weapon
            UIManager.instance.infoInventoryText.text = slot.inventoryItem.description;
            UIManager.instance.weaponLoadoutButton.gameObject.SetActive(false);
            UIManager.instance.bulletSelectParent.gameObject.SetActive(false);
            UIManager.instance.infoWeaponInventoryText.gameObject.SetActive(false);
            UIManager.instance.infoInventoryText.gameObject.SetActive(true);
            UIManager.instance.stackAmountSlider.gameObject.SetActive(true);
            UIManager.instance.selectedStackAmount.gameObject.SetActive(true);
            UIManager.instance.stackAmountSlider.minValue = 0;
            UIManager.instance.stackAmountSlider.maxValue = slot.currentStackSize;
            UIManager.instance.stackAmountSlider.value = slot.currentStackSize;
        }
        
        // displaying and setting the selected slot
        slot.slotImage.color = Color.white;
        selectedSlot = slot;
        slot.viewingSlot = true;
    }

    public void EquipAndUnequipWeapon()
    {
        if (selectedSlot != null && selectedSlot.inventoryItem.isWeapon == true)
        {
            if (selectedSlot.weaponEquipped == true)
            {
                // remove weapon from loadout
                for (int i = 0; i < weaponLoadoutSlotList.Count; i++)
                {
                    if (weaponLoadoutSlotList[i].inventoryItem == null) { continue; }
                    if (selectedSlot.equipID != weaponLoadoutSlotList[i].equipID) { continue; }
                    if (weaponLoadoutSlotList[i] != null) /*&& selectedSlot.isLoadoutSlot == false)*/
                    {
                        // if we have selected a weapon in the inventory rather than a loadout slot 
                        // notify the inventory which weapon has been unequipped
                        SearchInventoryForEquippedWeapon(weaponLoadoutSlotList[i]);
                        UIManager.instance.weaponLoadoutButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
                        Debug.Log("Removed weapon");

                        // turn off weapon
                        TurnOffWeapon(playerWeapons[i].GetComponent<WeaponBase>());
                        // hide bullet selection
                        UIManager.instance.bulletSelectParent.gameObject.SetActive(false);

                        // reset loadout slot
                        ResetLoadoutSlot(weaponLoadoutSlotList[i]);
                        if (weaponLoadoutSlotList[i].viewingSlot == true) { weaponLoadoutSlotList[i].SelectSlot(); } // deselects the empty weapon slot

                        // remove the weapon from the loadout slot
                        for (int x = 0; x < weaponLoadoutList.Count; x++)
                        {
                            if (weaponLoadoutList[x].equipID != weaponLoadoutSlotList[i].equipID) { continue; }
                            if (weaponLoadoutSlotList[i].equipID == weaponLoadoutList[x].equipID)
                            {
                                weaponLoadoutList.RemoveAt(x);
                                Debug.Log("removed weapon from list with correct equipID");
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                // if the weapon loadout is full, do not add a weapon
                if (weaponLoadoutList.Count >= 4) { Debug.Log("Weapon loadout is full!"); return; }
                // add weapon to loadout
                EquippedWeapon equippedWeapon = new EquippedWeapon();
                equippedWeapon.equippedSlotItem = selectedSlot;
                weaponLoadoutList.Add(equippedWeapon);
                UIManager.instance.bulletSelectParent.gameObject.SetActive(true);

                for (int i = 0; i < weaponLoadoutSlotList.Count; i++)
                {
                    if (weaponLoadoutSlotList[i].inventoryItem == null)
                    {
                        equippedWeapon.equipID = i;
                        weaponLoadoutSlotList[i].inventoryItem = selectedSlot.inventoryItem;
                        weaponLoadoutSlotList[i].weaponEquipped = true;
                        weaponLoadoutSlotList[i].RefreshSlot();
                        playerWeapons[i].GetComponent<WeaponBase>().ChangeWeaponType(weaponLoadoutSlotList[i].inventoryItem.weapon);
                        playerWeapons[i].gameObject.SetActive(true);
                        selectedSlot.weaponEquipped = true;
                        selectedSlot.equipID = equippedWeapon.equipID;
                        UIManager.instance.weaponLoadoutButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unequip";
                        Debug.Log("Equipped Weapon");
                        break;
                    }
                }
            }
        }
    }

    public void UnequipMultipleWeapons(InventorySlot slot)
    {
        if (slot.weaponEquipped == true)
        {
            for (int i = 0; i < weaponLoadoutSlotList.Count; i++)
            {
                if (weaponLoadoutSlotList[i].inventoryItem == null) { continue; }
                if (slot.equipID != weaponLoadoutSlotList[i].equipID) { continue; }
                if (weaponLoadoutSlotList[i] != null) /*&& selectedSlot.isLoadoutSlot == false)*/
                {
                    // if we have selected a weapon in the inventory rather than a loadout slot 
                    // notify the inventory which weapon has been unequipped
                    SearchInventoryForEquippedWeapon(weaponLoadoutSlotList[i]);
                    UIManager.instance.weaponLoadoutButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
                    Debug.Log("Removed weapon");

                    // turn off weapon
                    TurnOffWeapon(playerWeapons[i].GetComponent<WeaponBase>());

                    // reset loadout slot
                    ResetLoadoutSlot(weaponLoadoutSlotList[i]);
                    if (weaponLoadoutSlotList[i].viewingSlot == true) { weaponLoadoutSlotList[i].SelectSlot(); } // deselects the empty weapon slot

                    // remove the weapon from the loadout slot
                    for (int x = 0; x < weaponLoadoutList.Count; x++)
                    {
                        if (weaponLoadoutList[x].equipID != weaponLoadoutSlotList[i].equipID) { continue; }
                        if (weaponLoadoutSlotList[i].equipID == weaponLoadoutList[x].equipID)
                        {
                            weaponLoadoutList.RemoveAt(x);
                            Debug.Log("removed weapon from list with correct equipID");
                        }
                    }
                    break;
                }
            }
        }
    }

    public void TurnOffWeapon(WeaponBase weapon)
    {
        weapon.GetComponent<WeaponBase>().RemoveWeapon();
        weapon.gameObject.SetActive(false);
    }

    public void ResetLoadoutSlot(InventorySlot slot)
    {
        slot.inventoryItem = null;
        slot.RefreshSlot();
        slot.weaponEquipped = false;
    }

    public void DestroyLoadout()
    {
        for (int i = 0; i < weaponLoadoutList.Count; i++)
        {
            SearchForAndRemoveEquippedWeapon(weaponLoadoutList[i].equippedSlotItem);

            TurnOffWeapon(playerWeapons[i].GetComponent<WeaponBase>());
            ResetLoadoutSlot(weaponLoadoutSlotList[i]);
        }

        weaponLoadoutList.Clear();
        RemoveGapsFromInventory();
    }

    public void SearchForAndRemoveEquippedWeapon(InventorySlot weapon)
    {
        for (int i = inventorySlots.Count - 1; i >= 0; i--)
        {
            if (inventorySlots[i].equipID == weapon.equipID && inventorySlots[i].weaponEquipped == true)
            {
                // if the weapon is identical, and the weapon is equipped. Unequip it
                inventorySlots[i].RenewObject();
                inventorySlots[i].RefreshSlot();
                Debug.Log("Removed player weapons after successful infested clear!");
                break;
            }
        }
    }

    public void SearchInventoryForEquippedWeapon(InventorySlot weapon)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].equipID == weapon.equipID && inventorySlots[i].weaponEquipped == true)
            {
                // if the weapon is identical, and the weapon is equipped. Unequip it
                inventorySlots[i].weaponEquipped = false;
                Debug.Log("Found weapon that is equipped with the same ID and will unequip");
                break;
            }
        }
    }

    public void HideSelectedItem(InventorySlot slot)
    {
        if (selectedSlot == slot)
        {
            selectedSlot = null;
            UIManager.instance.weaponLoadoutButton.gameObject.SetActive(false);
            UIManager.instance.infoInventoryName.text = string.Empty;
            UIManager.instance.infoInventoryImage.sprite = null;
            UIManager.instance.infoInventoryImage.enabled = false;
            UIManager.instance.infoInventoryText.text = string.Empty;
            UIManager.instance.infoWeaponInventoryText.text = string.Empty;
            UIManager.instance.weaponStatInventoryText.text = string.Empty;
            UIManager.instance.stackAmountSlider.minValue = 0;
            UIManager.instance.stackAmountSlider.maxValue = 0;
            UIManager.instance.stackAmountSlider.value = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SpecimenObject>())
        {
            InventoryItemPackage obj = collision.gameObject.GetComponent<SpecimenObject>().specimenType;
            AddItemToInventory(obj);
            if (Spawning.instance != null)
            {
                Spawning.instance.specimenPool.AddToPool(collision.gameObject.GetComponent<SpecimenObject>());
            }
            else
            {
                collision.gameObject.SetActive(false);
            }
            Debug.Log("Added new object to inventory: " + obj.itemName);
        }
    }
}
