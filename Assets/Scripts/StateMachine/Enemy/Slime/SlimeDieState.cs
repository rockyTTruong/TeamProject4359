using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeDieState : SlimeState
{
    public SlimeDieState(SlimeStateMachine sm) : base(sm) { }

    private int dieHash = Animator.StringToHash("Die");
    private float crossFixedDuration = 0.1f;

    public override void Enter()
    {
        sm.forceReceiver.enabled = false;
        sm.controller.enabled = false;
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
    }
}
