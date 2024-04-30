using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardDieState : WizardState
{
    public WizardDieState(WizardStateMachine sm) : base(sm) { }

    private int dieHash = Animator.StringToHash("Die");
    private float crossFixedDuration = 0.1f;

    public override void Enter()
    {
        PlayAnimation(dieHash, crossFixedDuration);
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
