using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeStateMachine : StateMachine
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
    public Character character;
    public TargetManager targetManager;

    public GameObject attackCollider;
    public GameObject fireballPrefab;
    public GameObject hitEffectPrefab;
    public WizardBossSpawner wizardBossSpawner;

    public void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        forceReceiver = GetComponent<ForceReceiver>();
        character = GetComponent<Character>();
        targetManager = GetComponentInChildren<TargetManager>();
        character.DamageEvent += OnDamage;
        character.DieEvent += OnDie;

        SwitchState(new SlimeIdleState(this));
    }

    public void OnDamage()
    {
    }

    public void OnDie(GameObject dieCharacter)
    {
        SwitchState(new SlimeDieState(this));
        wizardBossSpawner.ReportKill(this.gameObject);
    }

    public void EnableAttackCollider()
    {
        attackCollider.GetComponent<AttackCollider>().SetAttack(10, 0.1f, 0f, 0.1f, 0.3f, hitEffectPrefab);
        attackCollider.SetActive(true);
    }

    public void DisableAttackCollider()
    {
        attackCollider.SetActive(false);
    }

    public void CastSpell()
    {
        Instantiate(fireballPrefab, transform.position + transform.forward * 0.2f + transform.up * 0.3f, transform.rotation);
    }
}
