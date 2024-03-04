using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoppingState : PlayerState
{
    public PlayerShoppingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    private int freelookHash = Animator.StringToHash("FreeLookBlendTree");
    private int blendSpeedHash = Animator.StringToHash("FreeLookBlendSpeed");
    private float crossFadeDuration = 0.1f;

    public override void Enter()
    {
        psm.currentState = PlayerStates.Shopping;
        PlayAnimation(freelookHash, crossFadeDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
        Move(Vector2.zero);
    }
}
