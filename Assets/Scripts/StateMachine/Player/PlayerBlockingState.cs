using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockingState : PlayerState
{
    public PlayerBlockingState(PlayerStateMachine psm) : base(psm) { }

    private int frontLookHash = Animator.StringToHash("FrontLookBlendTree");
    private int frontLookYHash = Animator.StringToHash("FrontLookY");
    private int frontLookXHash = Animator.StringToHash("FrontLookX");
    private float crossFadeDuration = 0.1f;

    private float perfectBlockTimer = 0f;

    public override void Enter()
    {
        psm.isDashing = false;
        psm.currentState = PlayerStates.Blocking;
        psm.character.isPerfectBlock = true;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] += Jump;
        InputReader.Instance.buttonPress[(int)GamePadButton.EastButton] += Dodge;
        PlayAnimation(frontLookHash, crossFadeDuration);
    }

    public override void Exit()
    {
        psm.character.isPerfectBlock = false;
        psm.character.isBlocking = false;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] -= Jump;
        InputReader.Instance.buttonPress[(int)GamePadButton.EastButton] -= Dodge;
    }

    public override void Tick()
    {
        HandleFrontLookMovement();
        UpdateLocomotionState();

        perfectBlockTimer += Time.deltaTime;
        if (perfectBlockTimer > 0.2f)
        {
            psm.character.isPerfectBlock = false;
            psm.character.isBlocking = true;
        }

        if (!InputReader.Instance.buttonHold[(int)GamePadButton.RightShoulder])
        {
            psm.SwitchState(new PlayerFreeLookState(psm));
        }

        if (!psm.groundChecker.IsGrounded) psm.SwitchState(new PlayerFallingState(psm));
    }

    private void UpdateLocomotionState()
    {
        Vector2 input = InputReader.Instance.leftStickValue;
        if (input == Vector2.zero)
        {
            psm.animator.SetFloat(frontLookYHash, 0f, 0.25f, Time.deltaTime);
            psm.animator.SetFloat(frontLookXHash, 0f, 0.25f, Time.deltaTime);
        }
        else
        {
            psm.animator.SetFloat(frontLookYHash, input.y, 0.25f, Time.deltaTime);
            psm.animator.SetFloat(frontLookXHash, input.x, 0.25f, Time.deltaTime);
        }
    }
}
