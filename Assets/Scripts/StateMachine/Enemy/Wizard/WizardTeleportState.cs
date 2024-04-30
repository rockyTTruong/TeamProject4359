using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardTeleportState : WizardState
{
    public WizardTeleportState(WizardStateMachine sm) : base(sm) { }

    private int teleportHash = Animator.StringToHash("Teleport");
    private float crossFixedDuration = 0.1f;

    public override void Enter()
    {
        PlayAnimation(teleportHash, crossFixedDuration);
        sm.forceReceiver.enabled = false;
        sm.controller.enabled = false;
    }

    public override void Exit()
    {
        sm.forceReceiver.enabled = true;
        sm.controller.enabled = true;
    }

    public override void Tick()
    {
        float normalizedTime = GetNormalizedTime(sm.animator, teleportHash);
        if (normalizedTime >= .7f)
        {
            sm.currentPositionIndex++;
            if (sm.currentPositionIndex >= sm.teleportPosition.Count)
            {
                sm.currentPositionIndex = 0;
            }
            sm.transform.position = sm.teleportPosition[sm.currentPositionIndex].transform.position;
            sm.SwitchState(new WizardIdleState(sm));
        }
    }
}
