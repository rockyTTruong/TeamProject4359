using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoppingState : PlayerState
{
    public PlayerShoppingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    //private int freelookHash = AnimatorHash.freelookHash;
    //private float crossFadeDuration = 0.1f;

    public override void Enter()
    {
        psm.currentState = PlayerStates.Shopping;
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
    }
}
