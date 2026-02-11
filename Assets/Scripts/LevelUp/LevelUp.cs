using UnityEngine;

public class LevelUp : MonoBehaviour
{
    [SerializeField] public ItemManager itemManager;

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

    public void OnLevelUp() // when the player levels up
    {
        if (itemManager == null)
        {
            itemManager = FindFirstObjectByType<ItemManager>();
        }

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



    // panel & cards reference in UI Manager

}
