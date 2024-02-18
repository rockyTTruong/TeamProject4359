using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(SphereCollider))]
public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject interactableUIIndicator;
    [SerializeField] private List<DialogueData> dialogues = new List<DialogueData>();
    [SerializeField] private float facePlayerSpeed;

    private Coroutine facingPlayer;

    public void Interact(PlayerStateMachine psm)
    {
        FacePlayer(psm);
        DialogueManager.Instance.StartDialogue(dialogues[0].textJson, dialogues[0].playableDirector);
    }

    public void FacePlayer(PlayerStateMachine psm)
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
