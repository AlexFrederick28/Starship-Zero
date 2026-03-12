using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeBase : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buyCostText;
    [SerializeField] private TextMeshProUGUI refundCostText;
    [SerializeField] private TextMeshProUGUI upgradeLevelText;

    public int upgradeLevel = 0;

    public Vector2 statIncreaseAndCostScaling;

    public Vector2 currentStatIncreaseAndCost;
    public List<Vector2> previousStatIncreaseAndCost;

    protected bool canUpgrade = false;

    private void OnEnable()
    {
        SetText();
    }

    public void SetText()
    {
        buyCostText.text = "-$" + currentStatIncreaseAndCost.y.ToString();
        upgradeLevelText.text = upgradeLevel.ToString();
        if (previousStatIncreaseAndCost.Count < 1) { refundCostText.text = "+$0"; return; }
        refundCostText.text = "+$" + previousStatIncreaseAndCost[previousStatIncreaseAndCost.Count-1].y.ToString();
    }

    public virtual void UpgradeCard()
    {
        // track the current amount to allow for a refund
        previousStatIncreaseAndCost.Add(currentStatIncreaseAndCost);
        // take the currency (the increase will be added via overriding this function)
        GameState.instance.player.currency -= (int)currentStatIncreaseAndCost.y;
        // scale up the stat and cost
        currentStatIncreaseAndCost.y += statIncreaseAndCostScaling.y;
        // visually show the player has upgraded
        upgradeLevel++;

        SetText();
    }

    public virtual void RefundCard()
    {
        // give back the currency (stat will be taken away from via overriding this function)
        GameState.instance.player.currency += (int)previousStatIncreaseAndCost[previousStatIncreaseAndCost.Count-1].y;
        // scale down the stat and cost
        currentStatIncreaseAndCost.y = previousStatIncreaseAndCost[previousStatIncreaseAndCost.Count-1].y;
        // once refunded, remove the latest recorded increase
        previousStatIncreaseAndCost.RemoveAt(previousStatIncreaseAndCost.Count-1);
        // visually show the player has refunded
        upgradeLevel--;

        SetText();
    }
}
