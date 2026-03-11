using System.Linq;
using UnityEngine;

public class QuestTaskBase : MonoBehaviour
{
    public bool disableUpdate = false;
    public QuestScriptableObjects[] questInfos;
    [SerializeField] private Quest[] quests;

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
                quests[i] = new Quest(questInfos[i].quest);
            }
            QuestManager.instance.AddQuestToQuestManagerOnStart(quests);
        }
    }

    public virtual void Update()
    {
        // just set to false in the function instead. 
        if (disableUpdate == true) { return; }
        OnQuestUpdate();
    }

    public virtual void OnQuestUpdate()
    {

    }

    public virtual void ActivateQuestObject(int id)
    {
        //Debug.Log("ID = " + id);
        //Debug.Log("Quests 0 ID =  " + quests[0].prerequisite.id);
        if (id != quests[0].prerequisite.id) { return; }
        if (disableUpdate == true)
        {
            disableUpdate = false;
            //Debug.Log("Enabled update");
        }
    }

    /// <summary>
    /// Completes the array of quests with the same ID as the active quests in the QuestManager
    /// </summary>
    public virtual void RegisterQuestInteraction()
    {
        Debug.Log("Completing active quest");
        foreach (Quest q in quests)
        {
            for (int i = 0; i < QuestManager.instance.activeQuests.Count; i++)
            {
                Debug.Log("Looping through active quests");
                if (QuestManager.instance.activeQuests[i].prerequisite.id == q.prerequisite.id)
                {
                    Debug.Log("Registered quest as complete");
                    QuestManager.instance.CompleteQuestArray(quests, this);
                }
            }
        }
    }
}
