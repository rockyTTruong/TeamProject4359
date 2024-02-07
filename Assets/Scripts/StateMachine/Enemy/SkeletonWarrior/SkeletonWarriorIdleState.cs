using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkeletonWarriorIdleState : EnemyState
{
    private SkeletonWarriorStateMachine sm;

    public SkeletonWarriorIdleState(SkeletonWarriorStateMachine skeletonWarriorStateMachine) : base(skeletonWarriorStateMachine)
    {
        sm = skeletonWarriorStateMachine;
    }

    private int idleHash = Animator.StringToHash("Idle");
    private int aggroHash = Animator.StringToHash("Aggro");
    private int idleBlendHash = Animator.StringToHash("IdleBlendSpeed");
    private float crossFixedDuration = 0.3f;
    private int patrolIndex;
    private GameObject destination;
    private float idleTime;
    private float randomIdleTime;
    private bool playingAnimation;
    private float patrolIdleTime = 5f;

    public override void Enter()
    {
        if (sm.patrolPath.Count() != 0)
        {
            patrolIndex = 0;
        }
        PlayAnimation(idleHash, crossFixedDuration);
        sm.animator.SetFloat(idleBlendHash, 0f);
        randomIdleTime = Random.Range(0.5f, 1.0f);
    }

    public override void Exit()
    {

    }

    public override void Tick()
    {
        if (GetCurrentTarget() != null)
        {
            FaceTarget(2f);

            if (sm.isAggro)
            {
                sm.isPatrol = false;
                if (playingAnimation)
                {
                    if (GetNormalizedTime(sm.animator, aggroHash) >= 1f)
                    {
                        sm.SwitchState(new SkeletonWarriorChasingState(sm));
                    }
                }
                else sm.SwitchState(new SkeletonWarriorChasingState(sm));
                return;
            }
            else
            {
                sm.isAggro = true;
                PlayAnimation(aggroHash, crossFixedDuration);
                playingAnimation = true;
            }
            return;
        }
        else sm.isAggro = false;

        if (idleTime < randomIdleTime)
        {
            idleTime += Time.deltaTime;
            return;
        }

        if (!sm.isPatrol)
        {
            PlayAnimation(idleHash, crossFixedDuration);
            sm.isPatrol = true;
        }

        if (sm.patrolPath.Count() != 0)
        {
            if (idleTime < patrolIdleTime)
            {
                idleTime += Time.deltaTime * Random.Range(1, 10);
            }
            else
            {
                Vector3 destination = sm.patrolPath[patrolIndex].transform.position;
                destination.y = sm.transform.position.y;
                if (Vector3.Distance(destination, sm.transform.position) <= 0.01f)
                {
                    patrolIndex++;
                    sm.animator.SetFloat(idleBlendHash, 0f);
                    if (patrolIndex == sm.patrolPath.Count()) patrolIndex = 0;
                    idleTime = 0f;
                    patrolIdleTime = Random.Range(1, 10);
                    return;
                }
                else
                {
                    FaceTargetInstantly(destination);
                    Move(sm.transform.forward * sm.walkSpeed);
                    sm.animator.SetFloat(idleBlendHash, 1f, 0.1f, Time.deltaTime);
                }
            }
        }
    }
}
