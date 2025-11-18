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
    [SerializeField] private Dialogue currentDialogue;
    [SerializeField] private int textIndex = 0;
    [SerializeField] private int dialogueIndex;
    [SerializeField] private float textSpeed;

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

        SetDialogueInActive();
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

            SetDialogueActive();

            ClearText();
            StartCoroutine(WriteLine_C());

            startedDialogue = true;
        }
    }

    public void NextLine()
    {
        StopAllCoroutines();
        ClearText();

        if (currentDialogue.completedTopic == false && currentDialogue.completedPrerequisite == false)
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
                CompleteTopic();
                textIndex = 0;
                ClearText();
                StartCoroutine(WriteLine_C());
            }
        }
        //else
        //{
        //    if (textIndex < currentDialogue.dialogueText.Length - 1)
        //    {
        //        textIndex++;
        //        StopAllCoroutines();
        //        ClearText();
        //        StartCoroutine(WriteLine_C());
        //        ActivateQuest();
        //    }
        //    else
        //    {
        //        CompleteTopic();
        //        textIndex = 0;
        //        ClearText();
        //        StartCoroutine(WriteLine_C());
        //    }
        //}
    }

    public void CompleteTopic()
    {
        if (currentDialogue.isQuest == false && textIndex == currentDialogue.dialogueText.Length - 1 || currentDialogue.isQuest == true && textIndex == currentDialogue.dialogueText.Length - 1 && currentDialogue.quest.prerequisite.complete == true)
        {
            // if the current topic is not a quest, and the dialogue length has been reached - set to true and continue
            currentDialogue.completedTopic = true;
            currentDialogue.completedPrerequisite = true;

            GoNextDialogue();
        }
    }

    public void GoNextDialogue()
    {
        // if possible, go to the next dialogue prompt. Otherwise end interaction
        if (dialogueIndex < newDialogue.Length - 1)
        {
            StopAllCoroutines();
            ClearText();
            dialogueIndex++;
            currentDialogue = newDialogue[dialogueIndex];
            textIndex = 0;
        }
        //else
        //{
        //    Debug.Log("Ended Dialogue Interaction");
        //    StopAllCoroutines();
        //    ClearText();
        //    EndDialogue();
        //}

        CompleteQuest();
    }

    public void ActivateQuest()
    {
        if (currentDialogue.isQuest == true && currentDialogue.completedPrerequisite == false)
        {
            if (!QuestManager.instance.activeQuests.Contains(currentDialogue.quest))
            {
                for (int i = 0; i < QuestManager.instance.questList.Count; i++)
                {
                    if (QuestManager.instance.questList[i].prerequisite.id == currentDialogue.quest.prerequisite.id)
                    {
                        Debug.Log("Activated quest");
                        GameObject newQuestInstance = Instantiate(UIManager.instance.questPrefab);
                        newQuestInstance.transform.SetParent(UIManager.instance.questParent.transform);
                        newQuestInstance.GetComponentInChildren<TextMeshProUGUI>().text = currentDialogue.quest.description;
                        QuestManager.instance.activeQuests.Add(currentDialogue.quest);
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
