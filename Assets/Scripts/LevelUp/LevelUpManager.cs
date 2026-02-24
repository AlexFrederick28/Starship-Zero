using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;
public class LevelUpManager : MonoBehaviour
{
    [SerializeField] public ItemManager itemManager;

    // reference to the physical card
    [SerializeField] public GameObject CardGO1;
    [SerializeField] public GameObject CardGO2;
    [SerializeField] public GameObject CardGO3;

    // reference to the 3 cards that can be chosen by the player
    [SerializeField] public ItemCardInfo pickedCard1;
    [SerializeField] public ItemCardInfo pickedCard2;
    [SerializeField] public ItemCardInfo pickedCard3;

    [System.Serializable]
    public class ItemCardInfo
    {
        //public int itemLevelCount; // the amount of the item obtained 
        public LevelCardScriptableObject LevelCardSO;

    }

    [SerializeField] public List<ItemCardInfo> cardList = new List<ItemCardInfo>();

    [SerializeField] public List<ItemCardInfo> cardListToChooseFrom;

    void Start()
    {
        if (itemManager == null)
        {
            itemManager = FindFirstObjectByType<ItemManager>();
        }

        // cardListToChooseFrom = new List<ItemCardInfo>(cardList);

        if (CardGO1 == null)
        {
            CardGO1 = UIManager.instance.levelUpCards[0];
        }

        if (CardGO2 == null)
        {
            CardGO2 = UIManager.instance.levelUpCards[1];
        }

        if (CardGO3 == null)
        {
            CardGO3 = UIManager.instance.levelUpCards[2];
        }
    }


    private void OnEnable()
    {
        GameState.instance.OnPlayerLevelUp += LevelUpCards;
    }

    private void OnDisable()
    {
        GameState.instance.OnPlayerLevelUp -= LevelUpCards;
    }

    public void LevelUpCards() // when the player levels up
    {
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



    public void RandomiseCards()
    {
        // setup list of cards to pick from
        cardListToChooseFrom = new List<ItemCardInfo>(cardList);

        // reset cards chosen if needed
        if (pickedCard1 != null && pickedCard2 != null && pickedCard3 != null)
        {
            pickedCard1 = null;
            pickedCard2 = null;
            pickedCard3 = null;
        }

        #region Card 1
        int randomCardIndex1 = Random.Range(0, cardListToChooseFrom.Count); // seperate int so it can be removed later
        pickedCard1 = cardListToChooseFrom[randomCardIndex1]; // chosen 'card'

        //Debug.Log("Card 1 choice:" + pickedCard1.LevelCardSO.cardName);

        UIManager.instance.levelUpItemName[0].text = pickedCard1.LevelCardSO.cardName;
        UIManager.instance.levelUpItemStatDescription[0].text = pickedCard1.LevelCardSO.cardText;

        int itemLevel1 = CheckCardItemEffectType(pickedCard1);
        UIManager.instance.levelUpItemLevel[0].text = "Level: " + itemLevel1.ToString();

        UIManager.instance.levelUpItemImage[0].sprite = pickedCard1.LevelCardSO.cardSprite;

        cardListToChooseFrom.RemoveAt(randomCardIndex1); // remove from temp pool of cards
        #endregion

        #region Card 2
        int randomCardIndex2 = Random.Range(0, cardListToChooseFrom.Count);
        pickedCard2 = cardListToChooseFrom[randomCardIndex2];

        //Debug.Log("Card 2 choice:" + pickedCard2.LevelCardSO.cardName);

        UIManager.instance.levelUpItemName[1].text = pickedCard2.LevelCardSO.cardName;
        UIManager.instance.levelUpItemStatDescription[1].text = pickedCard2.LevelCardSO.cardText;

        int itemLevel2 = CheckCardItemEffectType(pickedCard2);
        UIManager.instance.levelUpItemLevel[1].text = "Level: " + itemLevel2.ToString();

        UIManager.instance.levelUpItemImage[1].sprite = pickedCard2.LevelCardSO.cardSprite;

        cardListToChooseFrom.RemoveAt(randomCardIndex2);
        #endregion

        #region Card 3
        int randomCardIndex3 = Random.Range(0, cardListToChooseFrom.Count);
        pickedCard3 = cardListToChooseFrom[randomCardIndex3];

        //Debug.Log("Card 3 choice:" + pickedCard3.LevelCardSO.cardName);

        UIManager.instance.levelUpItemName[2].text = pickedCard3.LevelCardSO.cardName;
        UIManager.instance.levelUpItemStatDescription[2].text = pickedCard3.LevelCardSO.cardText;

        int itemLevel3 = CheckCardItemEffectType(pickedCard3);
        UIManager.instance.levelUpItemLevel[2].text = "Level: " + itemLevel3.ToString();

        UIManager.instance.levelUpItemImage[2].sprite = pickedCard3.LevelCardSO.cardSprite;

        cardListToChooseFrom.RemoveAt(randomCardIndex3);
        #endregion
    }

    // card 1 2 3
    public void CardSelected(int cardNumSelected) // after the players picks a card
    {
        int itemLevelSelect = -1;

        if (cardNumSelected == 1)
        {
            Debug.Log(pickedCard1.LevelCardSO.cardEffectType);
            itemLevelSelect = EffectTypeCardCount(pickedCard1);
        }

        else if (cardNumSelected == 2)
        {
            itemLevelSelect = EffectTypeCardCount(pickedCard2);
        }

        else if (cardNumSelected == 3)
        {
            itemLevelSelect = EffectTypeCardCount(pickedCard3);
        }

        else
        {
            Debug.Log("CARD ERROR SELECTED: Num " + cardNumSelected);
        }

        if (itemLevelSelect != -1)
        {

            itemManager.itemCountGO[itemLevelSelect]++;
            Debug.Log(itemManager.itemNamesGO[itemLevelSelect] + " level increased to: " + itemManager.itemCountGO[itemLevelSelect]);

        }

        itemManager.ItemScaleAllWeapons();

        GameState.instance.OnPlayerLevelUp?.Invoke(); // after card is done
    }

    // all card item effect types should be checked
    public int CheckCardItemEffectType(ItemCardInfo cardToCheck)
    {
        int itemManagerListValue = -1;

        if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.AttackDamage)
        {
            // NUMBER BASED ON ITEM MANAGER LIST - ITEM LIST GO
            itemManagerListValue = 0;
        }
        else if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.AttackSpeed)
        {
            itemManagerListValue = 3;
        }
        else if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.CritChance)
        {
            itemManagerListValue = 1;
        }
        else if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.CritDamage)
        {
            itemManagerListValue = 2;
        }

        int itemCountValue = CheckItemManagerItemLevel(itemManagerListValue);


        return itemCountValue;
    }

    public int CheckItemManagerItemLevel(int listValue)
    {
        int itemlevel = -1;

        itemlevel = itemManager.itemCountGO[listValue];

        return itemlevel;
    }

    public int EffectTypeCardCount(ItemCardInfo cardToCheck)
    {
        int itemManagerListValue = -1;

        if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.AttackDamage)
        {
            // NUMBER BASED ON ITEM MANAGER LIST - ITEM LIST GO
            itemManagerListValue = 0;
        }
        else if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.AttackSpeed)
        {
            itemManagerListValue = 3;
        }
        else if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.CritChance)
        {
            itemManagerListValue = 1;
        }
        else if (cardToCheck.LevelCardSO.cardEffectType == LevelCardScriptableObject.CardItemEffect.CritDamage)
        {
            itemManagerListValue = 2;
        }

        return itemManagerListValue;

    }
}
