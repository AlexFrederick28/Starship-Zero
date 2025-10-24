using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// The primary player movement script for 2D top down
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb;
    [SerializeField] private Vector2 moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.linearVelocity = moveInput * moveSpeed;
        MoveAnimations();
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void MoveAnimations()
    {
        // use vector2 move input to set the correct animation (Idle, Up, Down, left, Right)
    }
}
