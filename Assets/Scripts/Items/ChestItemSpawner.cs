using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestItemSpawner : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;
    [SerializeField] List<GameObject> dropItemPrefabs;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private GameObject interactableUIIndicator;

    private bool isOpened;

    public void Interact(PlayerStateMachine psm)
    {
        if (isOpened) return;
        animator.CrossFadeInFixedTime("Open", 0.1f);
        DropItem();
        isOpened = true; 
        interactableUIIndicator.SetActive(false);
    }

    private void DropItem()
    {
        StartCoroutine(DropCoroutine());
    }

    private IEnumerator DropCoroutine()
    {
        for (int i = 0; i < dropItemPrefabs.Count(); i++)
        {
            Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomPosition.x, 0f, randomPosition.y) + transform.position;

            GameObject dropitem = dropItemPrefabs[i];
            dropitem.transform.position = spawnPosition;

            GameObject newDropItem = Instantiate(dropitem);
            newDropItem.GetComponent<ItemDropData>().TriggerAutoCollect();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOpened) return;
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
