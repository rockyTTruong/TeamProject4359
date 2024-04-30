using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAttackingState : SlimeState
{
    public SlimeAttackingState(SlimeStateMachine sm) : base(sm) { }

    private int attackHash = Animator.StringToHash("blueJump");
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
        float normalizedTime = GetNormalizedTime(sm.animator, attackHash);
        if (normalizedTime >= 1f)
        {
            sm.SwitchState(new SlimeIdleState(sm));
        }
    }
}
