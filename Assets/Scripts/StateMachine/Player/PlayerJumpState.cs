using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    private readonly int jumpHash = Animator.StringToHash("Jump");
    private const float crossFadeDuration = 0.1f;

    public override void Enter()
    {
        psm.isJumping = true;
        AttackHandler attackHandler = psm.GetComponent<AttackHandler>();
        attackHandler.HitboxDisabled();
        attackHandler.DisabledSwordTrail();

        psm.forceReceiver.Jump();
        PlayAnimation(jumpHash, crossFadeDuration);
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] += LockOnMode;
    }

    public override void Exit()
    {
        psm.isJumping = false;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] -= LockOnMode;
    }

    public override void Tick()
    {
        HandlePlayerMovement();
        HandleCameraMovement();
        if (psm.controller.velocity.y <= 0)
        {
            psm.SwitchState(new PlayerFallingState(psm));
            return;
        }
    }
}