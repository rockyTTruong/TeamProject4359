using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerFallingState : PlayerState
{
    public PlayerFallingState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    private readonly int fallHash = Animator.StringToHash("Fall");
    private const float crossFadeDuration = 0.1f;

    public override void Enter()
    {
        psm.isFalling = true;
        AttackHandler attackHandler = psm.GetComponent<AttackHandler>();
        attackHandler.HitboxDisabled();
        attackHandler.DisabledSwordTrail();
        PlayAnimation(fallHash, crossFadeDuration);
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] += LockOnMode;
    }

    public override void Exit()
    {
        psm.isFalling = false;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] -= LockOnMode;
    }

    public override void Tick()
    {
        HandlePlayerMovement();
        HandleCameraMovement();
        if (psm.groundChecker.IsGrounded)
        {
            psm.SwitchState(new PlayerFreeLookState(psm));
        }
    }
}
