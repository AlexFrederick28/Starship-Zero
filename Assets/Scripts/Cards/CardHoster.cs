using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoster : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
{
    public CardBase Card;
    public Weapon chosenWeapon;

    public GameObject particleOne;
    public GameObject particleTwo;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SoundManager.instance.PlaySoundClip(SoundManager.instance.soundEffectsArray[5], transform, SoundManager.instance.SoundVolume(), true, true, 1f, 1f);

            LevelUpManager.instance.chosenCard = Card;
            LevelUpManager.instance.chosenWeapon = chosenWeapon;
            LevelUpManager.OnCardChosen?.Invoke();

            particleOne.SetActive(false);
            particleTwo.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        particleOne.SetActive(true);
        particleTwo.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        particleOne.SetActive(false);
        particleTwo.SetActive(false);
    }
}
