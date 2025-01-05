using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchState : PlayerBaseState
{
    private PlayerStateMechine stateMechine;

    public PlayerCrouchState(PlayerStateMechine stateMechine)
    {
        this.stateMechine = stateMechine;
    }

    public override void EnterState()
    {
        Debug.Log("Enter the Crouch State");
        stateMechine.onCrouchStateChange += HandleCrouchStateChange;
        stateMechine.animator.SetBool("Crouch", stateMechine.isCrouching);
        stateMechine.animator.SetBool("isRunning", stateMechine.isRunning);
        stateMechine.animator.CrossFadeInFixedTime("Crouch", 0.1f);
    }

    private void HandleCrouchStateChange(bool crouching)
    {
        if (!crouching)
        {
            stateMechine.SwitchState(PlayerState.Moveing);
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exit the Crouch State");
        stateMechine.onCrouchStateChange -= HandleCrouchStateChange;
        stateMechine.animator.SetBool("Crouch", stateMechine.isCrouching);
    }

    public override void FixedUpdateState()
    {
        Debug.Log("Fixed Update the Crouch State");
    }

    public override void UpdateState()
    {
        Debug.Log("Update the Crouch State");

        // This is where you handle the collider change when the player is crouching or standing up.
        stateMechine.BoxColliderChange();
    }

}
