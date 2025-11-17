using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PowerGridSwitch : MonoBehaviour
{
    public Quest[] quest;

    private void Start()
    {
        if (QuestManager.instance != null)
        {
            QuestManager.instance.AddQuestToQuestManagerOnStart(quest);
        }
    }
}
