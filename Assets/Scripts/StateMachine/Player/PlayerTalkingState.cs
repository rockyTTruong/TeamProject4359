using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTalkingState : PlayerState
{
    public PlayerTalkingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    //private int freelookHash = AnimatorHash.freelookHash;
    //private float crossFadeDuration = 0.1f;

    public override void Enter()
    {
        psm.currentState = PlayerStates.Talking;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] += NextDialogue;
    }

    public override void Exit()
    {
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] -= NextDialogue;
    }

    public override void Tick()
    {
    }

    private void NextDialogue()
    {
        DialogueManager.Instance.HandleUserInput();
    }
}
