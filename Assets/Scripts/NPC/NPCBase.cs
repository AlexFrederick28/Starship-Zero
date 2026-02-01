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
    public Dialogue currentDialogue { get; private set; }
    [SerializeField] private int textIndex = 0;
    [SerializeField] private int dialogueIndex;
    [SerializeField] private float textSpeed;

    [Space]
    [Header("Audio")]
    [SerializeField] protected float volume;
    [SerializeField] protected float minPitch;
    [SerializeField] protected float maxPitch;
    [SerializeField] protected AudioClip typingClip;

    private bool startedDialogue = false;

    /// <summary>
    /// To finish a dialogue:
    /// If isQuest - the player must complete the quest and finish reading all the dialogue
    /// If not a quest - the player must read all the dialogue (exhaust all text)
    /// </summary>

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
                CompleteQuest();
            }
            else
            {
                CompleteTopic();
            }
            ClearText();
            StartCoroutine(WriteLine_C());

            startedDialogue = true;
        }
    }

    public void NextLine()
    {
        if (currentDialogue.completedTopic == false && currentDialogue.completedPrerequisite == false || currentDialogue.completedTopic == true && dialogueIndex < newDialogue.Length - 1)
        {
            // repeats the same topic if not completed, as well as adds any quest that hasnt already been made active
            if (textIndex < currentDialogue.dialogueText.Length - 1)
            {
                textIndex++;
                StopAllCoroutines();
                ClearText();
                StartCoroutine(WriteLine_C());
                ActivateQuest();
            }
            else
            {
                CompleteQuest();
                CompleteTopic();
                textIndex = 0;
                StopAllCoroutines();
                ClearText();
                StartCoroutine(WriteLine_C());
            }
        }
    }

    public void CompleteTopic()
    {
        if (currentDialogue.isQuest == false && textIndex == currentDialogue.dialogueText.Length - 1)
        {
            if (dialogueIndex !< newDialogue.Length - 1)
            {
                // if the current topic is not a quest, and the dialogue length has been reached - set to true and continue
                currentDialogue.completedTopic = true;
                GoNextDialogue();
            }
        }
        else if (currentDialogue.isQuest == true && currentDialogue.quest.prerequisite.complete == true)
        {
            currentDialogue.completedPrerequisite = true;
            currentDialogue.completedTopic = true;
            GoNextDialogue();
        }
    }

    public void GoNextDialogue()
    {
        // if possible, go to the next dialogue prompt
        if (dialogueIndex < newDialogue.Length - 1)
        {
            StopAllCoroutines();
            ClearText();
            dialogueIndex++;
            currentDialogue = newDialogue[dialogueIndex];
            textIndex = 0;
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
            if (!QuestManager.instance.activeQuests.Contains(currentDialogue.quest))
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
                    }
                }
            }
        }
    }

    public void CompleteQuest()
    {
        if (currentDialogue.isQuest == true && currentDialogue.completedPrerequisite == false)
        {
            QuestManager.instance.CheckQuestCompletion(currentDialogue);
        }
        if (currentDialogue.completedPrerequisite == true)
        {
            if (currentDialogue.quest.prerequisite.currencyReward > 0)
            {
                GameState.instance.player.AddCurrencyFromNPC(this);
            }
            CompleteTopic();
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
        foreach (char c in currentDialogue.dialogueText[textIndex])
        {
            UIManager.instance.dialogueText.text += c;
            SoundManager.instance.PlaySoundClip(typingClip, transform, volume, true, true, minPitch, maxPitch);
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
}
