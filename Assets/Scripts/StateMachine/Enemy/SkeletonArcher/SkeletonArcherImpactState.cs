using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcherImpactState : EnemyState
{
    private SkeletonArcherStateMachine skeletonArcherStateMachine;

    public SkeletonArcherImpactState(SkeletonArcherStateMachine skeletonArcherStateMachine) : base(skeletonArcherStateMachine)
    {
        this.skeletonArcherStateMachine = skeletonArcherStateMachine;
    }

    private int impactHash = Animator.StringToHash("Impact");
    private float crossFixedDuration = 0.1f;

    public override void Enter()
    {
        PlayAnimation(impactHash, crossFixedDuration);
        skeletonArcherStateMachine.GetComponent<AttackHandler>().HitboxDisabled();
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
        ApplyForce();
        float normalizedTime = GetNormalizedTime(skeletonArcherStateMachine.animator, impactHash);
        if (normalizedTime >= 1f)
        {
            skeletonArcherStateMachine.SwitchState(new SkeletonArcherIdleState(skeletonArcherStateMachine));
        }
    }

    public void ApplyForce()
    {
        Move(Vector3.zero);
    }
}
