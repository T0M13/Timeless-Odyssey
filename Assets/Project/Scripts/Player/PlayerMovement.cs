using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public enum MovementState
    {
        Default,
        Running,
        Crouching
    }

    [Header("References")]
    [SerializeField] private PlayerReferences playerReferences;

    [Header("Movement Settings")]
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float defaultSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private Vector2 movement;
    [SerializeField] private bool canMove = true;

    [Header("Movement State")]
    [SerializeField] private MovementState moveState = MovementState.Default;

    [Header("Jump Settings")]
    [SerializeField] private bool canJump = true;
    [SerializeField][ShowOnly] private bool hasJumped = false;
    [SerializeField] private float jumpThreshold = .1f;
    [SerializeField] private float jumpForce = 5f; 

    [Header("Fall Settings")]
    [SerializeField][ShowOnly] private bool isFalling = false;
    [SerializeField] private float fallThreshold = -2f;

    [Header("Other Settings")]
    [SerializeField][ShowOnly] private float verticalVelocity;

    [Header("Gizmo Settings")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private float gizmoLineLength = 2f;
    [SerializeField] private float gizmoSphereRadius = 0.1f;

    public bool HasJumped { get => hasJumped; set => hasJumped = value; }

    private void Awake()
    {
        GetReferences();
    }

    private void OnValidate()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        if (playerReferences == null)
        {
            playerReferences = GetComponent<PlayerReferences>();
            if (playerReferences == null)
            {
                Debug.LogWarning("PlayerReferences component is missing from PlayerMovement");
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
        CheckJumpAndFallThresholds();
    }

    private void Move()
    {
        if (!canMove) return;
        MoveLogic(playerReferences.PlayerBody, movement);
    }

    private void MoveLogic(Rigidbody rb, Vector2 movement)
    {
        Vector3 velocity;

        switch (moveState)
        {
            case MovementState.Default:
                velocity = new Vector3(movement.x * defaultSpeed, rb.velocity.y, movement.y * defaultSpeed);
                break;
            case MovementState.Running:
                velocity = new Vector3(movement.x * runSpeed, rb.velocity.y, movement.y * runSpeed);
                break;
            case MovementState.Crouching:
                velocity = new Vector3(movement.x * crouchSpeed, rb.velocity.y, movement.y * crouchSpeed);
                break;
            default:
                velocity = new Vector3(movement.x * defaultSpeed, rb.velocity.y, movement.y * defaultSpeed);
                break;
        }

        rb.velocity = rb.transform.TransformDirection(velocity);
    }

    private void Jump()
    {
        if (!canJump || HasJumped || moveState == MovementState.Crouching || !playerReferences.IsGrounded) return;

        if (moveState == MovementState.Default || moveState == MovementState.Running)
        {
            playerReferences.PlayerBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            HasJumped = true;
        }
    }


    private void CheckJumpAndFallThresholds()
    {
        verticalVelocity = playerReferences.PlayerBody.velocity.y;

        if (verticalVelocity > jumpThreshold)
        {
            HasJumped = true;
            isFalling = false;
        }
        else if (verticalVelocity < fallThreshold && !playerReferences.IsGrounded)
        {
            HasJumped = true;
            isFalling = true;
        }
        else
        {
            HasJumped = false;
            isFalling = false;
        }
    }

    private void OnMovementStateChanged()
    {
        switch (moveState)
        {
            case MovementState.Default:
                playerReferences.SetDefaultCollider();
                playerReferences.SetDefaultCameraPosition();
                break;
            case MovementState.Running:
                playerReferences.SetDefaultCollider(); 
                playerReferences.SetDefaultCameraPosition(); 
                break;
            case MovementState.Crouching:
                playerReferences.SetCrouchCollider();
                playerReferences.SetCrouchCameraPosition();
                break;
        }
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        movement = value.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext value)
    {
        if (value.phase != InputActionPhase.Performed) return;

        if (moveState == MovementState.Crouching) return;

        moveState = value.ReadValue<float>() >= 1f ? MovementState.Running : MovementState.Default;
        OnMovementStateChanged();

    }

    public void OnCrouch(InputAction.CallbackContext value)
    {
        if (value.phase != InputActionPhase.Performed) return;

        if (moveState == MovementState.Running) return;

        moveState = value.ReadValue<float>() >= 1f ? MovementState.Crouching : MovementState.Default;
        OnMovementStateChanged();

    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.phase != InputActionPhase.Performed) return;

        if (moveState == MovementState.Crouching || HasJumped || !playerReferences.IsGrounded) return;

        if (value.ReadValue<float>() >= 1f && !HasJumped)
        {
            Jump(); 
            OnMovementStateChanged();
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
    }
}

