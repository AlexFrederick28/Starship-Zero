using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardBase : MonoBehaviour
{
    // this obejcts cardInfo is populated when the card has been randomised, having it equal to whatever cardInfo was chosen from the CardManager
    public CardManager.CardInfo cardInfo;
    public int totalStatAmount;
    public int statUpgradeAmount = 0;

    protected virtual void OnEnable()
    {
        //LevelUpManager.instance.OnCardChosen += AddBackToPhysicalCardPool;

        WeaponBase.OnDamagingEnemy += CustomOnHitEvent;

        GameState.instance.OnPlayerRespawn += ResetTotalStatAmount;
        GameState.instance.OnPlayerRetry += ResetTotalStatAmount;
        GameState.instance.OnCompletedInfestedClear += ResetTotalStatAmountFromInfestedRoomCompletion;
    }

    protected virtual void OnDisable()
    {
        //LevelUpManager.instance.OnCardChosen -= AddBackToPhysicalCardPool;

        WeaponBase.OnDamagingEnemy -= CustomOnHitEvent;

        GameState.instance.OnPlayerRespawn -= ResetTotalStatAmount;
        GameState.instance.OnPlayerRetry -= ResetTotalStatAmount;
        GameState.instance.OnCompletedInfestedClear -= ResetTotalStatAmountFromInfestedRoomCompletion;
    }

    public virtual void Start()
    {
        
    }

    /// <summary>
    /// Is used to apply any sort of affect that requires on hit such as life steal - must be overrided. The event is called each time a weapon deals damage.
    /// </summary>
    /// <param name="damageDealt"></param>
    /// <param name="enemy"></param>
    public virtual void CustomOnHitEvent(float damageDealt, EnemyBase enemy)
    {
        //Debug.Log("Applied on hit affect!");
    }

    //public void AddBackToPhysicalCardPool()
    //{
    //    if (CardManager.instance.physicalCardPool.Contains(this) == true) { Debug.Log("Card still in physical card pool - returning."); return; }
    //    else
    //    {
    //        if (LevelUpManager.instance.chosenCard != this)
    //        {
    //            CardManager.instance.playerActiveCardList.Add(this);
    //        }

    //        CardManager.instance.ReAddPhysicalCard(this);
    //        // problem: the physical card list is being taken from when randomisng the cards on the level up manager
    //        // the cards that wernt chosen are not being added back - however we cannot just add a random instance back, we must add the previous instance
    //        Debug.Log("Re adding card to physical card pool from the active list");
    //        //CardManager.instance.physicalCardPool.Add(this);
    //    }
    //    //CardManager.instance.AddPhysicalCard(cardInfo.card);
    //}

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
        // adding to the total stat amount, so that items like life steal can utilise it
        totalStatAmount += statUpgradeAmount;

        Debug.Log("Added new stat from chosen card!");
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

    public void ResetTotalStatAmount()
    {
        totalStatAmount = 0;
        statUpgradeAmount = 0;
    }

    public void ResetTotalStatAmountFromInfestedRoomCompletion(Room room)
    {
        totalStatAmount = 0;
        statUpgradeAmount = 0;
    }
}
