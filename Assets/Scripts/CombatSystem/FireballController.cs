using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class FireballController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float changeDirectionSpeed = 5f;
    [SerializeField] private float lifeTime = 3.5f;
    [SerializeField] private float stopChaseTime = 3f;
    [SerializeField] private float[] stanbyTimeRange = new float[2];
    [SerializeField] private int damage = 20;
    [SerializeField] private float knockback;
    [SerializeField] private float launchForce;
    [SerializeField] private float hitLagDuration;
    [SerializeField] private float hitLagStrength;
    [SerializeField] private GameObject hitEffectPrefab;

    private GameObject player;
    private float timer;
    private float chaseTime;
    private bool chasing;

    private void Start()
    {
        chasing = true;
        chaseTime = Random.Range(stanbyTimeRange[0], stanbyTimeRange[1]);
        controller = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player");
        Invoke(nameof(DestroySelf), lifeTime);
        GetComponent<AttackCollider>().SetAttack(damage, knockback, launchForce, hitLagDuration, hitLagStrength, hitEffectPrefab);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < chaseTime) return;

        controller.Move(Time.deltaTime * speed * transform.forward);

        if (timer < stopChaseTime && chasing)
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        Vector3 lookPos = player.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, changeDirectionSpeed * Time.deltaTime);
    }

    private void DestroySelf()
    {
        GameObject.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            chasing = false;
        }
    }
}
