using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoster : MonoBehaviour, IPointerClickHandler
{
    public CardBase Card;
    public Weapon chosenWeapon;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            LevelUpManager.instance.chosenCard = Card;
            LevelUpManager.instance.chosenWeapon = chosenWeapon;
            LevelUpManager.OnCardChosen?.Invoke();
        }
    }
}
