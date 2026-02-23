using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelUpManager : MonoBehaviour
{
    [SerializeField] public ItemManager itemManager;

    [SerializeField] public GameObject CardGO1;
    [SerializeField] public GameObject CardGO2;
    [SerializeField] public GameObject CardGO3;


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


    // NOTE - for item level text try "Level: itemLevelCount"
    public void RandomiseCards()
    {
        // setup list of cards to pick from
        cardListToChooseFrom = new List<ItemCardInfo>(cardList);

        // Card 1
        int randomCardIndex1 = Random.Range(0, cardListToChooseFrom.Count); // seperate int so it can be removed later
        ItemCardInfo pickedCard1 = cardListToChooseFrom[randomCardIndex1]; // chosen 'card'

        //Debug.Log("Card 1 choice:" + pickedCard1.LevelCardSO.cardName);

        UIManager.instance.levelUpItemName[0].text = pickedCard1.LevelCardSO.cardName;
        UIManager.instance.levelUpItemStatDescription[0].text = pickedCard1.LevelCardSO.cardText;
        UIManager.instance.levelUpItemLevel[0].text = pickedCard1.LevelCardSO.cardItemCount.ToString();
        UIManager.instance.levelUpItemImage[0].sprite = pickedCard1.LevelCardSO.cardSprite;

        cardListToChooseFrom.RemoveAt(randomCardIndex1); // remove from temp pool of cards


        // Card 2
        int randomCardIndex2 = Random.Range(0, cardListToChooseFrom.Count);
        ItemCardInfo pickedCard2 = cardListToChooseFrom[randomCardIndex2];

        //Debug.Log("Card 2 choice:" + pickedCard2.LevelCardSO.cardName);

        UIManager.instance.levelUpItemName[1].text = pickedCard2.LevelCardSO.cardName;
        UIManager.instance.levelUpItemStatDescription[1].text = pickedCard2.LevelCardSO.cardText;
        UIManager.instance.levelUpItemLevel[1].text = pickedCard2.LevelCardSO.cardItemCount.ToString();
        UIManager.instance.levelUpItemImage[1].sprite = pickedCard2.LevelCardSO.cardSprite;

        cardListToChooseFrom.RemoveAt(randomCardIndex2);

        // Card 3
        int randomCardIndex3 = Random.Range(0, cardListToChooseFrom.Count);
        ItemCardInfo pickedCard3 = cardListToChooseFrom[randomCardIndex3];

        //Debug.Log("Card 3 choice:" + pickedCard3.LevelCardSO.cardName);

        UIManager.instance.levelUpItemName[2].text = pickedCard3.LevelCardSO.cardName;
        UIManager.instance.levelUpItemStatDescription[2].text = pickedCard3.LevelCardSO.cardText;
        UIManager.instance.levelUpItemLevel[2].text = pickedCard3.LevelCardSO.cardItemCount.ToString();
        UIManager.instance.levelUpItemImage[2].sprite = pickedCard3.LevelCardSO.cardSprite;

        cardListToChooseFrom.RemoveAt(randomCardIndex3);

    }

    public void CardSelected() // after the players picks a card
    {
        GameState.instance.OnPlayerLevelUp?.Invoke(); // after card is done
    }

}
