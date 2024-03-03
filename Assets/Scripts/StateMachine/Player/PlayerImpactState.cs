using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImpactState : PlayerState
{
    public PlayerImpactState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    private int impactHash = Animator.StringToHash("Impact");
    private float crossFixedDuration = 0.1f;

    public override void Enter()
    {
        psm.currentState = PlayerStates.Hit;
        AttackHandler attackHandler = psm.GetComponent<AttackHandler>();
        attackHandler.HitboxDisabled();
        attackHandler.DisabledSwordTrail();

        PlayAnimation(impactHash, crossFixedDuration);
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] += LockOnMode;
    }

    public override void Exit()
    {
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] -= LockOnMode;
    }

    public override void Tick()
    {
        Move(Vector3.zero); //Apply Force
        float normalizedTime = GetNormalizedTime(psm.animator, impactHash);
        if (normalizedTime >= 1f)
        {
            psm.SwitchState(new PlayerFreeLookState(psm));
        }
    }
}
