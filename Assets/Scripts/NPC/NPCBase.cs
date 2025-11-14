using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.UI;

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
        UIManager.instance.continueButton.GetComponent<Button>().onClick.RemoveListener(NextDialogue);
        SetDialogueInActive();
    }

    public void StartDialogue()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.name = nameNPC;
            UIManager.instance.continueButton.GetComponent<Button>().onClick.AddListener(NextDialogue);

            SetDialogueActive();

            ClearText();
            StartCoroutine(WriteLine_C());
        }
    }

    public void NextDialogue()
    {
        if (currentDialogue.completedTopic == false && currentDialogue.completedPrerequisite == false)
        {
            // repeats the same topic if not completed
            if (textIndex < currentDialogue.dialogueText.Length - 1)
            {
                textIndex++;
                ClearText();
                StartCoroutine(WriteLine_C());
            }
            else
            {
                CompleteTopic();
                textIndex = 0;
                ClearText();
                StartCoroutine(WriteLine_C());
            }
        }
    }

    public void CompleteTopic()
    {
        if (currentDialogue.isQuest == false && textIndex == currentDialogue.dialogueText.Length - 1)
        {
            // if the current topic is not a quest, and the dialogue length has been reached - set to true and continue
            currentDialogue.completedTopic = true;
            currentDialogue.completedPrerequisite = true;

            GoNextDialogue();
        }
    }

    public void GoNextDialogue()
    {
        if (dialogueIndex < newDialogue.Length - 1)
        {
            dialogueIndex++;
            currentDialogue = newDialogue[dialogueIndex];
            textIndex = 0;
        }
        else
        {
            ClearText();
            EndDialogue();
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
