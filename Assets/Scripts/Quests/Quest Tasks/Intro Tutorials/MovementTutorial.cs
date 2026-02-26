using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementTutorial : QuestTaskBase
{
    public bool registeredMovement = false;

    public override void Update()
    {
        base.Update();

        if (GameState.instance.player == null) { return; }
        if (GameState.instance.player.GetComponent<Rigidbody2D>().linearVelocity.x > 0 || GameState.instance.player.GetComponent<Rigidbody2D>().linearVelocity.y > 0)
        {
            RegisterQuestInteraction();
        }
    }

    public override void RegisterQuestInteraction()
    {
        base.RegisterQuestInteraction();
    }
}
