using UnityEngine;
using UnityEngine.EventSystems;

public class CardBase : MonoBehaviour
{
    // this obejcts cardInfo is populated when the card has been randomised, having it equal to whatever cardInfo was chosen from the CardManager
    public CardManager.CardInfo cardInfo;

    public float statUpgradeAmount = 0;

    protected virtual void OnEnable()
    {
        // need to subscribe to OnPlayerLevelUp to get the correct item level (probably using a loop on the ActiveCardList - or even a lambda expression. If there are no cards of that type active, add it and set the level to 1)
        // i believe we do not need that anymore ^
    }

    protected virtual void OnDisable()
    {
        
    }

    public virtual void Start()
    {
        
    }

    public void SearchForExistingSelectedCardInfo()
    {
        if (CardManager.instance.allSelectedCardsList.Count > 0)
        {
            for (int i = 0; i < CardManager.instance.allSelectedCardsList.Count; i++)
            {
                // if the card is a default one (A card that applies to a specific weapon) - then continue
                if (CardManager.instance.allSelectedCardsList[i].card != cardInfo.card || cardInfo.defaultCard == true) { continue; }
                else
                {
                    cardInfo.cardLevel = CardManager.instance.allSelectedCardsList[i].cardLevel;
                    break;
                }

                // if the loop gets to this stage, it means there are no cards and the level will be 0
            }
        }
        else
        {
            Debug.Log("First card of the type has been chosen");
        }
    }

    public void RandomiseStatAmount()
    {
        statUpgradeAmount = Random.Range(cardInfo.card.cardScalingMin, cardInfo.card.cardScalingMax);
    }

    /// <summary>
    /// override this function to add custom card logic, this could include adding to a weapons stat, or adding a stat to the player character and so on.
    /// </summary>
    public virtual void AddStatUpgrade()
    {
        Debug.Log("Added new stat from chosen card!");
        LevelUpManager.instance.ShowOrHideLevelUpCards();
    }

    public void PopulateCardInfoOnSpawn()
    {
        cardInfo.defaultCard = cardInfo.card.defaultCard;
        cardInfo.cardName = cardInfo.card.name;
        cardInfo.cardDescription = cardInfo.card.cardDescription;
        cardInfo.cardScalingMax = cardInfo.card.cardScalingMax;
        cardInfo.cardScalingMin = cardInfo.card.cardScalingMin;
        cardInfo.cardLevel = cardInfo.card.cardLevel;
        cardInfo.cardSprite = cardInfo.card.cardSprite;
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (eventData.button == PointerEventData.InputButton.Left)
    //    {
    //        LevelUpManager.instance.chosenCard = this;
    //        LevelUpManager.instance.OnCardChosen?.Invoke();
    //    }
    //}
}
