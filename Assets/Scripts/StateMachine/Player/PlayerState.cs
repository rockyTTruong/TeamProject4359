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

    public void HandleFrontLookMovement()
    {
        Vector3 movement = CalculateMovement();
        Move(movement * (psm.walkSpeed));

        Vector3 forward = psm.mainCameraTransform.forward;
        forward.y = psm.transform.forward.y;
        psm.transform.forward = forward;
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
        if (psm.character.TryUseStamina(PlayerActionCost.dodgeAction))
            psm.SwitchState(new PlayerDodgingState(psm));
    }

    public void Block()
    {
        if (InventoryBox.Instance.CheckInventory("5004").quantity == 0) return;
        if (psm.character.GetCurrentWeaponData().weaponType == WeaponType.Bow) return;
        psm.SwitchState(new PlayerBlockingState(psm));
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
}
