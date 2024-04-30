using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardBossSpawner : MonoBehaviour
{
    public GameObject wizard;
    public GameObject teleportPositions;
    public List<GameObject> slimes;

    private void Start()
    {
        wizard.SetActive(false);
        teleportPositions.SetActive(false);
    }

    public void ReportKill(GameObject enemy)
    {
        if (slimes.Contains(enemy))
        {
            slimes.Remove(enemy);
            if (slimes.Count == 0)
            {
                teleportPositions.SetActive(true);
                Invoke(nameof(SpawnWizard), 3f);
            }
        }
    }

    private void SpawnWizard()
    {
        wizard.SetActive(true);
    }
}
