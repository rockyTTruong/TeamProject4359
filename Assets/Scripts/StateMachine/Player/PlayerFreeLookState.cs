using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        psm.currentState = PlayerStates.Idle;
        InputReader.Instance.buttonPress[(int)GamePadButton.WestButton] += NormalAttack;
        InputReader.Instance.buttonLongPress[(int)GamePadButton.WestButton] += ChargeAttack;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] += Jump;
        InputReader.Instance.buttonPress[(int)GamePadButton.EastButton] += Dodge;
        InputReader.Instance.buttonPress[(int)GamePadButton.LeftStickPress] += Dash;
        InputReader.Instance.buttonPress[(int)GamePadButton.NorthButton] += TryInteract;
        InputReader.Instance.buttonPress[(int)GamePadButton.RightStickPress] += SpanCameraFaceTarget;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] += LockOnMode;
        InputReader.Instance.buttonPress[(int)GamePadButton.RightShoulder] += Block;
        PlayAnimation(freelookHash, crossFixedDuration);
    }

    public override void Exit()
    {
        InputReader.Instance.buttonPress[(int)GamePadButton.WestButton] -= NormalAttack;
        InputReader.Instance.buttonLongPress[(int)GamePadButton.WestButton] -= ChargeAttack;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] -= Jump;
        InputReader.Instance.buttonPress[(int)GamePadButton.EastButton] -= Dodge;
        InputReader.Instance.buttonPress[(int)GamePadButton.LeftStickPress] -= Dash;
        InputReader.Instance.buttonPress[(int)GamePadButton.NorthButton] -= TryInteract;
        InputReader.Instance.buttonPress[(int)GamePadButton.RightStickPress] -= SpanCameraFaceTarget;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] -= LockOnMode;
        InputReader.Instance.buttonPress[(int)GamePadButton.RightShoulder] -= Block;
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

        if (psm.isDashing)
        {
            if (!psm.character.TryUseStamina(PlayerActionCost.runAction)) psm.isDashing = false;
        }

        if (psm.character.CurrentStamina == 0f)
        {
            psm.isDashing = false;
            psm.SwitchState(new PlayerExhaustionState(psm));
        }

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
        }
        else StrongAttack();
    }

    private void UpdateAnimator()
    {
        if (InputReader.Instance.leftStickValue == Vector2.zero)
        {
            psm.currentState = PlayerStates.Idle;
            psm.animator.SetFloat(blendSpeedHash, 0f, 0.1f, Time.deltaTime);
            return;
        }
        if (psm.walkMode)
        {
            blendValue = 0.5f;
            psm.currentState = PlayerStates.Walking;
        }
        else
        {
            blendValue = Mathf.Max(Mathf.Abs(InputReader.Instance.leftStickValue.x), Mathf.Abs(InputReader.Instance.leftStickValue.y));
            if (blendValue > 0.7f)
            {
                blendValue = 1f;
                psm.currentState = PlayerStates.Running;
            }
            else
            {
                blendValue = 0.5f;
                psm.currentState = PlayerStates.Walking;
            }
        }

        if (psm.isDashing)
        {
            psm.currentState = PlayerStates.Dashing;
        }
        psm.animator.SetFloat(blendSpeedHash, blendValue, 0.1f, Time.deltaTime);
    }

    private void NormalAttack()
    {
        if (!psm.character.TryUseStamina(PlayerActionCost.attackAction)) return;
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
        if (!psm.character.TryUseStamina(PlayerActionCost.chargeAttackAction)) return;
        psm.isDashing = false;
        WeaponType weaponType = psm.character.GetCurrentWeaponData().weaponType;
        if (weaponType == WeaponType.Sword)
        {
            psm.SwitchState(new PlayerChargeAttackingState(psm));
        }
        else return;
    }

    private void StrongAttack()
    {
        if (!psm.character.TryUseStamina(PlayerActionCost.strongAttackAction)) return;
        psm.isDashing = false;
        WeaponType weaponType = psm.character.GetCurrentWeaponData().weaponType;
        if (weaponType == WeaponType.Sword)
        {
            psm.SwitchState(new PlayerAttackingState(psm, psm.comboManager.strongSwordAttackCombo, 0));
        }
        else return;
    }

    private void Dash()
    {
        psm.isDashing = true;
    }

    private void Cheat()
    {
        InventoryBox.Instance.AddItem("9999", 100);
        GameObject.FindObjectOfType<QuickSlotManager>().UpdateUI();
        CoinManager.Instance.UpdateUI();
    }
}
