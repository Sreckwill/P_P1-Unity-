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

    public bool isRunning = false;
    public bool isCrouching = false;

    public event Action<bool> onRunningStateChange;
    public event Action<bool> onCrouchStateChange;
   
    bool isCKeyPressed;
    public float crouchHeight = 0.5f;  
    public float standHeight = 2.0f;   
    public float crouchSpeed = 3f;     
    public float standingSpeed = 4f;   

    public Vector3 originalCenter;
    public Vector3 crouchCenter;

    private PlayerIkSystem playerIKSystem;

    public float rotationSpeed = 10f;

    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        playerIKSystem = GetComponent<PlayerIkSystem>();

        if (rb == null || animator == null || boxCollider == null || playerIKSystem == null)
        {
            Debug.LogError("Rigidbody, Animator, BoxCollider, or PlayerIkSystem is missing!");
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

    private void FixedUpdate()
    {
        currentState?.FixedUpdateState();
    }

    // Switch between states
    public void SwitchState(PlayerState newState)
    {
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

    // Handle player input for movement and state transitions
    private void HandlePlayerInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(moveX, 0, moveY).normalized;

        // Jump logic
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }

        // Apply crouch and running logic
        ApplyRunning();
        ApplyCrouching();

        // Set the look target dynamically
        if (playerIKSystem.lookTarget != null)
        {
            playerIKSystem.AdjustLookTargetForCrouch(isCrouching);
        }

        // Camera-relative movement and rotation
        if (!isCrouching &&moveInput.magnitude >= 0.1f)
        {
            float speed = isRunning ? runSpeed : walkSpeed;
            // Pass the correct speed to MovePlayer
            MovePlayer(speed);
        }
        else
        {
            // No input, stop movement
            rb.MovePosition(new Vector3(rb.position.x, rb.position.y, rb.position.z));
        }
    }

    // Get movement input as a Vector3
    public Vector3 GetMoveInput() => moveInput;
    

    public void MovePlayer(float speed)
    {
        // Get the camera's forward and right vectors
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        // Flatten the vectors to the horizontal plane
        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate the movement direction relative to the camera
        Vector3 moveDirection = cameraForward * moveInput.z + cameraRight * moveInput.x;

        // Calculate the target rotation and apply it to the Rigidbody
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        // Apply movement using MovePosition
        Vector3 targetPosition = rb.position + moveDirection * speed * Time.fixedDeltaTime;
        rb.MovePosition(targetPosition);
    }

    // Handle crouching logic
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

        // Smoothly change the collider size and center based on crouch state
        BoxColliderChange();
    }

    // Handle running logic
    private void ApplyRunning()
    {
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);

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

    // Check if the player is grounded
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.5f);
    }

    // Smoothly change the collider's size and center during crouch and stand
    public void BoxColliderChange()
    {
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
}
