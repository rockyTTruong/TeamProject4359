using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWizardZone : MonoBehaviour
{
    public Scene loadScene;
    public GameObject startPosition;

    private GameObject player;
    private PlayerStateMachine psm;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = startPosition.transform.position;
        player.transform.rotation = startPosition.transform.rotation;
        psm = player.GetComponent<PlayerStateMachine>(); 
        psm.savePosition = player.transform.position;
        psm.saveRotation = player.transform.rotation;
        psm.saveScene = loadScene;
    }
}
