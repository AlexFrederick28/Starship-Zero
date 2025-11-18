using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementTutorial : MonoBehaviour
{
    public Quest[] quest;
    public bool registeredMovement = false;
    public PlayerBase player;

    private void Start()
    {
        if (QuestManager.instance != null)
        {
            QuestManager.instance.AddQuestToQuestManagerOnStart(quest);
        }
    }

    private void FixedUpdate()
    {
        if (registeredMovement == false)
        {
            if (player == null)
            {
                player = FindAnyObjectByType<PlayerBase>();
            }

            // need a better way/place for the quests to check when they're active rather than spamming update
            if (player.GetComponent<Rigidbody2D>().linearVelocity.x > 0 || player.GetComponent<Rigidbody2D>().linearVelocity.y > 0)
            {
                RegisterPlayerMovement();
            }
        }
    }

    public void RegisterPlayerMovement()
    {
        if (registeredMovement == false)
        {
            foreach (Quest q in quest)
            {
                for (int i = 0; i < QuestManager.instance.activeQuests.Count; i++)
                {
                    if (QuestManager.instance.activeQuests[i].prerequisite.id == q.prerequisite.id)
                    {
                        Debug.Log("Registered player movement");
                        QuestManager.instance.CompleteQuestArray(quest);

                        registeredMovement = true;
                    }
                }
            }
        }
    }
}
