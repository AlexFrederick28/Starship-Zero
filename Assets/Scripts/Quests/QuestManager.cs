using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestManager : MonoBehaviour
{
    public List<Quest> questList = new List<Quest>(); // not used yet
    public List<Quest> activeQuests = new List<Quest>();
    public List<QuestUIParent> questUIList = new List<QuestUIParent>();
    public Sprite completeQuestSprite;

    public static Action<int> OnActivateNewQuest;
    public static Action OnQuestCompletion;

    public static QuestManager instance;

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // turns off duplicate instances if there are more than one enabled
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void EnableQuestObject(int questID, IInteractable interactable)
    {
        if (activeQuests.Any(q => q.prerequisite.id == questID))
        {
            interactable.EnableInteractionComponent();
        }
    }

    public void CheckQuestCompletion(Dialogue currentDialogue)
    {
        for (int i = 0; i < instance.activeQuests.Count; i++)
        {
            if (instance.activeQuests[i].prerequisite.id == currentDialogue.quest.prerequisite.id && instance.activeQuests[i].prerequisite.complete == true)
            {
                NPCBase.OnHandedInQuest?.Invoke();
                currentDialogue.completedPrerequisite = true;
                instance.activeQuests.RemoveAt(i);
                Destroy(instance.questUIList[i].gameObject);
                instance.questUIList.RemoveAt(i);
                Debug.Log("Quest Complete and rewards claimed!");
            }
        }
    }

    public void CompleteQuestArray(Quest[] quest, QuestTaskBase taskBase)
    {
        foreach (Quest q in quest)
        {
            q.prerequisite.complete = true;

            for (int i = 0; i < instance.activeQuests.Count; i++)
            {
                if (instance.activeQuests[i].prerequisite.id == q.prerequisite.id)
                {
                    Debug.Log("Complete ACTIVE quest");
                    instance.activeQuests[i].prerequisite.complete = true;
                    instance.questUIList[i].image.sprite = completeQuestSprite;
                    OnQuestCompletion?.Invoke();
                }
                else
                {
                    Debug.Log("ID does not match");
                }
            }
        }

        taskBase.enabled = false;
    }

    public void AddQuestToQuestManagerOnStart(Quest[] quest)
    {
        foreach (Quest q in quest)
        {
            Debug.Log("Added quest to QuestManager " + q.prerequisite.name);
            instance.questList.Add(q);
        }
    }
}
