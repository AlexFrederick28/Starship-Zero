using UnityEngine;
using System.Collections;
using System;

public class NPCBase : MonoBehaviour, IInteractable, IDialogue
{
    public NPCScriptableObject NPC;
    [SerializeField] protected string nameNPC;
    [SerializeField] protected NPCScriptableObject.NPCType type;
    [SerializeField] private Dialogue[] newDialogue = new Dialogue[0];
    [SerializeField] private Dialogue currentDialogue;

    protected virtual void Start()
    {
        if (NPC != null)
        {
            CreateNPC();
        }
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
        throw new NotImplementedException();
    }

    public void StartDialogue()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.name = nameNPC;

            SetDialogueActive();
            //UIManager.instance.dialogueText
        }
    }

    public void EndDialogue()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.dialogueParent.SetActive(false);
        }
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
