using System.Linq;
using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour, IPointerClickHandler
{
    public CardManager.CardInfo newInfo = new CardManager.CardInfo();

    public Toggle cardActiveToggle;
    public Image highlightImage;
    public Color originalColour;
    public Color highlightedColour;
    public Image cardIconImage;
    public Image cardLockImage;
    public TextMeshProUGUI cardName;

    public CardScriptableObject card;

    public int unlockCost;
    public bool isUnlocked = false;
    public bool isActive = false;

    private void OnEnable()
    {
        CardUnlockUpgradeMenu.OnViewingCard += ViewCardSlot;
    }

    private void OnDisable()
    {
        CardUnlockUpgradeMenu.OnViewingCard += ViewCardSlot;

        // unview the slot on closing UI
        DeselectSlot();
    }

    public void ViewCardSlot(CardSlot slot)
    {
        if (slot == this)
        {
            if (CardUnlockUpgradeMenu.instance.selectedCardSlot != this)
            {
                // if the selected slot is this, highlight it
                // setting the viewed slot visually on the bottom card description UI. 
                // letting the card upgrade menu know what slot has been selected
                Debug.Log("Selected Slot");
                SelectSlot();
                highlightImage.color = highlightedColour;
                UIManager.instance.cardDescriptionImage.sprite = card.cardSprite;
                UIManager.instance.cardDescriptionNameText.text = card.cardName;
                UIManager.instance.cardDescriptionText.text = card.cardDescription;
                if (slot.card.defaultCard == true) { UIManager.instance.cardUnlockButton.gameObject.SetActive(false); return; }
                if (isUnlocked == false)
                {
                    UIManager.instance.cardUnlockButton.gameObject.SetActive(true);
                    UIManager.instance.cardUnlockButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unlock" + "\n" + "$" + unlockCost;
                    UIManager.instance.cardUnlockButton.onClick.RemoveAllListeners();
                    UIManager.instance.cardUnlockButton.onClick.AddListener(UnlockCard);
                }
                else
                {
                    //UIManager.instance.cardUnlockButton.gameObject.SetActive(false);
                    UIManager.instance.cardUnlockButton.onClick.RemoveAllListeners();
                }
            }
            else
            {
                // if the selected slot is this, deselect it
                Debug.Log("Deselected Slot");
                DeselectSlot();
                return;
            }
        }
        else
        {
            highlightImage.color = originalColour;
        }
    }

    public void DeselectSlot()
    {
        highlightImage.color = originalColour;
        UIManager.instance.cardUnlockButton.gameObject.SetActive(false);
        UIManager.instance.cardDescriptionImage.gameObject.SetActive(false);
        UIManager.instance.cardDescriptionNameText.gameObject.SetActive(false);
        UIManager.instance.cardDescriptionText.gameObject.SetActive(false);
        CardUnlockUpgradeMenu.instance.selectedCardSlot = null;
    }

    public void SelectSlot()
    {
        highlightImage.color = highlightedColour;
        UIManager.instance.cardDescriptionImage.gameObject.SetActive(true);
        UIManager.instance.cardDescriptionNameText.gameObject.SetActive(true);
        UIManager.instance.cardDescriptionText.gameObject.SetActive(true);
        CardUnlockUpgradeMenu.instance.selectedCardSlot = this;
    }

    public void AddOrRemoveCardToSelectedCards(bool isOn)
    {
        // using lambda expression to find the reference to the cardBase, allowing the activation of the card 
        //GameObject physicalCard = CardManager.instance.physicalCardPrefabs.First(b => b.GetComponent<CardBase>().cardInfo.card == card);

        if (isOn == true)
        {
            newInfo.card = card;

            //CardManager.instance.EnableCardInPool(newBase);
            CardManager.instance.AddPhysicalCard(card);
            CardManager.instance.PopulateSelectedCardInfo(newInfo);
            CardManager.instance.allSelectedCardsList.Add(newInfo);
            Debug.Log("Added selected card");
        }
        else
        {
            //CardManager.instance.DisableCardInPool(newBase);
            CardManager.instance.RemovePhysicalCard(card);
            CardManager.instance.allSelectedCardsList.Remove(newInfo);
            Debug.Log("Removed selected card");
        }
    }

    public void UnlockCard()
    {
        if (GameState.instance.player.currency >= unlockCost)
        {
            isUnlocked = true;
            cardLockImage.enabled = false;
            cardActiveToggle.onValueChanged.AddListener(AddOrRemoveCardToSelectedCards);
            UIManager.instance.cardUnlockButton.gameObject.SetActive(false);
            cardActiveToggle.gameObject.SetActive(true);
            GameState.instance.player.currency -= unlockCost;
        }
        // make the card go red or smth if the player doesnt have the money
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // show card details in the description, and highlight this selected card
            CardUnlockUpgradeMenu.OnViewingCard?.Invoke(this);
        }
    }
}
