using System.Linq;
using UnityEngine;

public class QuestTaskBase : MonoBehaviour
{
    public bool turnObjectOffBeforeActive = false;
    public QuestScriptableObjects[] questInfos;
    private Quest[] quests;

    public virtual void OnEnable()
    {
        QuestManager.OnActivateNewQuest += ActivateQuestObject;
    }

    public virtual void OnDisable()
    {
        QuestManager.OnActivateNewQuest -= ActivateQuestObject;
    }

    public virtual void Start()
    {
        if (QuestManager.instance != null)
        {
            quests = new Quest[questInfos.Length];
            for (int i = 0; i < quests.Length; i++)
            {
                quests[i] = questInfos[i].quest;
            }
            QuestManager.instance.AddQuestToQuestManagerOnStart(quests);
        }
    }

    public virtual void Update()
    {
        // just set to false in the function instead. 
        if (turnObjectOffBeforeActive == true) { return; }
    }

    public virtual void ActivateQuestObject(int id)
    {
        // TODO: cannot activate object as it is inactive :)
        if (turnObjectOffBeforeActive == true)
        {
            //if (QuestManager.instance != null)
            //{
            //    QuestManager.instance.ActiveQuestObject(quests[0], quests[0].prerequisite.id);
            //}

            turnObjectOffBeforeActive = false;
        }
    }

    /// <summary>
    /// Completes the array of quests with the same ID as the active quests in the QuestManager
    /// </summary>
    public virtual void RegisterQuestInteraction()
    {
        Debug.Log("Completing active quest");
        foreach (QuestScriptableObjects q in questInfos)
        {
            for (int i = 0; i < QuestManager.instance.activeQuests.Count; i++)
            {
                Debug.Log("Looping through active quests");
                if (QuestManager.instance.activeQuests[i].prerequisite.id == q.quest.prerequisite.id)
                {
                    Debug.Log("Registered quest as complete");
                    QuestManager.instance.CompleteQuestArray(quests, this);
                }
            }
        }
    }
}
