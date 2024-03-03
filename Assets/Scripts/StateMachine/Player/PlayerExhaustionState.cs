using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExhaustionState : PlayerState
{
    public PlayerExhaustionState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    private int exhaustionHash = Animator.StringToHash("Exhaustion");
    private float crossFixedDuration = 0.1f;

    public override void Enter()
    {
        psm.currentState = PlayerStates.Exhausting;
        AttackHandler attackHandler = psm.GetComponent<AttackHandler>();
        attackHandler.HitboxDisabled();
        attackHandler.DisabledSwordTrail();

        PlayAnimation(exhaustionHash, crossFixedDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
        Move(Vector3.zero);
        float normalizedTime = GetNormalizedTime(psm.animator, exhaustionHash);
        if (normalizedTime >= 1f)
        {
            psm.SwitchState(new PlayerFreeLookState(psm));
        }
    }
}
