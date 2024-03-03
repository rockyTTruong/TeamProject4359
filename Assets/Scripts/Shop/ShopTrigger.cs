using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(SphereCollider))]
public class ShopTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject interactableUIIndicator;
    [SerializeField] private GameObject shopMenu;
    [SerializeField] private DialogueData shopStartDialogue;
    [SerializeField] private float facePlayerSpeed;
    private Coroutine facingPlayer;

    public void Interact(PlayerStateMachine psm)
    {
        FacePlayer(psm);
        DialogueManager.Instance.StartDialogue(shopStartDialogue);
        DialogueManager.Instance.ActionAfterDialogue(OpenShopMenu);
    }

    private void FacePlayer(PlayerStateMachine psm)
    {
        if (facingPlayer != null)
        {
            StopCoroutine(facingPlayer);
            facingPlayer = null;
        }
        facingPlayer = StartCoroutine(FacePlayerCoroutine(psm));
    }

    private IEnumerator FacePlayerCoroutine(PlayerStateMachine psm)
    {
        Vector3 lookDirection = psm.transform.position - transform.position;
        lookDirection.y = 0f;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        float angle = Quaternion.Angle(lookRotation, transform.rotation);

        while (angle > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * facePlayerSpeed);
            angle = Quaternion.Angle(lookRotation, transform.rotation);
            yield return null;
        }
        facingPlayer = null;
    }

    private void OpenShopMenu()
    {
        PlayerStateMachine psm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        psm.SwitchState(new PlayerShoppingState(psm));

        Time.timeScale = 0.0f;
        shopMenu.SetActive(true);
        InputReader.Instance.buttonPress[(int)GamePadButton.EastButton] += CloseShopMenu;
    }

    private void CloseShopMenu()
    {
        StartCoroutine(CloseShopMenuCoroutine());
    }

    private IEnumerator CloseShopMenuCoroutine()
    {
        InputReader.Instance.buttonPress[(int)GamePadButton.EastButton] -= CloseShopMenu;
        shopMenu.SetActive(false);
        Time.timeScale = 1.0f;

        yield return new WaitForSecondsRealtime(0.5f);
        PlayerStateMachine psm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        psm.SwitchState(new PlayerFreeLookState(psm));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine psm))
        {
            interactableUIIndicator.SetActive(true);
            psm.interactableHandler.Subscribe(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine psm))
        {
            interactableUIIndicator.SetActive(false);
            psm.interactableHandler.Unsubscribe(this);
        }
    }
}
