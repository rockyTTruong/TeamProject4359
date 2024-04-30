using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    // Start is called before the first frame update
    Animator _doorAnim;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            _doorAnim.SetBool("isOpening", true);
            Destroy(gameObject);
        }
    }
    void Start()
    {
        _doorAnim = this.transform.parent.GetComponent<Animator>();
    }
}
