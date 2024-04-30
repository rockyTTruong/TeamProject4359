using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class IcicleSpell : MonoBehaviour
{
    public List<GameObject> icicles;
    public float interval = 1f;
    public float followTime;

    private GameObject player;
    private float timer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); 
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
        StartCoroutine(SpawnIcicle());
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer <= followTime)
        {
            FollowTarget();
        }
        if (transform.childCount == 0) Destroy(gameObject);
    }

    private IEnumerator SpawnIcicle()
    {
        foreach (GameObject icicle in icicles)
        {
            icicle.SetActive(true);
            yield return new WaitForSeconds(interval);
        }
    }

    private void FollowTarget()
    {
        transform.position = player.transform.position;
    }
}
