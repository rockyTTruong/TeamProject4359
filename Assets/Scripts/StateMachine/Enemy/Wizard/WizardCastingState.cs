using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardCastingState : WizardState
{
    public WizardCastingState(WizardStateMachine sm) : base(sm) { }

    private int fireballHash = Animator.StringToHash("CastFireball");
    private int icicleHash = Animator.StringToHash("CastIcicle");
    private int thunderHash = Animator.StringToHash("CastThunder");
    private float crossFixedDuration = 0.1f;

    private int animationHash;
    public override void Enter()
    {
        int i = Random.Range(0, 2);
        switch (i)
        {
            case 0:
                animationHash = fireballHash;
                break;

            case 1:
                animationHash = icicleHash;
                break;

            case 2:
                animationHash = thunderHash;
                break;
        }
        PlayAnimation(animationHash, crossFixedDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
        Move(Vector3.zero);
        FaceTarget();

        float normalizedTime = GetNormalizedTime(sm.animator, animationHash);
        if (normalizedTime >= 1f)
        {
            sm.SwitchState(new WizardIdleState(sm));
        }
    }
}
