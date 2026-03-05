using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
public class LevelUpManager : MonoBehaviour
{

    // level up manager - used to handle all the level up stuff with the cards

    public CardManager itemManager;

    // reference to the physical card
    public GameObject CardGO1;
    public GameObject CardGO2;
    public GameObject CardGO3;

    // reference to the 3 cards that can be chosen by the player
    //public List<>

    //private List<CardBase> displayedCards;
    public CardBase chosenCard;
    public Weapon chosenWeapon;

    public Action OnCardChosen;

    //public ItemCardInfo pickedCard1;
    //public ItemCardInfo pickedCard2;
    //public ItemCardInfo pickedCard3;

    //[System.Serializable]
    //public class ItemCardInfo
    //{
    //    //public int itemLevelCount; // the amount of the item obtained 
    //    public LevelCardScriptableObject LevelCardSO;

    //}

    //[SerializeField] public List<ItemCardInfo> cardList = new List<ItemCardInfo>();

    //[SerializeField] public List<ItemCardInfo> cardListToChooseFrom; // just choosing random cards from CardManager.allCardTypes instead

    public static LevelUpManager instance;

    void Start()
    {
        if (itemManager == null)
        {
            itemManager = FindFirstObjectByType<CardManager>();
        }

        // cardListToChooseFrom = new List<ItemCardInfo>(cardList);

        // check for cards
        //if (CardGO1 == null)
        //{
        //    CardGO1 = UIManager.instance.levelUpCards[0];
        //}

        //if (CardGO2 == null)
        //{
        //    CardGO2 = UIManager.instance.levelUpCards[1];
        //}

        //if (CardGO3 == null)
        //{
        //    CardGO3 = UIManager.instance.levelUpCards[2];
        //}
    }


    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GameState.instance.OnPlayerLevelUp += ShowOrHideLevelUpCards;

