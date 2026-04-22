using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;

/// <summary>
/// The primary player movement script for 2D top down
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerBase player;
    private Rigidbody2D rb;
    [SerializeField] private Vector2 moveInput;
    [Space]
    [Header("Audio")]
    [SerializeField] protected float volume;
    [SerializeField] protected AudioClip footstepClip;

    [SerializeField] protected Animator animator;
    [SerializeField] private string currentState;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>(); // View - child has animator

        SetAnimFrontSide(); // front default
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * player.speed;

        MoveAnimations();
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // footstep sound needs to play alongside the player animation to get the correct footstep timing
    }

    public void MoveAnimations()
    {
        // use vector2 move input to set the correct animation (Idle, Up, Down, left, Right)

        // if move input forward = SetFront etc

        if (moveInput.y >= 1)
        {
            SetAnimBackSide();
        }

        else if (moveInput.y <= -1)
        {
            SetAnimFrontSide();
        }

        else if (moveInput.x <= -0.5)
        {
            SetAnimLeftSide();
        }

        else if (moveInput.x >= 0.5f)
        {
            SetAnimRightSide();
        }

        else if (moveInput.x == 0 && moveInput.y == 0)
        {
            SetAnimIdle();
        }


    }

    public void SetAnimIdle()
    {
        // idle stuff

        PlaySetAnimation("Idle");
    }

    public void SetAnimFrontSide()
    {
        //currentState = "FrontSide";

        PlaySetAnimation("FrontSide");
    }

    public void SetAnimBackSide()
    {
        //currentState = "BackSide";

        PlaySetAnimation("BackSide");
    }

    public void SetAnimLeftSide()
    {
        //currentState = "LeftSide";

        PlaySetAnimation("LeftSide");
    }

    public void SetAnimRightSide()
    {
        //currentState = "RightSide";

        PlaySetAnimation("RightSide");
    }

    public void PlaySetAnimation(string stateToPlay)
    {
        if (currentState == stateToPlay)
        {
            return; // already playing
        }

        currentState = stateToPlay;

        Debug.Log("Anim State: " + currentState);
        animator.CrossFadeInFixedTime(currentState, 0, 0); // state, transition time, layer?
    }

}
