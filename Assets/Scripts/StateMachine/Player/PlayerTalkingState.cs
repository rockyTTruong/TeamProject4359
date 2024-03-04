using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTalkingState : PlayerState
{
    public PlayerTalkingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private int freelookHash = Animator.StringToHash("FreeLookBlendTree");
    private int blendSpeedHash = Animator.StringToHash("FreeLookBlendSpeed");
    private float crossFadeDuration = 0.1f;

    public override void Enter()
    {
        psm.currentState = PlayerStates.Talking;
        psm.animator.SetFloat(blendSpeedHash, 0f);
        PlayAnimation(freelookHash, crossFadeDuration);
        InputReader.Instance.buttonPress[(int)GamePadButton.WestButton] += NextDialogue;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] += NextDialogue;
    }

    public override void Exit()
    {
        InputReader.Instance.buttonPress[(int)GamePadButton.WestButton] -= NextDialogue;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] -= NextDialogue;
    }

    public override void Tick()
    {
        Move(Vector2.zero);
    }

    private void NextDialogue()
    {
        DialogueManager.Instance.HandleUserInput();
    }
}
