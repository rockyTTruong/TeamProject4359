using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcherDieState : EnemyState
{
    private SkeletonArcherStateMachine skeletonArcherStateMachine;
    public SkeletonArcherDieState(SkeletonArcherStateMachine skeletonArcherStateMachine) : base(skeletonArcherStateMachine)
    {
        this.skeletonArcherStateMachine = skeletonArcherStateMachine;
    }

    private int dieHash = Animator.StringToHash("Die");
    private float crossFixedDuration = 0.1f;

    public override void Enter()
    {
        PlayAnimation(dieHash, crossFixedDuration);
        skeletonArcherStateMachine.forceReceiver.enabled = false;
        skeletonArcherStateMachine.controller.enabled = false;
        skeletonArcherStateMachine.GetComponent<AttackHandler>().HitboxDisabled();
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
    }
}