using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHandler : MonoBehaviour
{
    [SerializeField] private List<IInteractable> interactableList = new List<IInteractable>();

    public void Subscribe(IInteractable interactable)
    {
        interactableList.Add(interactable);
    }

    public void Unsubscribe(IInteractable interactable)
    {
        if (interactableList.Contains(interactable))
            interactableList.Remove(interactable);
    }

    public bool TryInteract(PlayerStateMachine psm)
    {
        if (interactableList.Count == 0) return false;
        interactableList[0].Interact(psm);
        return true;
    }
}
