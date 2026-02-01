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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        MoveAnimations();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * player.speed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        // footstep sound needs to play alongside the player animation to get the correct footstep timing
    }

    public void MoveAnimations()
    {
        // use vector2 move input to set the correct animation (Idle, Up, Down, left, Right)

    }
}
