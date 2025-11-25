using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Player")]
    public Slider playerHealthSlider;

    [Header("Infested Room")]
    public TextMeshProUGUI currentTime;
    public TextMeshProUGUI currentDifficulty;

    [Header("Dialogue")]
    [Space]
    public GameObject canvas;
    public GameObject dialogueParent;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject continueButton;

    [Header("Quests")]
    [Space]
    public GameObject questParent;
    public GameObject questPrefab;

    public static UIManager instance;
    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Start()
    {
        if (dialogueParent == null)
        {
            GameObject parent = Instantiate(dialogueParent);
            parent.transform.SetParent(canvas.transform);
        }
        if (questParent == null)
        {
            GameObject parent = Instantiate(questParent);
            parent.transform.SetParent(canvas.transform);
        }
    }

    private void Update()
    {
        GetUINumbersTEMP();
    }
    public void GetUINumbersTEMP()
    {
        // this is a temporary function to get the infested room and player health bar working for prototype testing
        int newTime = (int)Spawning.instance.CurrentTime;
        int newDifficulty = (int)Spawning.instance.CurrentDifficulty;


        currentTime.text = newTime.ToString();
        currentDifficulty.text = newDifficulty.ToString();
    }
}
