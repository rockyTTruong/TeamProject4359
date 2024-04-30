using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeChasingState : SlimeState
{
    public SlimeChasingState(SlimeStateMachine sm) : base(sm) { }

    private int runHash = Animator.StringToHash("blueRun");
    private float crossFixedDuration = 0.3f;

    private bool targetInRange;
    private float targetDistance;
    public override void Enter()
    {
        PlayAnimation(runHash, crossFixedDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
        ChaseTarget(1.5f);
        if (sm.targetManager.GetCurrentTarget() == null)
        {
            sm.SwitchState(new SlimeIdleState(sm));
        }
        if (targetInRange)
        {
            sm.SwitchState(new SlimeAttackingState(sm));
        }
    }

    private void ChaseTarget(float chaseStopRange)
    {
        targetDistance = sm.targetManager.GetDistanceToTarget();

        if (targetDistance <= chaseStopRange)
        {
            targetInRange = true;
        }
        else
        {
            targetInRange = false;
            FaceTarget();
            Move(sm.transform.forward * sm.chaseSpeed);
        }
    }
}
