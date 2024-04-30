using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNextTrigger : MonoBehaviour
{
    public GameObject nextCollider;
    public GameObject portalToEnable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nextCollider.SetActive(true);
            portalToEnable.SetActive(true);
        }
    }
}
