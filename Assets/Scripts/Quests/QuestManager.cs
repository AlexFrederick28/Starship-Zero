using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public List<Quest> questList = new List<Quest>();
    public List<Quest> activeQuests = new List<Quest>();

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

    public void CheckQuestCompletion(Dialogue currentDialogue)
    {
        for (int i = 0; i < instance.activeQuests.Count; i++)
        {
            if (instance.activeQuests[i].prerequisite.id == currentDialogue.quest.prerequisite.id && instance.activeQuests[i].prerequisite.complete == true)
            {
                currentDialogue.completedPrerequisite = true;
                Debug.Log("Quest Complete");
            }
        }
    }

    public void CompleteQuestArray(Quest[] quest)
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
                }
                else
                {
                    Debug.Log("ID does not match");
                }
            }
        }
    }

    public void AddQuestToQuestManagerOnStart(Quest[] quest)
    {
        foreach (Quest q in quest)
        {
            instance.questList.Add(q);
        }
    }
}
