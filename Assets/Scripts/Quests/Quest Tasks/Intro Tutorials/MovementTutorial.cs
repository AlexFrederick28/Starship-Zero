using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementTutorial : QuestTaskBase
{
    [SerializeField] private float movementThreshold;

    public override void OnQuestUpdate()
    {
        base.OnQuestUpdate();

        if (GameState.instance.player == null) { return; }
        //if (GameState.instance.player.GetComponent<Rigidbody2D>().linearVelocity.x > 0 || GameState.instance.player.GetComponent<Rigidbody2D>().linearVelocity.y > 0)
        //{
        //    RegisterQuestInteraction();
        //}
        if (GameState.instance.player.GetComponent<Rigidbody2D>().linearVelocity.magnitude > movementThreshold)
        {
            RegisterQuestInteraction();
        }
    }

    public override void RegisterQuestInteraction()
    {
        base.RegisterQuestInteraction();

        gameObject.SetActive(false);
    }
}
