using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private int experience;
    [SerializeField] List<GameObject> dropItemPrefabs;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private float spawnRadius = 2f;

    private void Start()
    {
        character.DieEvent += DropItem;
    }

    private void DropItem(GameObject thisGameObject)
    {
        if (thisGameObject == gameObject) StartCoroutine(DropCoroutine());
    }

    private IEnumerator DropCoroutine()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>().character.GainExp(experience);
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
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        character.DieEvent -= DropItem;
    }
}
