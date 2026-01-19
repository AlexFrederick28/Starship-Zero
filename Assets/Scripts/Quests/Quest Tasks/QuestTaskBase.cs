using System.Linq;
using UnityEngine;

public class QuestTaskBase : MonoBehaviour
{
    public Quest[] quest;

    public virtual void Start()
    {
        if (QuestManager.instance != null)
        {
            QuestManager.instance.AddQuestToQuestManagerOnStart(quest);
        }
    }

    /// <summary>
    /// Completes the array of quests with the same ID as the active quests in the QuestManager
    /// </summary>
    public virtual void RegisterQuestInteraction()
    {
        Debug.Log("Completing active quest");
        foreach (Quest q in quest)
        {
            for (int i = 0; i < QuestManager.instance.activeQuests.Count; i++)
            {
                Debug.Log("Looping through active quests");
                if (QuestManager.instance.activeQuests[i].prerequisite.id == q.prerequisite.id)
                {
                    Debug.Log("Registered player movement");
                    QuestManager.instance.CompleteQuestArray(quest);
                }
            }
        }
    }
}
