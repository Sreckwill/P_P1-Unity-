using UnityEngine;

public class PlayerRunningState : PlayerBaseState
{
    private PlayerStateMechine stateMechine;

    public PlayerRunningState(PlayerStateMechine stateMechine)
    {
        this.stateMechine = stateMechine;
    }

    public override void EnterState()
    {
        Debug.Log("Enter Running State");
        stateMechine.onRunningStateChange += HandleRunningStateChange;
        stateMechine.animator.SetBool("isRunning", stateMechine.isRunning);
    }

    private void HandleRunningStateChange(bool running)
    {
        if (!running)
        {
            stateMechine.SwitchState(PlayerState.Moveing);
        }
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Running State");
        stateMechine.onRunningStateChange -= HandleRunningStateChange;
    }


    public override void FixedUpdateState()
    {
        stateMechine.Move(stateMechine.runSpeed);
    }

    public override void UpdateState()
    {
        float speed = stateMechine.GetMoveInput().magnitude;
        stateMechine.animator.SetFloat("Speed", speed);
    }
}
