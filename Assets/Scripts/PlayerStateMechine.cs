using System;
using UnityEngine;

public class PlayerStateMechine : MonoBehaviour
{
    private PlayerBaseState currentState;
    private PlayerMoveState moveState;
    private PlayerRunningState runningState;

    public PlayerState currentPlayerState;
    public Rigidbody rb;
    public Animator animator;


    private float moveX;
    private float moveY;

    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;

    private Vector3 moveInput;
    
    public bool isShiftPressed;
    public bool isRunning = false;

    public event Action<bool> onRunningStateChange;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        if (rb == null || animator == null)
        {
            Debug.LogError("Rigidbody or Animator is missing!");
            enabled = false;
            return;
        }

        moveState = new PlayerMoveState(this);
        runningState = new PlayerRunningState(this);

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

    public void SwitchState(PlayerState newState)
    {
        if (currentPlayerState == newState) return;

        currentState?.ExitState();
        currentPlayerState = newState;

        currentState = newState switch
        {
            PlayerState.Moveing => moveState,
            PlayerState.Running => runningState,
            _ => currentState
        };

        currentState?.EnterState();
    }

    private void HandlePlayerInput()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(moveX, rb.velocity.y, moveY).normalized;
        isShiftPressed = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }

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
}