        OnCardChosen += ApplyChosenCard;
    }

    private void OnDisable()
    {
        GameState.instance.OnPlayerLevelUp -= ShowOrHideLevelUpCards;

        OnCardChosen -= ApplyChosenCard;
    }

    public void ShowOrHideLevelUpCards() // when the player levels up
    {
        if (GameState.instance.player.playerDead == true) { return; }
        if (UIManager.instance.levelUpMenuParent.activeSelf == false)
        {
            UIManager.instance.levelUpMenuParent.SetActive(true);
            RandomiseCards();
        }
        else
        {
            UIManager.instance.levelUpMenuParent.SetActive(false);
        }
    }


    // the randomisation of the card
    public void RandomiseCards()
    {
        // NEW

        for (int i = 0; i < UIManager.instance.levelUpCardUIList.Length; i++)
        {
            if (CardManager.instance.physicalCardPool.Count == 0) { break; }
            int randomCard = Random.Range(0, CardManager.instance.physicalCardPool.Count);
            CardHoster cardHost = UIManager.instance.levelUpCardUIList[i].levelUpCard.GetComponent<CardHoster>();
            //CardScriptableObject cardSO = CardManager.instance.physicalCardPool[randomCard].cardInfo.card;
            CardBase cardBase = CardManager.instance.physicalCardPool[randomCard];
            // remove chosen card from the pool so it doesnt show up more than once - add it back when the card has been chosen by the player
            cardHost.Card = cardBase;
            cardHost.Card.cardInfo = cardBase.cardInfo;
            cardHost.Card.SearchForExistingSelectedCardInfo();

            // randomise the cards stat
            cardHost.Card.RandomiseStatAmount();

            // remove any weapon that had been selected previously for a card
            cardHost.chosenWeapon = null;
            chosenWeapon = null;

            if (cardBase.cardInfo.card.defaultCard == true)
            {
                // select a weapon for the card stats to go to 
                int weaponPos = Random.Range(0, GameState.instance.playerInventory.weaponLoadoutList.Count);
                cardHost.chosenWeapon = GameState.instance.playerInventory.playerWeapons[weaponPos].GetComponent<Weapon>();

                // show a specific upgrade stat on a single weapon 
                UIManager.instance.levelUpCardUIList[i].levelUpItemName.text = cardHost.chosenWeapon.weaponName; // name
                UIManager.instance.levelUpCardUIList[i].levelUpItemStatDescription.text = "+ " + cardHost.Card.cardInfo.cardName + "\n" + "\n" + cardHost.Card.statUpgradeAmount.ToString("F1"); ; // description
                UIManager.instance.levelUpCardUIList[i].levelUpItemLevel.text = cardHost.chosenWeapon.weaponLevel.ToString(); // weapon level
                UIManager.instance.levelUpCardUIList[i].levelUpItemImage.sprite = cardHost.chosenWeapon.weaponType.weaponSprite; // weapon sprite 
            }
            else
            {
                // show a flat stat that upgrades all weapons
                UIManager.instance.levelUpCardUIList[i].levelUpItemName.text = cardHost.Card.cardInfo.cardName; // name
                UIManager.instance.levelUpCardUIList[i].levelUpItemStatDescription.text = "+ " + cardHost.Card.cardInfo.cardName + "\n" + "\n" + cardHost.Card.statUpgradeAmount.ToString("F1"); // description
                UIManager.instance.levelUpCardUIList[i].levelUpItemLevel.text = cardHost.Card.cardInfo.cardLevel.ToString(); // card level
                UIManager.instance.levelUpCardUIList[i].levelUpItemImage.sprite = cardHost.Card.cardInfo.cardSprite; // card sprite icon
            }

            CardManager.instance.physicalCardPool.RemoveAt(randomCard);
        }

        // OLD

        // setup list of cards to pick from
        //cardListToChooseFrom = new List<ItemCardInfo>(cardList);

        //// reset cards chosen if needed
        //if (pickedCard1 != null && pickedCard2 != null && pickedCard3 != null)
        //{
        //    pickedCard1 = null;
        //    pickedCard2 = null;
        //    pickedCard3 = null;
        //}

        //#region Card 1
        //int randomCardIndex1 = Random.Range(0, cardListToChooseFrom.Count); // seperate int so it can be removed later
        //pickedCard1 = cardListToChooseFrom[randomCardIndex1]; // chosen 'card'

        ////Debug.Log("Card 1 choice:" + pickedCard1.LevelCardSO.cardName);

        //UIManager.instance.levelUpItemName[0].text = pickedCard1.LevelCardSO.cardName; // name
        //UIManager.instance.levelUpItemStatDescription[0].text = pickedCard1.LevelCardSO.cardText; // description

        //int itemLevel1 = CheckCardItemEffectType(pickedCard1);
        //UIManager.instance.levelUpItemLevel[0].text = "Level: " + itemLevel1.ToString(); // item level

        //UIManager.instance.levelUpItemImage[0].sprite = pickedCard1.LevelCardSO.cardSprite; // sprite icon

        //cardListToChooseFrom.RemoveAt(randomCardIndex1); // remove from temp pool of cards
        //#endregion

        //#region Card 2
        //int randomCardIndex2 = Random.Range(0, cardListToChooseFrom.Count);
        //pickedCard2 = cardListToChooseFrom[randomCardIndex2];

        ////Debug.Log("Card 2 choice:" + pickedCard2.LevelCardSO.cardName);

        //UIManager.instance.levelUpItemName[1].text = pickedCard2.LevelCardSO.cardName;
        //UIManager.instance.levelUpItemStatDescription[1].text = pickedCard2.LevelCardSO.cardText;

        //int itemLevel2 = CheckCardItemEffectType(pickedCard2);
        //UIManager.instance.levelUpItemLevel[1].text = "Level: " + itemLevel2.ToString();

        //UIManager.instance.levelUpItemImage[1].sprite = pickedCard2.LevelCardSO.cardSprite;

        //cardListToChooseFrom.RemoveAt(randomCardIndex2);
        //#endregion

        //#region Card 3
        //int randomCardIndex3 = Random.Range(0, cardListToChooseFrom.Count);
        //pickedCard3 = cardListToChooseFrom[randomCardIndex3];

        ////Debug.Log("Card 3 choice:" + pickedCard3.LevelCardSO.cardName);

        //UIManager.instance.levelUpItemName[2].text = pickedCard3.LevelCardSO.cardName;
        //UIManager.instance.levelUpItemStatDescription[2].text = pickedCard3.LevelCardSO.cardText;

        //int itemLevel3 = CheckCardItemEffectType(pickedCard3);
        //UIManager.instance.levelUpItemLevel[2].text = "Level: " + itemLevel3.ToString();

        //UIManager.instance.levelUpItemImage[2].sprite = pickedCard3.LevelCardSO.cardSprite;

        //cardListToChooseFrom.RemoveAt(randomCardIndex3);
        //#endregion
    }

    public void ApplyChosenCard()
    {
        // NEW
        chosenCard.AddStatUpgrade();
        ShowOrHideLevelUpCards();
        GameState.instance.OnPlayerLevelUp?.Invoke();
    }

    // card 1 2 3
    //public void CardSelected(int cardNumSelected) // after the players picks a card
    //{
    //    // OLD
    //    int itemLevelSelect = -1;

    //    if (cardNumSelected == 1)
    //    {
    //        //Debug.Log(pickedCard1.LevelCardSO.cardEffectType);
    //        itemLevelSelect = EffectTypeCardCount(pickedCard1);
    //    }

    //    else if (cardNumSelected == 2)
    //    {
    //        //Debug.Log(pickedCard2.LevelCardSO.cardEffectType);
    //        itemLevelSelect = EffectTypeCardCount(pickedCard2);
    //    }

    //    else if (cardNumSelected == 3)
    //    {
    //        //Debug.Log(pickedCard3.LevelCardSO.cardEffectType);
    //        itemLevelSelect = EffectTypeCardCount(pickedCard3);
    //    }

    //    else
    //    {
    //        Debug.Log("CARD ERROR SELECTED: Num " + cardNumSelected);
    //    }

    //    if (itemLevelSelect != -1)
    //    {
    //        // add +1 to the selected item from the card
    //        itemManager.itemCountGO[itemLevelSelect]++;
    //        Debug.Log(itemManager.itemNamesGO[itemLevelSelect] + " level increased to: " + itemManager.itemCountGO[itemLevelSelect]); // debug level after

    //    }

    //    itemManager.ItemScaleAllWeapons(); // scale weapons with items as a new item has been acquired

    //    GameState.instance.OnPlayerLevelUp?.Invoke(); // after card is done
    //}

    // all card item effect types should be checked
    //public int CheckCardItemEffectType(ItemCardInfo cardToCheck)
    //{
    //    int itemManagerListValue = -1;

    //    if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.AttackDamage)
    //    {
    //        // NUMBER BASED ON ITEM MANAGER LIST - ITEM LIST GO
    //        itemManagerListValue = 0;
    //    }
    //    else if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.AttackSpeed)
    //    {
    //        itemManagerListValue = 3;
    //    }
    //    else if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.CritChance)
    //    {
    //        itemManagerListValue = 1;
    //    }
    //    else if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.CritDamage)
    //    {
    //        itemManagerListValue = 2;
    //    }

    //    int itemCountValue = CheckItemManagerItemLevel(itemManagerListValue);


    //    return itemCountValue;
    //}

    // item manager level for reference
    //public int CheckItemManagerItemLevel(int listValue)
    //{
    //    int itemlevel = -1;

    //    itemlevel = itemManager.itemCountGO[listValue];

    //    return itemlevel;
    //}

    // check the item count of the card the player selected
    //public int EffectTypeCardCount(ItemCardInfo cardToCheck)
    //{
    //    int itemManagerListValue = -1;

    //    if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.AttackDamage)
    //    {
    //        // NUMBER BASED ON ITEM MANAGER LIST - ITEM LIST GO
    //        itemManagerListValue = 0;
    //    }
    //    else if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.AttackSpeed)
    //    {
    //        itemManagerListValue = 3;
    //    }
    //    else if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.CritChance)
    //    {
    //        itemManagerListValue = 1;
    //    }
    //    else if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.CritDamage)
    //    {
    //        itemManagerListValue = 2;
    //    }

    //    return itemManagerListValue;

    //}
}
