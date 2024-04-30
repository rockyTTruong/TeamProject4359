using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardStateMachine : StateMachine
{
    [Header("Movement Parameters")]
    public float changeDirectionSpeed = 15;
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

    public bool targetInRange;
    public float targetDistance;
    public GameObject fireball2Prefab;
    public GameObject iciclePrefab;
    public GameObject thunderPrefab;
    public GameObject teleportPrefab;

    public List<GameObject> teleportPosition;
    public int currentPositionIndex;

    public void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        forceReceiver = GetComponent<ForceReceiver>();
        character = GetComponent<Character>();
        targetManager = GetComponentInChildren<TargetManager>();
        character.DamageEvent += OnDamage;
        character.DieEvent += OnDie;

        SwitchState(new WizardIdleState(this));
    }

    public void OnDamage()
    {
        if (!character.isUnflinching) SwitchState(new WizardImpactState(this));
    }

    public void OnDie(GameObject dieCharacter)
    {
        SwitchState(new WizardDieState(this));
    }

    public void CastSpell(string spellName)
    {
        switch(spellName)
        {
            case "Fireball":
                Instantiate(fireball2Prefab, transform.position + transform.forward * 0.35f + transform.up * 0.3f, transform.rotation);
                return;

            case "Icicle":
                Instantiate(iciclePrefab);
                return;

            case "Thunder":
                Instantiate(thunderPrefab);
                return;

            case "Teleport":
                teleportPrefab.SetActive(false);
                teleportPrefab.SetActive(true);
                return;

            default:
                return;
        }
    }
}
