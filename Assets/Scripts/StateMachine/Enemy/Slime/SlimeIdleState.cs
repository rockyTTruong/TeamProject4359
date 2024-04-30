using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlimeIdleState : SlimeState
{
    public SlimeIdleState(SlimeStateMachine sm) : base(sm) { }

    private int idleHash = Animator.StringToHash("blueBow");
    private float crossFixedDuration = 0.3f;
    private float timer;
    private float randomIdleTime;

    public override void Enter()
    {
        PlayAnimation(idleHash, crossFixedDuration);
        randomIdleTime = Random.Range(0.5f, 1.0f);
    }

    public override void Exit()
    {

    }

    public override void Tick()
    {
        if (timer < randomIdleTime)
        {
            timer += Time.deltaTime;
            return;
        }

        if (GetCurrentTarget() != null)
        {
            sm.SwitchState(new SlimeChasingState(sm));
        }
    }
}
