using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float stanbyTime = 3f;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float changeDirectionSpeed = 20f;
    [SerializeField] private float lifeTime = 3.5f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float knockback;
    [SerializeField] private float launchForce;
    [SerializeField] private float hitLagDuration;
    [SerializeField] private float hitLagStrength;
    [SerializeField] private Vector3 offset;
    [SerializeField] private GameObject hitEffectPrefab;

    private GameObject player;
    private float timer;
    private float chaseTime;
    private bool finished;
    private float lerpSpeed;

    private void Start()
    {
        lerpSpeed = speed * -0.2f;
        controller = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player");
        Invoke(nameof(DestroySelf), lifeTime);
        GetComponent<AttackCollider>().SetAttack(damage, knockback, launchForce, hitLagDuration, hitLagStrength, hitEffectPrefab);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer < stanbyTime) return;

        lerpSpeed = Mathf.Lerp(lerpSpeed, speed, Time.deltaTime * 3f);
        controller.Move(Time.deltaTime * lerpSpeed * transform.forward);
    }

    private void FollowTarget()
    {
        if (finished) return;
        transform.position = player.transform.position + offset;
    }

    private void DestroySelf()
    {
        GameObject.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == this.gameObject) return;
        if (other.gameObject.name == "Targeter") return;
        if (other.gameObject.CompareTag("Player")) DestroySelf(); 
    }
}
