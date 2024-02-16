using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : State
{
    public PlayerStateMachine psm;

    public PlayerState(PlayerStateMachine playerStateMachine)
    {
        this.psm = playerStateMachine;
    }

    public void Move(Vector3 movement)
    {
        psm.controller.Move((movement + psm.forceReceiver.GetForce() + psm.groundChecker.slidingForce) * Time.deltaTime);
    }

    public Vector3 CalculateMovement()
    {
        Vector2 movementInput = InputReader.Instance.leftStickValue;
        Vector3 forward = psm.mainCameraTransform.forward;
        Vector3 right = psm.mainCameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        return forward * movementInput.y + right * movementInput.x;
    }

    public void HandlePlayerMovement()
    {
        Vector3 movement = CalculateMovement();
        if (InputReader.Instance.leftStickValue == Vector2.zero)
        {
            psm.isDashing = false;
            Move(Vector3.zero);
        }
        else
        {
            if (psm.walkMode) Move(movement * psm.walkSpeed);
            else Move(movement * (psm.isDashing ? psm.dashSpeed : psm.movementSpeed));
            ChangeDirection(movement);
        }
    }

    public void FaceTarget(GameObject target)
    {
        Vector3 lookPos = target.transform.position - psm.transform.position;
        lookPos.y = 0f;
        Quaternion lookRotation = Quaternion.LookRotation(lookPos);
        psm.transform.rotation = Quaternion.Slerp(psm.transform.rotation, lookRotation, psm.changeDirectionSpeed * Time.deltaTime);
    }

    public void FaceTargetInstantly(GameObject target)
    {
        Vector3 lookPos = target.transform.position - psm.transform.position;
        lookPos.y = 0f;
        psm.transform.rotation = Quaternion.LookRotation(lookPos);
    }

    public void ChangeDirection(Vector3 movement)
    {
        psm.transform.rotation = Quaternion.Lerp(psm.transform.rotation, Quaternion.LookRotation(movement), psm.changeDirectionSpeed * Time.deltaTime);
    }

    public void ChangeDirectionInstantly(Vector3 movement)
    {
        psm.transform.rotation = Quaternion.LookRotation(movement);
    }

    public void HandleCameraMovement()
    {
        if (InputReader.Instance.rightStickValue != Vector2.zero)
        {
            CameraController.Instance.RotateCamera();
        }
        if (InputReader.Instance.buttonHold[(int)GamePadButton.LeftTrigger])
        {
            CameraController.Instance.ZoomOut();
        }
        if (InputReader.Instance.buttonHold[(int)GamePadButton.RightTrigger])
        {
            CameraController.Instance.ZoomIn();
        }
    }
    public void Dodge()
    {
        psm.SwitchState(new PlayerDodgingState(psm));
    }

    public void Jump()
    {
        psm.SwitchState(new PlayerJumpState(psm));
    }

    public void PlayAnimation(int animationHash, float crossFixedDuration)
    {
        psm.animator.CrossFadeInFixedTime(animationHash, crossFixedDuration);
    }

    public void LockOnMode()
    {
        if (psm.targetManager.GetCurrentTarget() == null)
        {
            if (psm.targetManager.TryLockOn())
                SpanCameraFaceTarget();
        }
        else
        {
            //playerStateMachine.targetManager.DisableLockOn();
            LockOnNextTarget();
        }
    }

    public void LockOnNextTarget()
    {
        if (psm.targetManager.GetCurrentTarget() == null) return;
        psm.targetManager.NextTarget();
        SpanCameraFaceTarget();
    }

    public void LockOnPreviousTarget()
    {
        if (psm.targetManager.GetCurrentTarget() == null) return;
        psm.targetManager.PreviouTarget();
        SpanCameraFaceTarget();
    }

    public void SpanCameraFaceTarget()
    {
        if (psm.targetManager.GetCurrentTarget() == null) return;
        Vector3 direction = psm.targetManager.GetCurrentTarget().transform.position - psm.transform.position;
        float cameraAngle = Quaternion.FromToRotation(Vector3.forward, direction).eulerAngles.y;
        CameraController.Instance.SpanCamera(cameraAngle);
    }

    public void QuickSwitchWeapon()
    {
        WeaponItemData currentWeapon = psm.character.GetCurrentWeaponData();
        if (currentWeapon.weaponType == WeaponType.Sword)
        {
            if (InventoryBox.Instance.CheckInventory("5002") == null)
            {
                Debug.Log($"You don't have a bow yet.");
                return;
            }
            Debug.Log($"Switch to Bow.");
            psm.character.ChangeWeapon("5002");
            psm.swordMainHand.SetActive(false);
            psm.bowBack.SetActive(false);
            psm.swordBack.SetActive(true);
            psm.bowMainHand.SetActive(true);
        }
        else if (currentWeapon.weaponType == WeaponType.Bow)
        {
            Debug.Log($"Switch to Sword.");
            psm.character.ChangeWeapon("5001");
            psm.swordMainHand.SetActive(true);
            psm.bowBack.SetActive(true);
            psm.swordBack.SetActive(false);
            psm.bowMainHand.SetActive(false);
        }
        EventHandler.OnSwitchWeaponEvent();
    }
}
