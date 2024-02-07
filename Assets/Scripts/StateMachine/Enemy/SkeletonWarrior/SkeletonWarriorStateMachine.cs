using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkeletonWarriorStateMachine : EnemyStateMachine
{
    public List<GameObject> patrolPath = new List<GameObject>();
    public bool isAggro;
    public bool isPatrol;

    public override void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        forceReceiver = GetComponent<ForceReceiver>();
        comboManager = GetComponent<ComboManager>();
        character = GetComponent<Character>();
        targetManager = GetComponentInChildren<TargetManager>();
        groundChecker = GetComponent<GroundChecker>();
        character.DamageEvent += OnDamage;
        character.DieEvent += OnDie;

        SwitchState(new SkeletonWarriorIdleState(this));
    }

    public override void OnDamage()
    {
        SwitchState(new SkeletonWarriorImpactState(this));
    }

    public override void OnDie(GameObject dieCharacter)
    {
        SwitchState(new SkeletonWarriorDieState(this));
    }
}
