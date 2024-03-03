using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDieState : PlayerState
{
    public PlayerDieState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    private int dieHash = Animator.StringToHash("Die");
    private float crossFixedDuration = 0.1f;

    public override void Enter()
    {
        psm.currentState = PlayerStates.Die;
        AttackHandler attackHandler = psm.GetComponent<AttackHandler>();
        attackHandler.HitboxDisabled();
        attackHandler.DisabledSwordTrail();

        PlayAnimation(dieHash, crossFixedDuration);
        psm.forceReceiver.enabled = false;
        psm.controller.enabled = false;
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
    }
}
