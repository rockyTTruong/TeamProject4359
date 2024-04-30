using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestTriggerEndEnemy : MonoBehaviour
{
    public int questEnd;
    public GameObject colliderToEnable;

    [SerializeField] private Character character;

    private void Start()
    {
        character.DieEvent += EndQuestOnDeath;
    }

    public void EndQuestOnDeath(GameObject enemyGameObject)
    {
        if (colliderToEnable != null)
        {
            colliderToEnable.SetActive(true);
        }
        if (enemyGameObject.CompareTag("Enemy") && !QuestDatabase.Instance.questDatabase[questEnd].completed)
        {
            QuestA2Manager.Instance.finishQuest(questEnd);

        }
    }
    private void OnDestroy()
    {
        character.DieEvent -= EndQuestOnDeath;
    }
}