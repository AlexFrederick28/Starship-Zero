using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

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

        cardListToChooseFrom = new List<ItemCardInfo>(cardList);

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
        #region Card 1
        ItemCardInfo pickedCard = cardListToChooseFrom[Random.Range(0, cardListToChooseFrom.Count)];
        
        #endregion

        #region Card 2

        #endregion

        #region Card 3

        #endregion

    }

    public void CardSelected() // after the players picks a card
    {
        GameState.instance.OnPlayerLevelUp?.Invoke(); // after card is done
    }

}
