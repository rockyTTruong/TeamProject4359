using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonArcherChasingState : EnemyState
{
    private bool targetInRange;
    private float targetDistance;

    public SkeletonArcherChasingState(SkeletonArcherStateMachine skeletonArcherStateMachine) : base(skeletonArcherStateMachine)
    {
        this.skeletonArcherStateMachine = skeletonArcherStateMachine;
    }

    private SkeletonArcherStateMachine skeletonArcherStateMachine;
    private int runHash = Animator.StringToHash("Run");
    private float crossFixedDuration = 0.3f;
    private float intervalTimer;

    public override void Enter()
    {
        PlayAnimation(runHash, crossFixedDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
        ChaseTarget(4f);
        if (skeletonArcherStateMachine.targetManager.GetCurrentTarget() == null) 
        { 
            enemyStateMachine.SwitchState(new SkeletonArcherIdleState(skeletonArcherStateMachine));
        }
        if (targetDistance <= 5f)
        {
            intervalTimer += Time.deltaTime;
            if (intervalTimer > 0.2f)
            {
                intervalTimer -= 0.2f;
                if (Random.Range(0, 100) < 30)
                {
                    enemyStateMachine.SwitchState(new SkeletonArcherAttackingState(skeletonArcherStateMachine, 2));
                }
            }
        }
        if (targetInRange)
        {
            if (Random.Range(0, 100) < 30)
            {
                enemyStateMachine.SwitchState(new SkeletonArcherAttackingState(skeletonArcherStateMachine, 1));
            }
            else
            {
                enemyStateMachine.SwitchState(new SkeletonArcherAttackingState(skeletonArcherStateMachine, 0));
            }
        }
    }

    private void ChaseTarget(float chaseStopRange)
    {
        targetDistance = enemyStateMachine.targetManager.GetDistanceToTarget();

        if (targetDistance <= chaseStopRange)
        {
            targetInRange = true;
        }
        else
        {
            targetInRange = false;
            FaceTarget(enemyStateMachine.changeDirectionSpeed);
            Move(enemyStateMachine.transform.forward * enemyStateMachine.chaseSpeed);
        }
    }
}