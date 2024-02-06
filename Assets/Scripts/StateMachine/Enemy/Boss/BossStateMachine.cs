using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.TextCore.Text;

public class BossStateMachine : StateMachine
{
    [Header("Movement Parameters")]
    public float movementSpeed = 1.5f;
    public float dashSpeed = 2.5f;
    public float walkSpeed = 0.7f;
    public float chaseSpeed = 2f;
    public float dodgeSpeed = 2f;
    public float changeDirectionSpeed = 15;
    public float jumpForce = 0.6f;
    public bool walkMode;
    public bool isDashing;
    public float groundCheckDistance = 0.2f;
    public float slideSpeed = 2.0f;
    public Vector3 slideDirection;

    [Header("Combat Parameters")]
    public float attackRange = 2f;

    [Header("Required Components")]
    public CharacterController controller;
    public Animator animator;
    public ForceReceiver forceReceiver;
    public ComboManager comboManager;
    public Character character;
    public TargetManager targetManager;
    public GroundChecker groundChecker;

    private Vector3 groundNormal;

    private Coroutine actionCoroutine;
    public bool targetInRange;
    public float targetDistance;
    public GameObject fireballPrefab;
    public GameObject firewallPrefab;

    public bool below50Percent;
    public int specialUseCount;

    public void Start()
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

        SwitchState(new BossIdleState(this));
    }

    public void OnDamage()
    {
        if (character.CurrentHpPercent <= 0.5f && !below50Percent)
        {
            below50Percent = true;
        }
        SwitchState(new BossImpactState(this));
    }

    public void OnDie(GameObject dieCharacter)
    {
        SwitchState(new BossDieState(this));
    }
}
