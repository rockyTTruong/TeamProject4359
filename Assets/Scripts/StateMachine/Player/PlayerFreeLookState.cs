using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFreeLookState : PlayerState
{
    public PlayerFreeLookState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    private int freelookHash = Animator.StringToHash("FreeLookBlendTree");
    private int blendSpeedHash = Animator.StringToHash("FreeLookBlendSpeed");
    private float crossFixedDuration = 0.1f;
    private float blendValue;
    private float footstepInterval;

    public override void Enter()
    {
        InputReader.Instance.buttonPress[(int)GamePadButton.WestButton] += NormalAttack;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadUp] += UseItem;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadRight] += SwitchItem;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadLeft] += QuickSwitchWeapon;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] += TryInteract;
        InputReader.Instance.buttonPress[(int)GamePadButton.EastButton] += Dodge;
        InputReader.Instance.buttonPress[(int)GamePadButton.LeftStickPress] += Dash;
        InputReader.Instance.buttonPress[(int)GamePadButton.NorthButton] += StrongAttack;
        InputReader.Instance.buttonLongPress[(int)GamePadButton.DpadUp] += ChargeAttack;
        InputReader.Instance.buttonPress[(int)GamePadButton.RightStickPress] += SpanCameraFaceTarget;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] += LockOnMode;
        PlayAnimation(freelookHash, crossFixedDuration);
    }

    public override void Exit()
    {
        InputReader.Instance.buttonPress[(int)GamePadButton.WestButton] -= NormalAttack;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadUp] -= UseItem;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadRight] -= SwitchItem;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadLeft] -= QuickSwitchWeapon;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] -= TryInteract;
        InputReader.Instance.buttonPress[(int)GamePadButton.EastButton] -= Dodge;
        InputReader.Instance.buttonPress[(int)GamePadButton.LeftStickPress] -= Dash;
        InputReader.Instance.buttonPress[(int)GamePadButton.NorthButton] -= StrongAttack;
        InputReader.Instance.buttonLongPress[(int)GamePadButton.DpadUp] -= ChargeAttack;
        InputReader.Instance.buttonPress[(int)GamePadButton.RightStickPress] -= SpanCameraFaceTarget;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] -= LockOnMode;
    }

    public override void Tick()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Cheat();
        }
        UpdateAnimator();
        HandleCameraMovement();
        HandlePlayerMovement();
        if (!psm.groundChecker.IsGrounded) psm.SwitchState(new PlayerFallingState(psm));

        footstepInterval += Time.deltaTime;
        if (InputReader.Instance.leftStickValue == Vector2.zero) return;
        if (psm.walkMode || Mathf.Max(Mathf.Abs(InputReader.Instance.leftStickValue.x), Mathf.Abs(InputReader.Instance.leftStickValue.y)) < 0.7f)
        {
            if (footstepInterval > 0.9f)
            {
                if (!psm.footstepSource.isPlaying) psm.footstepSource.Play();
                footstepInterval -= 0.9f;
            }
        }
        else if (psm.isDashing)
        {
            if (footstepInterval > 0.1f)
            {
                if (!psm.footstepSource.isPlaying) psm.footstepSource.Play();
                footstepInterval -= 0.1f;
            }
        }
        else
        {
            if (footstepInterval > 0.2f)
            {
                if (!psm.footstepSource.isPlaying) psm.footstepSource.Play();
                footstepInterval -= 0.2f;
            }
        }
    }

    private void TryInteract()
    {
        if (psm.interactableHandler.TryInteract(psm))
        {
            psm.animator.SetFloat(blendSpeedHash, 0f);
            psm.SwitchState(new PlayerTalkingState(psm));
        }
        else Jump();
    }

    private void UpdateAnimator()
    {
        if (InputReader.Instance.leftStickValue == Vector2.zero)
        {
            psm.animator.SetFloat(blendSpeedHash, 0f, 0.1f, Time.deltaTime);
            return;
        }
        if (psm.walkMode)
        {
            blendValue = 0.5f;
        }
        else
        {
            blendValue = Mathf.Max(Mathf.Abs(InputReader.Instance.leftStickValue.x), Mathf.Abs(InputReader.Instance.leftStickValue.y));
            if (blendValue > 0.7f) blendValue = 1f;
            else blendValue = 0.5f;
        }

        psm.animator.SetFloat(blendSpeedHash, blendValue, 0.1f, Time.deltaTime);
    }

    private void NormalAttack()
    {
        psm.isDashing = false;
        WeaponType weaponType = psm.character.GetCurrentWeaponData().weaponType;
        if (weaponType == WeaponType.Sword)
        {
            psm.SwitchState(new PlayerAttackingState(psm, psm.comboManager.normalSwordCombo, 0));
        }
        else if (weaponType == WeaponType.Bow)
        {
            InventorySlot arrowSlot = InventoryBox.Instance.CheckInventory("5003");
            if (arrowSlot == null || arrowSlot.quantity < 1)
            {
                Debug.Log($"Out of arrow.");
                return;
            }
            InventoryBox.Instance.RemoveItem("5003", 1);
            EventHandler.OnUseItemEvent("5003");
            psm.SwitchState(new PlayerAttackingState(psm, psm.comboManager.normalBowCombo, 0));
        }
        else return;
    }

    private void ChargeAttack()
    {
        psm.isDashing = false;
        WeaponType weaponType = psm.character.GetCurrentWeaponData().weaponType;
        if (weaponType == WeaponType.Sword)
        {
            /*
            playerStateMachine.swordMainHand.SetActive(true);
            playerStateMachine.bowBack.SetActive(true);
            playerStateMachine.swordBack.SetActive(false);
            playerStateMachine.bowMainHand.SetActive(false);*/
            psm.SwitchState(new PlayerChargeAttackingState(psm));
        }
        else return;
    }

    private void StrongAttack()
    {
        psm.isDashing = false;
        WeaponType weaponType = psm.character.GetCurrentWeaponData().weaponType;
        if (weaponType == WeaponType.Sword)
        {/*
            playerStateMachine.swordMainHand.SetActive(true);
            playerStateMachine.bowBack.SetActive(true);
            playerStateMachine.swordBack.SetActive(false);
            playerStateMachine.bowMainHand.SetActive(false);*/
            psm.SwitchState(new PlayerAttackingState(psm, psm.comboManager.strongSwordAttackCombo, 0));
        }
        else return;
    }

    private void Dash()
    {
        psm.isDashing = true;
    }

    private void UseItem()
    {
        string itemGuid = psm.currentItemGuid;
        Debug.Log($"Try using item {itemGuid}");
        if (InventoryBox.Instance.RemoveItem(itemGuid, 1))
        {
            ConsumableItemData consumableItem = (ConsumableItemData)ItemDatabase.Instance.GetItemData(itemGuid);
            consumableItem.Use(psm.gameObject);
            EventHandler.OnUseItemEvent(itemGuid);
        }
    }

    private void SwitchItem()
    {
        if (psm.currentItemGuid == "1001")
        {
            psm.currentItemGuid = "1002";
            GameObject.FindObjectOfType<QuickSlotManager>().UpdateCurrentItemInfo("1002");

        }
        else if (psm.currentItemGuid == "1002")
        {
            psm.currentItemGuid = "1001";
            GameObject.FindObjectOfType<QuickSlotManager>().UpdateCurrentItemInfo("1001");

        }
        
        Debug.Log($"Current Item {psm.currentItemGuid}");
    }

    private void Cheat()
    {
        InventoryBox.Instance.AddItem("9999", 10);
        InventoryBox.Instance.AddItem("9998", 10);
        InventoryBox.Instance.AddItem("9997", 10);
        GameObject.FindObjectOfType<QuickSlotManager>().UpdateUI();
        CoinManager.Instance.UpdateUI();
    }
}
