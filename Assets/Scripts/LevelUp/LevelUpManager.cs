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
        public int itemLevelCount; // the amount of the item obtained 
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


        // BUG FIX ---------------------------------------------------
        Debug.Log(pickedCard1.LevelCardSO.cardName + " aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"); // test
        Debug.Log(cardListToChooseFrom[randomCardIndex1].LevelCardSO.name + "bbbbbbbbbbbbbbbbbb");



        //set text
        UIManager.instance.levelUpItemName[0].text = pickedCard1.LevelCardSO.cardName;
        UIManager.instance.levelUpItemStatDescription[0].text = pickedCard1.LevelCardSO.cardText; 

        cardListToChooseFrom.RemoveAt(randomCardIndex1); // remove from temp pool of cards

        // debug after remove (BUG FIX)
        Debug.Log(cardListToChooseFrom[0].LevelCardSO.cardName);
        Debug.Log(cardListToChooseFrom[1].LevelCardSO.cardName);
        Debug.Log(cardListToChooseFrom[2].LevelCardSO.cardName);


        // Card 2

        // Card 3

    }

    public void CardSelected() // after the players picks a card
    {
        GameState.instance.OnPlayerLevelUp?.Invoke(); // after card is done
    }

}
