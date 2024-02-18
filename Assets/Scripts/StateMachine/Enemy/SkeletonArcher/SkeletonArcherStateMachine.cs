using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkeletonArcherStateMachine : EnemyStateMachine
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

        SwitchState(new SkeletonArcherIdleState(this));
    }

    public override void OnDamage()
    {
        SwitchState(new SkeletonArcherImpactState(this));
    }

    public override void OnDie(GameObject dieCharacter)
    {
        SwitchState(new SkeletonArcherDieState(this));
    }
}

