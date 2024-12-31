using System;
using UnityEngine;

public class PlayerStateMechine : MonoBehaviour
{
    private PlayerBaseState currentState;
    private PlayerMoveState moveState;
    private PlayerRunningState runningState;
    private PlayerCrouchState crouchState;

    public PlayerState currentPlayerState;

    public Rigidbody rb;
    public Animator animator;
    public BoxCollider boxCollider;

    private float moveX;
    private float moveY;

    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;

    private Vector3 moveInput;
    
    public bool isShiftPressed;
    public bool isRunning = false;

    public bool isCKeyPressed;
    public bool isCrouching = false;

    public event Action<bool> onRunningStateChange;
    public event Action<bool> onCrouchStateChange;


    public float crouchHeight = 0.5f;  // Height of the collider when crouched
    public float standHeight = 2.0f;   // Height of the collider when standing
    public float crouchSpeed = 3f;     // Speed of crouch transition
    public float standingSpeed = 4f;   // Speed of standing transition

    public Vector3 originalCenter;
    public Vector3 crouchCenter;

    private PlayerIkSystem playerIKSystem;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        playerIKSystem = GetComponent<PlayerIkSystem>();

        if (rb == null || animator == null || boxCollider == null || playerIKSystem == null)
        {
            Debug.LogError("Rigidbody or Animator or BoxCollider or PlayerIkSystem is missing!");
            enabled = false;
            return;
        }


        // Store the original center of the collider
        originalCenter = boxCollider.center;
        crouchCenter = new Vector3(originalCenter.x, crouchHeight / 2, originalCenter.z);

        moveState = new PlayerMoveState(this);
        runningState = new PlayerRunningState(this);
        crouchState = new PlayerCrouchState(this);

        currentPlayerState = PlayerState.Moveing;
        currentState = moveState;
        currentState?.EnterState();
    }

    private void Update()
    {
        currentState?.UpdateState();
        HandlePlayerInput();
    }

    public void BoxColliderChange()
    {
        // Smoothly adjust the collider's size and center during crouch and stand
        if (isCrouching)
        {
            boxCollider.size = Vector3.Lerp(boxCollider.size, new Vector3(boxCollider.size.x, crouchHeight, boxCollider.size.z), Time.deltaTime * crouchSpeed);
            boxCollider.center = Vector3.Lerp(boxCollider.center, crouchCenter, Time.deltaTime * crouchSpeed);
        }
        else
        {
            boxCollider.size = Vector3.Lerp(boxCollider.size, new Vector3(boxCollider.size.x, standHeight, boxCollider.size.z), Time.deltaTime * standingSpeed);
            boxCollider.center = Vector3.Lerp(boxCollider.center, originalCenter, Time.deltaTime * standingSpeed);
        }
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdateState();
    }

    public void SwitchState(PlayerState newState)
    {
        if (currentPlayerState == newState) return;

        currentState?.ExitState();
        currentPlayerState = newState;

        currentState = newState switch
        {
            PlayerState.Moveing => moveState,
            PlayerState.Running => runningState,
            PlayerState.Crouch => crouchState,
            _ => currentState
        };

        currentState?.EnterState();
    }

    private void HandlePlayerInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(moveX, rb.velocity.y, moveY).normalized;


        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }

        ApplyRunning();
        ApplyCrouching();

        // Set the look target dynamically
        if (playerIKSystem.lookTarget != null)
        {
            playerIKSystem.SetLookTarget(playerIKSystem.lookTarget);
        }
    }

    private void ApplyCrouching()
    {
        isCKeyPressed = Input.GetKey(KeyCode.C);
        if (isCKeyPressed && !isCrouching)
        {
            isCrouching = true;
            onCrouchStateChange?.Invoke(isCrouching);
        }
        else if (!isCKeyPressed && isCrouching)
        {
            isCrouching = false;
            onCrouchStateChange?.Invoke(isCrouching);
        }
    }

    private void ApplyRunning()
    {
        isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        // Handle running logic based on shift key and movement
        if (isShiftPressed && moveInput.magnitude > 0 && !isRunning)
        {
            isRunning = true;
            onRunningStateChange?.Invoke(isRunning);
        }
        else if ((!isShiftPressed || moveInput.magnitude == 0) && isRunning)
        {
            isRunning = false;
            onRunningStateChange?.Invoke(isRunning);
        }
    }

    private bool IsGrounded()
    {
        // Simple check to see if the player is on the ground
        return Physics.Raycast(transform.position, Vector3.down, 0.5f);
    }
    public Vector3 GetMoveInput() => moveInput;

    public void Move(float speed)
    {
        Vector3 velocity = moveInput * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + velocity);
    }
    // In case you want to remove the look target
    private void RemoveLookTarget()
    {
        playerIKSystem.ClearLookTarget();
    }
}
