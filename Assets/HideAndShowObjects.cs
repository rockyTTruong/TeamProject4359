using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAndShowObjects : MonoBehaviour
{
    public GameObject showObject;
    public GameObject hideObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (showObject != null)
                showObject.SetActive(true);
            if (hideObject != null)
                hideObject.SetActive(false);
        }
    }
}
