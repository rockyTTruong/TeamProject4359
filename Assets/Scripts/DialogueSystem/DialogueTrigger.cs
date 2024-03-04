using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private bool autoTrigger;
    [SerializeField] private GameObject interactableUIIndicator;
    [SerializeField] private List<DialogueData> dialogues = new List<DialogueData>();
    [SerializeField] private float facePlayerSpeed;

    private Coroutine facingPlayer;

    public void Interact(PlayerStateMachine psm)
    {
        if (dialogues.Count == 0) return;

        psm.SwitchState(new PlayerTalkingState(psm));

        FacePlayer(psm);

        for (int i = 0; i < dialogues.Count; i++)
        {
            if (dialogues[i].MeetRequirement())
            {
                DialogueManager.Instance.StartDialogue(dialogues[i]);
                if (autoTrigger) this.gameObject.SetActive(false);
                return;
            }
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine psm))
        {
            if (autoTrigger)
            {
                Interact(psm);
                return;
            }
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
