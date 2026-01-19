using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementTutorial : QuestTaskBase
{
    public bool registeredMovement = false;

    private void FixedUpdate()
    {
        if (registeredMovement == false)
        {
            if (GameState.instance.player == null) { return; }
            if (GameState.instance.player.GetComponent<Rigidbody2D>().linearVelocity.x > 0 || GameState.instance.player.GetComponent<Rigidbody2D>().linearVelocity.y > 0)
            {
                foreach (Quest q in quest)
                {
                    for (int i = 0; i < QuestManager.instance.activeQuests.Count; i++)
                    {
                        if (QuestManager.instance.activeQuests[i].prerequisite.id == q.prerequisite.id)
                        {
                            RegisterQuestInteraction();
                        }
                    }
                }
            }
        }
    }

    public override void RegisterQuestInteraction()
    {
        base.RegisterQuestInteraction();

        registeredMovement = true;
        gameObject.SetActive(false);
    }
}
