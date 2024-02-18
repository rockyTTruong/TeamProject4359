using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackingState : PlayerState
{
    public PlayerAttackingState(PlayerStateMachine playerStateMachine, Combo combo, int attackIndex) : base(playerStateMachine)
    {
        this.attack = combo.attack[attackIndex];
        this.combo = combo;
        attackHash = Animator.StringToHash(attack.animationName);
    }

    private Attack attack;
    private Combo combo;
    private float normalizedTime;
    private int attackHash;

    public override void Enter()
    {
        PlayAnimation(attackHash, attack.transitionDuration);
        InputReader.Instance.buttonPress[(int)GamePadButton.WestButton] += TryComboNormalAttack;
        InputReader.Instance.buttonLongPress[(int)GamePadButton.WestButton] += ChargeAttack;
        InputReader.Instance.buttonPress[(int)GamePadButton.NorthButton] += StrongAttack;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] += LockOnMode;
        InputReader.Instance.buttonPress[(int)GamePadButton.EastButton] += Dodge;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] += Jump;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadLeft] += QuickSwitchWeapon;
    }

    public override void Exit()
    {
        InputReader.Instance.buttonPress[(int)GamePadButton.WestButton] -= TryComboNormalAttack;
        InputReader.Instance.buttonLongPress[(int)GamePadButton.WestButton] -= ChargeAttack;
        InputReader.Instance.buttonPress[(int)GamePadButton.NorthButton] -= StrongAttack;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadDown] -= LockOnMode;
        InputReader.Instance.buttonPress[(int)GamePadButton.EastButton] -= Dodge;
        InputReader.Instance.buttonPress[(int)GamePadButton.SouthButton] -= Jump;
        InputReader.Instance.buttonPress[(int)GamePadButton.DpadLeft] -= QuickSwitchWeapon;
    }

    public override void Tick()
    {
        HandleCameraMovement();
        GameObject target = psm.targetManager.GetCurrentTarget();
        if (target == null)
        {
            target = psm.targetManager.GetNearestTarget();
        }
        if (target != null)
        {
            if (psm.character.GetCurrentWeaponData().weaponType == WeaponType.Sword)
            {
                if (Vector3.Distance(target.transform.position, psm.transform.position) > 1.5f)
                {
                    Move(psm.transform.forward);
                }
            }
            FaceTargetInstantly(target);
        }

        normalizedTime = GetNormalizedTime(psm.animator, attackHash);
        if (normalizedTime >= attack.moveStartTime && normalizedTime < attack.moveEndTime)
        {
            Move(psm.transform.forward * attack.moveForwardDistance);
        }

        if (normalizedTime >= 1f)
        {
            psm.SwitchState(new PlayerFreeLookState(psm));
        }
    }

    private void TryComboNormalAttack()
    {
        if (attack.nextComboIndex == -1) return;
        if (normalizedTime < attack.nextComboEnableTime) return;
        if (!psm.character.TryUseStamina(PlayerActionCost.attackAction)) return;

        WeaponType weaponType = psm.character.GetCurrentWeaponData().weaponType;
        if (weaponType == WeaponType.Sword)
        {
            if (combo == psm.comboManager.normalSwordCombo)
            {
                psm.SwitchState(new PlayerAttackingState(psm, combo, attack.nextComboIndex));
            }
            else 
            { 
                psm.SwitchState(new PlayerAttackingState(psm, psm.comboManager.normalSwordCombo, 0)); 
            }
                   
        }
        else
        if (weaponType == WeaponType.Bow)
        {
            InventorySlot arrowSlot = InventoryBox.Instance.CheckInventory("5003");
            if (arrowSlot == null || arrowSlot.quantity < 1)
            {
                Debug.Log($"Out of arrow.");
                return;
            }
            InventoryBox.Instance.RemoveItem("5003", 1);
            EventHandler.OnUseItemEvent("5003");
            if (combo == psm.comboManager.normalBowCombo)
            {
                psm.SwitchState(new PlayerAttackingState(psm, combo, attack.nextComboIndex));
            }
            else
            {
                psm.SwitchState(new PlayerAttackingState(psm, psm.comboManager.normalBowCombo, 0));
            }
        }
    }

    private void ChargeAttack()
    {
        if (!psm.character.TryUseStamina(PlayerActionCost.chargeAttackAction)) return;
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
        if (attack.nextStrongComboIndex == -1) return;
        if (normalizedTime < attack.nextComboEnableTime) return;
        if (!psm.character.TryUseStamina(PlayerActionCost.strongAttackAction)) return;

        WeaponType weaponType = psm.character.GetCurrentWeaponData().weaponType;
        if (weaponType == WeaponType.Sword)
        {
            /*
            playerStateMachine.swordMainHand.SetActive(true);
            playerStateMachine.bowBack.SetActive(true);
            playerStateMachine.swordBack.SetActive(false);
            playerStateMachine.bowMainHand.SetActive(false);*/
            psm.SwitchState(new PlayerAttackingState(psm, psm.comboManager.strongSwordAttackCombo, attack.nextStrongComboIndex));
        }
        else return;
    }
}
