using NUnit.Framework;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    [SerializeField] public ItemManager itemManager;

    //[SerializeField] public List<> itemList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (itemManager == null)
        {
            itemManager = FindFirstObjectByType<ItemManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        GameState.instance.OnPlayerLevelUp += LevelUpCards; 
    }

    private void OnDisable()
    {
        GameState.instance.OnPlayerLevelUp -= LevelUpCards;
    }

    public void LevelUpCards()
    {
        if (UIManager.instance.levelUpMenuParent.activeSelf == false)
        {
            UIManager.instance.levelUpMenuParent.SetActive(true);
        }

        else
        {
            UIManager.instance.levelUpMenuParent.SetActive(false);
        }
    }

    public void CardSelected()
    {
        GameState.instance.OnPlayerLevelUp?.Invoke();
    }

    // invoke on player level up in function card is selected

}
