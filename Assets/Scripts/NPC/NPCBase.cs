using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using Unity.VisualScripting;
using System.Net;
using JetBrains.Annotations;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class NPCBase : MonoBehaviour, IInteractable, IDialogue
{
    public NPCScriptableObject NPC;
    [SerializeField] protected string nameNPC;
    [SerializeField] protected NPCScriptableObject.NPCType type;
    [SerializeField] private Dialogue[] newDialogue = new Dialogue[0];
    public Dialogue currentDialogue { get; set; }
    [SerializeField] private int textIndex = 0;
    [SerializeField] private int dialogueIndex;
    [SerializeField] private float textSpeed;
    private bool givenCurrentQuestReward = false;

    [Space]
    [Header("Audio")]
    [SerializeField] protected bool useVoiceLines = true;
    [SerializeField] protected float volume;
    [SerializeField] protected float minPitch;
    [SerializeField] protected float maxPitch;
    [SerializeField] protected AudioClip typingClip;

    private bool startedDialogue = false;

    public static Action OnHandedInQuest;

    /// <summary>
    /// To finish a dialogue:
    /// If isQuest - the player must complete the quest and finish reading all the dialogue
    /// If not a quest - the player must read all the dialogue (exhaust all text)
    /// </summary>

    private void OnEnable()
    {
        QuestManager.OnQuestCompletion += CompletePrerequisite;
    }

    private void OnDisable()
    {
        QuestManager.OnQuestCompletion -= CompletePrerequisite;
    }

    protected virtual void Start()
    {
        if (NPC != null)
        {
            CreateNPC();
        }

        currentDialogue = newDialogue[0];
    }

    protected void CreateNPC()
    {
        nameNPC = NPC.name;
        type = NPC.typeNPC;
    }

    public void OnInteract()
    {
        StartDialogue();
    }

    public void OnEndInteraction()
    {
        if (UIManager.instance != null)
        {
            if (UIManager.instance.continueButton != null)
            {
                UIManager.instance.continueButton.GetComponent<Button>().onClick.RemoveListener(NextLine);
            }
        }

        StopAllCoroutines();
        ClearText();
        SetDialogueInActive();
        textIndex = 0;
        startedDialogue = false;
    }

    public void StartDialogue()
    {
        if (startedDialogue == true)
        {
            NextLine();
        }
        if (UIManager.instance != null && startedDialogue == false)
        {
            UIManager.instance.nameText.text = nameNPC;
            UIManager.instance.continueButton.GetComponent<Button>().onClick.AddListener(NextLine);
            Debug.Log("Started Dialogue");

            textIndex = 0;
            SetDialogueActive();
            if (currentDialogue.isQuest == true)
            {
                // check to see if an active quest is complete, if so, notify the NPC its complete.
                CompleteQuestOnCurrentDialogue();
            }

            GoNextDialogue();
            ClearText();
            StartCoroutine(WriteLine_C());

            startedDialogue = true;
        }
    }

    public void NextLine()
    {
        if (currentDialogue.completedTopic == false || currentDialogue.completedTopic == true && currentDialogue.isQuest == true && currentDialogue.completedPrerequisite == false || currentDialogue.completedTopic == true && dialogueIndex < newDialogue.Length - 1) //&& currentDialogue.completedPrerequisite == false || currentDialogue.completedTopic == true && dialogueIndex < newDialogue.Length - 1)
        {
            // repeats the same topic if not completed, as well as adds any quest that hasnt already been made active
            if (textIndex < currentDialogue.dialogueText.Length - 1)
            {
                textIndex++;
                StopAllCoroutines();
                ClearText();
                StartCoroutine(WriteLine_C());
                ActivateQuest();
                if (textIndex == currentDialogue.dialogueText.Length - 1)
                {
                    Debug.Log("Completed topic and quest on correct line");
                    CompleteTopic();
                    CompleteQuestOnCurrentDialogue();
                }
            }
            else if (textIndex == currentDialogue.dialogueText.Length - 1)
            {
                CompleteQuestOnCurrentDialogue();
                GoNextDialogue();
                textIndex = 0;
                StopAllCoroutines();
                ClearText();
                StartCoroutine(WriteLine_C());
            }
        }
    }

    public void CompleteTopic()
    {
        currentDialogue.completedTopic = true;
    }

    public void GoNextDialogue()
    {
        // if possible, go to the next dialogue prompt
        if (currentDialogue.completedTopic == false) { return; }
        if (currentDialogue.isQuest == true && currentDialogue.completedPrerequisite == false) { return; }
        if (dialogueIndex < newDialogue.Length - 1)
        {
            StopAllCoroutines();
            ClearText();
            dialogueIndex++;
            currentDialogue = newDialogue[dialogueIndex];
            textIndex = 0;
            givenCurrentQuestReward = false;
        }

        if (currentDialogue.dialogueText.Length == 1 && currentDialogue.isQuest == true)
        {
            // if there is only one entry for the current dialogue topic, start the quest if there is one
            ActivateQuest();
        }
    }

    public void ActivateQuest()
    {
        // to active a quest it must already be in the QuestManagers quest list

        if (currentDialogue.isQuest == true && currentDialogue.completedPrerequisite == false)
        {
            Quest newQuest = new Quest(currentDialogue.questInfo.quest);
            currentDialogue.quest = newQuest;

            // using LINQ method - Any() (returns a true or false if an element satisfies a condition)
            // using lambda expression - q => (means for each element) essentially going through a foreach loop checking if the id matches
            if (!QuestManager.instance.activeQuests.Any(q => q.prerequisite.id == newQuest.prerequisite.id))
            {
                for (int i = 0; i < QuestManager.instance.questList.Count; i++)
                {
                    if (QuestManager.instance.questList[i].prerequisite.id == currentDialogue.quest.prerequisite.id)
                    {
                        Debug.Log("Activated quest");
                        currentDialogue.quest.prerequisite.level = GameState.instance.player.Level; // as the quest is activated, so is the quest level which is used in circumstances such as entering an infested room
                        GameObject newQuestInstance = Instantiate(UIManager.instance.questPrefab);
                        newQuestInstance.transform.SetParent(UIManager.instance.questParent.transform);
                        newQuestInstance.GetComponentInChildren<TextMeshProUGUI>().text = currentDialogue.quest.description;
                        newQuestInstance.GetComponent<QuestUIParent>().questID = currentDialogue.quest.prerequisite.id;
                        QuestManager.instance.activeQuests.Add(currentDialogue.quest);
                        QuestManager.instance.questUIList.Add(newQuestInstance.GetComponent<QuestUIParent>());

                        // quest objects get notified when a quest is activated
                        QuestManager.OnActivateNewQuest?.Invoke(currentDialogue.quest.prerequisite.id); 
                    }
                }
            }
        }
    }

    public void CompletePrerequisite()
    {
        currentDialogue.completedPrerequisite = true;
    }

    public void CompleteQuestOnCurrentDialogue()
    {
        if (currentDialogue.completedTopic == false) { return; }

        QuestManager.instance.CheckQuestCompletion(currentDialogue);
        if (currentDialogue.completedPrerequisite == true && givenCurrentQuestReward == false)
        {
            // give the player the currency reward if we are on the last piece of text (prevents the reward from being given twice)
            if (currentDialogue.quest.prerequisite.currencyReward > 0)
            {
                GameState.instance.player.AddCurrencyFromNPC(this);
                givenCurrentQuestReward = true;
            }
        }
    }

    public void EndDialogue()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.dialogueParent.SetActive(false);
        }
    }

    public IEnumerator WriteLine_C()
    {
        if (currentDialogue.dialogueClip != null && currentDialogue.dialogueClip.Length > 0 && currentDialogue.dialogueClip[textIndex] != null && useVoiceLines == true)
        {
            SoundManager.instance.PlayDialogueSoundClip(currentDialogue.dialogueClip[textIndex], transform, volume, true, false, 0f, 0f);
        }
        foreach (char c in currentDialogue.dialogueText[textIndex])
        {
            UIManager.instance.dialogueText.text += c;
            if (useVoiceLines == false)
            {
                SoundManager.instance.PlaySoundClip(typingClip, transform, volume, true, true, minPitch, maxPitch);
            }
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void ClearText()
    {
        UIManager.instance.dialogueText.text = string.Empty;
    }

    public void SetDialogueActive()
    {
        UIManager.instance.dialogueParent.SetActive(true);
    }
    public void SetDialogueInActive()
    {
        UIManager.instance.dialogueParent.SetActive(false);
    }

    public void DisableInteractionComponent()
    {
        return;
    }

    public void EnableInteractionComponent()
    {
        return;
    }
}
