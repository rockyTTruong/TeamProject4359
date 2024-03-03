using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChargeAttackingState : PlayerState
{
    public PlayerChargeAttackingState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
        chargingAttackHash = Animator.StringToHash("ChargingAttack");
    }

    private float normalizedTime;
    private int chargingAttackHash;
    private float crossFixedDuration = 0.1f;

    public override void Enter()
    {
        psm.currentState = PlayerStates.Attacking;
        AttackHandler attackHandler = psm.GetComponent<AttackHandler>();
        attackHandler.HitboxDisabled();
        attackHandler.DisabledSwordTrail();

        PlayAnimation(chargingAttackHash, crossFixedDuration);
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] += LockOnMode;
    }

    public override void Exit()
    {
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] -= LockOnMode;
    }

    public override void Tick()
    {
        HandleCameraMovement();
        Move(Vector3.zero);
        GameObject target = psm.targetManager.GetCurrentTarget();
        if (target != null)
        {
            FaceTarget(target);
        }

        //if (InputReader.Instance.isPressingWestButton) return; 

        psm.SwitchState(new PlayerAttackingState(psm, psm.comboManager.chargeSwordAttackCombo, 0));
    }
}
