using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockingImpactState : PlayerState
{
    public PlayerBlockingImpactState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    private int blockImpactHash = Animator.StringToHash("BlockImpact");
    private float crossFixedDuration = 0.1f;

    public override void Enter()
    {
        psm.currentState = PlayerStates.BlockImpact;
        AttackHandler attackHandler = psm.GetComponent<AttackHandler>();
        attackHandler.HitboxDisabled();
        attackHandler.DisabledSwordTrail();

        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] += Jump;
        InputReader.Instance.buttonPress[(int)GamePadButton.EastButton] += Dodge;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] += LockOnMode;
        InputReader.Instance.buttonPress[(int)GamePadButton.RightShoulder] += Block;
        PlayAnimation(blockImpactHash, crossFixedDuration);
    }

    public override void Exit()
    {
        psm.character.isPerfectBlock = false;
        psm.character.isBlocking = false;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] -= Jump;
        InputReader.Instance.buttonPress[(int)GamePadButton.EastButton] -= Dodge;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] -= LockOnMode;
        InputReader.Instance.buttonPress[(int)GamePadButton.RightShoulder] -= Block;
    }

    public override void Tick()
    {
        Move(-psm.transform.forward * 0.4f);
        float normalizedTime = GetNormalizedTime(psm.animator, blockImpactHash);
        if (normalizedTime >= 1f)
        {
            psm.SwitchState(new PlayerBlockingState(psm));
        }

        if (!InputReader.Instance.buttonHold[(int)GamePadButton.RightShoulder])
        {
            psm.SwitchState(new PlayerFreeLookState(psm));
        }
    }
}
