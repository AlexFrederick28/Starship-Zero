using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public List<Quest> questList = new List<Quest>();

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

    public void CheckQuestCompletion()
    {

    }

    public void AddQuestToQuestManagerOnStart(Quest[] quest)
    {
        foreach (Quest q in quest)
        {
            instance.questList.Add(q);
        }
    }
}
