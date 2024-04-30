using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSlimeAttackingState : RedSlimeState
{
    public RedSlimeAttackingState(RedSlimeStateMachine sm) : base(sm) { }

    private int attackHash = Animator.StringToHash("wizardJump");
    private float crossFixedDuration = 0.3f;

    public override void Enter()
    {
        PlayAnimation(attackHash, crossFixedDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
        FaceTarget();
        float normalizedTime = GetNormalizedTime(sm.animator, attackHash);
        if (normalizedTime >= 1f)
        {
            sm.SwitchState(new RedSlimeIdleState(sm));
        }
    }
}
