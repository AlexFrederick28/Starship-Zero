using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // Item Manager - stores all the info of items and gives their effects to the weapon

    [Serializable]
    public class CardInfo
    {
        public CardScriptableObject card;
        //public CardBase cardBehaviour;
        public bool defaultCard = false;
        public string cardName;
        public string cardDescription;
        public float cardScalingMin;
        public float cardScalingMax;
        public int cardLevel = 0;
        public Sprite cardSprite;
    }

    //[SerializeField] public List<GameObject> itemListGO;
    [Header("Cards")]
    public List<CardScriptableObject> allCardTypes; // every type of card in the game
    public List<CardInfo> allSelectedCardsList; // all cards that are selected to be in a run using the card upgrades by the player - the info on the cards will be populated when they are made active
    public List<CardInfo> playerActiveCardList; // the active selected cards from player level ups

    [Header("Card Pool")]
    public GameObject poolParentToSpawn;
    public GameObject poolParent;
    public List<CardBase> physicalCardPool = new List<CardBase>(); // the pool of cards that we need references of to play the unqiue card behaviours
    public GameObject[] physicalCardPrefabs; // all the prefabs of cards to be spawned and managed by the pool

    // a card is selected upon level up
    // use a function connected to the card that adds the stat to wherever it needs to go (such as to the player stats, or a weapons stats)
    // check to see if the player has any of the same active cards, if so add to the cards level

    //[SerializeField] public List<string> itemNamesGO;
    //[SerializeField] public List<int> itemScalingGO;
    //[SerializeField] public List<int> itemCountGO;
    //[SerializeField] public List<string> itemDescriptionGO;
    //[SerializeField] public List<Sprite> itemSpritesGO;

    //[SerializeField] public List<int> itemCountInitial; // used to reset the item count on death to its initial on restart

    public WeaponBase[] allWeapons;

    public static CardManager instance;

    protected void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (GameState.instance != null)
        {
            //GameState.instance.OnEnteringInfestedRoom += ResetItemCount;
            //GameState.instance.OnEnteringInfestedRoom += ItemScaleAllWeapons;
            GameState.instance.OnEnteringInfestedRoom += ClearActiveCards;

            //GameState.instance.OnPlayerRespawn += ResetItemCount;
            //GameState.instance.OnPlayerRespawn += ItemScaleAllWeapons;
            GameState.instance.OnPlayerRespawn += ClearActiveCards;

            //GameState.instance.OnPlayerRetry += ResetItemCount;
            //GameState.instance.OnPlayerRetry += ItemScaleAllWeapons;
            GameState.instance.OnPlayerRetry += ClearActiveCards;

            //GameState.instance.OnCompletedInfestedClear += ResetItemCount;
            //GameState.instance.OnCompletedInfestedClear += ItemScaleAllWeapons;
            GameState.instance.OnCompletedInfestedClear += ClearActiveCards;
        }
    }

    protected void OnDisable()
    {
        //GameState.instance.OnEnteringInfestedRoom -= ResetItemCount;
        //GameState.instance.OnEnteringInfestedRoom -= ItemScaleAllWeapons;
        GameState.instance.OnEnteringInfestedRoom -= ClearActiveCards;

        //GameState.instance.OnPlayerRespawn -= ResetItemCount;
        //GameState.instance.OnPlayerRespawn -= ItemScaleAllWeapons;
        GameState.instance.OnPlayerRespawn -= ClearActiveCards;

        //GameState.instance.OnPlayerRetry -= ResetItemCount;
        //GameState.instance.OnPlayerRetry -= ItemScaleAllWeapons;
        GameState.instance.OnPlayerRetry -= ClearActiveCards;

        //GameState.instance.OnCompletedInfestedClear -= ResetItemCount;
        //GameState.instance.OnCompletedInfestedClear -= ItemScaleAllWeapons;
        GameState.instance.OnCompletedInfestedClear -= ClearActiveCards;
    }

    // initialise
    void Start()
    {
        //foreach (GameObject item in itemListGO)
        //{
        //    Instantiate(item, transform, worldPositionStays: false);
        //}

        //ItemStats(); 
        //ItemInfo(); 
        //ItemScaleAllWeapons();

        SpawnCardObjects();
        TempFunctionPopulateSelectedCardsInfo();
    }

    public void SpawnCardObjects()
    {
        // spawning gameobjects that house all the different unique card behaviours, these behaviours are referenced through CardBase
        if (poolParent == null)
        {
            poolParent = Instantiate(poolParentToSpawn, transform.position, Quaternion.identity);
        }

        for (int i = 0; i < physicalCardPrefabs.Length; i++)
        {
            // look past the cards that are not default, as we only want the default ones to be added on start and made active permanently
            // all none default cards need to be unlocked and made active by the player
            if (physicalCardPrefabs[i].GetComponent<CardBase>().cardInfo.card.defaultCard == false) { continue; }
            GameObject newPhysicalCard = physicalCardPrefabs[i];

            InstantiateAsync(newPhysicalCard, poolParent.transform);
            CardBase newBase = newPhysicalCard.GetComponent<CardBase>();
            newBase.GetComponent<CardBase>().PopulateCardInfoOnSpawn();
            physicalCardPool.Add(newBase);

            if (newBase.cardInfo.defaultCard == true)
            {
                //EnableCardInPool(newBase);
                allSelectedCardsList.Add(newBase.cardInfo);
            }
        }

        Debug.Log("Spawned card pool");
    }

    public void AddPhysicalCard(CardScriptableObject so)
    {
        GameObject physicalCard = physicalCardPrefabs.First(s => s.GetComponent<CardBase>().cardInfo.card == so);

        InstantiateAsync(physicalCard, poolParent.transform);
        CardBase newBase = physicalCard.GetComponent<CardBase>();
        newBase.GetComponent<CardBase>().PopulateCardInfoOnSpawn();
        physicalCardPool.Add(newBase);
    }

    public void RemovePhysicalCard(CardScriptableObject so)
    {
        GameObject physicalCard = physicalCardPrefabs.First(s => s.GetComponent<CardBase>().cardInfo.card == so);

        InstantiateAsync(physicalCard, poolParent.transform);
        CardBase newBase = physicalCard.GetComponent<CardBase>();
        newBase.GetComponent<CardBase>().PopulateCardInfoOnSpawn();
        physicalCardPool.Remove(newBase);
        Destroy(newBase.gameObject);
    }

    //public void EnableCardInPool(CardBase card) // this function is pretty much redundant now
    //{
    //    // all cards are added to the pool
    //    // cards are enabled when the player wants them active from the card upgrade menu, or the cards are default and are always enabled
    //    card.gameObject.SetActive(true);
    //    card.enabled = true;
    //}

    //public void DisableCardInPool(CardBase card) // this function is pretty much redundant now
    //{
    //    // will only need to remove a card from the pool when the player doesnt have the card active/unlocked
    //    card.gameObject.SetActive(false);
    //    card.enabled = false;
    //}

    //public void DestroyCardObjects()
    //{
    //    for (int i = 0; i < physicalCardPool.Count; i++)
    //    {
    //        physicalCardPool[i].gameObject.SetActive(false);
    //    }
    //}

    public void PopulateSelectedCardInfo(CardInfo info)
    {
        CardScriptableObject newInstance = ScriptableObject.CreateInstance<CardScriptableObject>();
        newInstance = info.card;

        info.defaultCard = newInstance.defaultCard;
        info.cardName = newInstance.cardName;
        info.cardDescription = newInstance.cardDescription;
        info.cardScalingMin = newInstance.cardScalingMin;
        info.cardScalingMax = newInstance.cardScalingMax;
        info.cardLevel = newInstance.cardLevel;
        info.cardSprite = newInstance.cardSprite;
    }

    public void TempFunctionPopulateSelectedCardsInfo()
    {
        // this function will be removed - the card info should be populated when equipping a card from the upgrade menu, unless it is a default card
        for (int i = 0; i < allSelectedCardsList.Count; i++)
        {
            CardScriptableObject newInstance = ScriptableObject.CreateInstance<CardScriptableObject>();
            newInstance = allSelectedCardsList[i].card;
            allSelectedCardsList[i].defaultCard = newInstance.defaultCard;
            allSelectedCardsList[i].cardName = newInstance.cardName;
            allSelectedCardsList[i].cardDescription = newInstance.cardDescription;
            allSelectedCardsList[i].cardScalingMin = newInstance.cardScalingMin;
            allSelectedCardsList[i].cardScalingMax = newInstance.cardScalingMax;
            allSelectedCardsList[i].cardLevel = newInstance.cardLevel;
        }

        Debug.Log("Set cards!");
    }

    // grabs the amount of items at the start/end of a room
    //public void RecordInitialItemCount()
    //{
    //    Debug.Log("Record Initial Item Count");
    //    //itemCountInitial = new List<int>(allCardInfoList);
    //}

    public void ClearActiveCards()
    {
        if (playerActiveCardList == null) { return; }
        playerActiveCardList.Clear();
    }

    // rewind the amount of items a player at the start of a room
    //public void ResetCardLevels()
    //{
    //    Debug.Log("Reset Initial Item Count");

    //    // dont need this, as the active cards will be cleared each time the player dies or a room is complete
    //    // reset card to default
    //    for (int i = 0; i < allSelectedCardsList.Count; i++)
    //    {
    //        allSelectedCardsList[i].cardLevel = 0;
    //    }

    //    //if (itemCountInitial.Count > 0)
    //    //{
    //    //    itemCountGO = new List<int>(itemCountInitial);
    //    //}
    //    //else
    //    //{
    //    //    Debug.Log("Nothing to Reset back to");
    //    //    ResetItemCount();
    //    //}

    //    // instead of scaling the weapon this way, it must be scaled from the card, not the weapon itself.
    //    //foreach (WeaponBase weapon in allWeapons)
    //    //{
    //    //    weapon.ItemScaling();
    //    //}
    //}

    // finds all weapons and runs a function to calculate the items on them
    //public void ItemScaleAllWeapons()
    //    // essentially resets a weapons stats - needs a rework
    //{
    //    //allWeapons = FindObjectsByType<WeaponBase>(FindObjectsSortMode.None);

    //    //foreach (WeaponBase weapon in allWeapons)
    //    //{
    //    //    weapon.ItemScaling();
    //    //}
    //}

    // finds the 2 stat items that affect a weapons stats
    //public void ItemStats() 
    //{
    //    // resets the items. has to do with the spawning of gameobjects to store card info - old
    //    // reset
    //    //itemScalingGO.Clear();
    //    //itemCountGO.Clear();

    //    //foreach (GameObject itemGO in itemListGO)
    //    //{
    //    //    ItemBase itemBase = itemGO.GetComponent<ItemBase>();

    //    //    if (itemBase == null)
    //    //    {
    //    //        Debug.Log("[" + itemGO.name + "] has no ItemBase ");
    //    //        return;
    //    //    }

    //    //    CardScriptableObject item = itemBase.itemType;


    //    //    itemScalingGO.Add(item.cardScaling);
    //    //    itemCountGO.Add(item.cardLevel);

    //    //    //Debug.Log("Item: Scaling [" + item.itemScaling + "], Count [" + item.itemCount + "]");
    //    //}
    //}

    // the information side of the items such as name and what they do
    //public void ItemInfo()   
    //{
    //    //// reset
    //    //itemNamesGO.Clear();
    //    //itemDescriptionGO.Clear();
    //    //itemSpritesGO.Clear();

    //    //foreach (GameObject itemGO in itemListGO)
    //    //{
    //    //    ItemBase itemBase = itemGO.GetComponent<ItemBase>();

    //    //    if (itemBase == null)
    //    //    {
    //    //        Debug.Log("[" + itemGO.name + "] has no ItemBase ");
    //    //        return;
    //    //    }

    //    //    CardScriptableObject item = itemBase.itemType;

    //    //    itemNamesGO.Add(item.cardName);
    //    //    itemDescriptionGO.Add(item.cardDescription);
    //    //    itemSpritesGO.Add(item.cardSprite);

    //    //    //Debug.Log("Item: Name [" + item.itemName + "], Description [" + item.itemDescription + "]");
    //    //}
    //}

    // reset all items to 0
    //public void ResetItemCount()
    //{

    //    //for (int i = 0; i < itemCountGO.Count; i++)
    //    //{
    //    //    itemCountGO[i] = 0;
    //    //}

    //    Debug.Log("Reset Item Count");

    //}
}
