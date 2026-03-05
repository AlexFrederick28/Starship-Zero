using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour
{
    public Image cardIconImage;
    public TextMeshProUGUI cardName;

    public ItemBase cardItem;

    public int unlockCost;
    public bool isUnlocked = false;
}
