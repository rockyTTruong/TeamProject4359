using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
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

    public Transform mainCameraTransform;
    public GameObject swordMainHand;
    public GameObject bowMainHand;
    public GameObject swordBack;
    public GameObject bowBack;
    public AudioSource footstepSource;
    public string currentItemGuid = "1001";
    public bool isJumping;
    public bool isFalling;
    public Vector3 savePosition;
    public Quaternion saveRotation;
    public string saveScene;
    public GameObject retryMenuUI;

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

        savePosition = transform.position;
        saveRotation = transform.rotation;
        SwitchState(new PlayerFreeLookState(this));
    }

    public void OnDamage()
    {
        SwitchState(new PlayerImpactState(this));
    }

    public void OnDie(GameObject dieCharacter)
    {
        if (dieCharacter != this.gameObject) return;
        SwitchState(new PlayerDieState(this));
    }

    private void ApplySlide()
    {
        slideDirection = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized * slideSpeed;
    }
}
