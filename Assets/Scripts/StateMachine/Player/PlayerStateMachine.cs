using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStates
{
    Idle, Walking, Running, Dashing, Jumping, Falling, Die,
    Dodging, Attacking, Hit, Exhausting, Blocking, BlockImpact,
    Talking, Shopping
}

public class PlayerStateMachine : StateMachine
{
    public PlayerStates currentState;

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
    public InteractableHandler interactableHandler;

    private Vector3 groundNormal;

    public Transform mainCameraTransform;
    public GameObject swordMainHand;
    public GameObject bowMainHand;
    public GameObject swordBack;
    public GameObject bowBack;
    public GameObject shield;
    public GameObject shieldBack;
    public AudioSource footstepSource;
    public string currentItemGuid = "1001";
    public bool isJumping;
    public bool isFalling;
    public Vector3 savePosition;
    public Quaternion saveRotation;
    public Scene saveScene;
    public GameObject retryMenuUI;
    public GameObject blockEffectPrefab;

    public void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        forceReceiver = GetComponent<ForceReceiver>();
        comboManager = GetComponent<ComboManager>();
        character = GetComponent<Character>();
        targetManager = GetComponentInChildren<TargetManager>();
        groundChecker = GetComponent<GroundChecker>();
        interactableHandler = GetComponent<InteractableHandler>();
        character.DamageEvent += OnDamage;
        character.DieEvent += OnDie;
        character.BlockEvent += OnBlock;

        savePosition = transform.position;
        saveRotation = transform.rotation;
        SwitchState(new PlayerFreeLookState(this));

        InputReader.Instance.buttonPress[(int)GamePadButton.DpadUp] += UseItem;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadRight] += SwitchItem;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadLeft] += QuickSwitchWeapon;
    }
    private void OnApplicationQuit()
    {
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadUp] -= UseItem;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadRight] -= SwitchItem;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadLeft] -= QuickSwitchWeapon;
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

    public void OnBlock()
    {
        SwitchState(new PlayerBlockingImpactState(this));
    }

    private void ApplySlide()
    {
        slideDirection = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized * slideSpeed;
    }

    public void QuickSwitchWeapon()
    {
        if (currentState == PlayerStates.Die) return;
        if (currentState == PlayerStates.Talking) return;
        if (currentState == PlayerStates.Shopping) return;

        WeaponItemData currentWeapon = character.GetCurrentWeaponData();
        if (currentWeapon.weaponType == WeaponType.Sword)
        {
            if (InventoryBox.Instance.CheckInventory("5002").quantity == 0)
            {
                return;
            }
            character.ChangeWeapon("5002");
            swordMainHand.SetActive(false);
            bowBack.SetActive(false);
            swordBack.SetActive(true);
            bowMainHand.SetActive(true);
            if (InventoryBox.Instance.CheckInventory("5004").quantity != 0)
            {
                shieldBack.SetActive(true);
                shield.SetActive(false);
            }
        }
        else if (currentWeapon.weaponType == WeaponType.Bow)
        {
            character.ChangeWeapon("5001");
            swordMainHand.SetActive(true);
            bowBack.SetActive(true);
            swordBack.SetActive(false);
            bowMainHand.SetActive(false);
            if (InventoryBox.Instance.CheckInventory("5004").quantity != 0)
            {
                shield.SetActive(true);
                shieldBack.SetActive(false);
            }
        }
        EventHandler.OnSwitchWeaponEvent();
    }

    public void UseItem()
    {
        if (currentState == PlayerStates.Die) return;
        if (currentState == PlayerStates.Talking) return;
        if (currentState == PlayerStates.Shopping) return;

        string itemGuid = currentItemGuid;
        Debug.Log($"Try using item {itemGuid}");
        if (InventoryBox.Instance.RemoveItem(itemGuid, 1))
        {
            ConsumableItemData consumableItem = (ConsumableItemData)ItemDatabase.Instance.GetItemData(itemGuid);
            consumableItem.Use(gameObject);
            EventHandler.OnUseItemEvent(itemGuid);
        }
    }

    public void SwitchItem()
    {
        if (currentState == PlayerStates.Die) return;
        if (currentState == PlayerStates.Talking) return;
        if (currentState == PlayerStates.Shopping) return;

        if (currentItemGuid == "1001")
        {
            currentItemGuid = "1002";
            GameObject.FindObjectOfType<QuickSlotManager>().UpdateCurrentItemInfo("1002");

        }
        else if (currentItemGuid == "1002")
        {
            currentItemGuid = "1003";
            GameObject.FindObjectOfType<QuickSlotManager>().UpdateCurrentItemInfo("1003");
        }
        else if (currentItemGuid == "1003")
        {
            currentItemGuid = "1001";
            GameObject.FindObjectOfType<QuickSlotManager>().UpdateCurrentItemInfo("1001");
        }
    }
}
