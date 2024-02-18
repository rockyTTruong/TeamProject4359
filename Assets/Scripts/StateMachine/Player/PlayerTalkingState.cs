using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.InputSystem.LowLevel;

public class PlayerTalkingState : PlayerState
{
    public PlayerTalkingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    //private int freelookHash = AnimatorHash.freelookHash;
    //private float crossFadeDuration = 0.1f;

    public override void Enter()
    {
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
