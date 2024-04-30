using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardIdleState : WizardState
{
    public WizardIdleState(WizardStateMachine sm) : base(sm) { }

    private int idleHash = Animator.StringToHash("Idle");
    private float crossFixedDuration = 0.3f;
    private float timer;

    private float idleTime = 2f;

    public override void Enter()
    {
        timer = 0f;
        PlayAnimation(idleHash, crossFixedDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
        if (sm.targetManager.GetCurrentTarget() == null) return;
        Move(Vector3.zero);
        FaceTarget();

        timer += Time.deltaTime;
        if (timer >= idleTime)
        {
            if (sm.targetManager.GetDistanceToTarget() < 4f)
            {
                sm.SwitchState(new WizardTeleportState(sm));
            }
            else sm.SwitchState(new WizardCastingState(sm));
            timer -= idleTime;
        }
    }
}
