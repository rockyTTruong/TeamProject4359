using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgingState : PlayerState
{
    public PlayerDodgingState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    private readonly int dodgeHash = Animator.StringToHash("Dodge");
    private Vector3 movement;
    private const float crossFadeDuration = 0.1f;
    private float invincibleTimer;

    public override void Enter()
    {
        psm.currentState = PlayerStates.Dodging;
        AttackHandler attackHandler = psm.GetComponent<AttackHandler>();
        attackHandler.HitboxDisabled();
        attackHandler.DisabledSwordTrail();

        psm.character.isInvincible = true;
        invincibleTimer = 0f;

        movement = CalculateMovement();
        if (movement != Vector3.zero) ChangeDirectionInstantly(movement);

        PlayAnimation(dodgeHash, crossFadeDuration);
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] += LockOnMode;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] += Jump;
    }

    public override void Exit()
    {
        psm.character.isInvincible = false;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] -= LockOnMode;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] -= Jump;
    }

    public override void Tick()
    {
        HandleCameraMovement();

        if (invincibleTimer < 0.5f && psm.character.isInvincible == true)
        {
            invincibleTimer += Time.deltaTime;
        }
        else if (psm.character.isInvincible == true)
        {
            psm.character.isInvincible = false;
        }

        float normalizedTime = GetNormalizedTime(psm.animator, dodgeHash);
        if (normalizedTime <= 0.7f)
        {
            Move(psm.transform.forward * psm.dodgeSpeed);
        }
        else
        {
            Move(psm.transform.forward * psm.dodgeSpeed * 0.2f);
        }
        if (normalizedTime >= 0.9f)
        {
            psm.SwitchState(new PlayerFreeLookState(psm));
        }
    }
}
