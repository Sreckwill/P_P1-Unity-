using System;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    private PlayerStateMechine stateMechine;

    public PlayerMoveState(PlayerStateMechine stateMechine)
    {
        this.stateMechine = stateMechine;
    }

    public override void EnterState()
    {
        Debug.Log("Enter Move State");
        stateMechine.onRunningStateChange += HandleRunningStateChange;
        stateMechine.onCrouchStateChange += HandleCrouchStateChange;
        stateMechine.animator.SetBool("isRunning", stateMechine.isRunning);
    }

    private void HandleCrouchStateChange(bool crouching)
    {
        if(crouching)
        {
            stateMechine.SwitchState(PlayerState.Crouch);
        }
    }

    private void HandleRunningStateChange(bool running)
    {
        if (running)
        {
            stateMechine.SwitchState(PlayerState.Running);
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Move State");
        stateMechine.onRunningStateChange -= HandleRunningStateChange;
        stateMechine.onCrouchStateChange -= HandleCrouchStateChange;
    }


    public override void FixedUpdateState()
    {
        stateMechine.Move(stateMechine.walkSpeed);
    }

    public override void UpdateState()
    {
        float speed = stateMechine.GetMoveInput().magnitude;
        stateMechine.animator.SetFloat("Speed", speed);
        stateMechine.BoxColliderChange();
        if (speed == 0)
        {
            stateMechine.animator.SetFloat("Speed", 0);
        }
    }
}
