using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float aimbot = 1f;

    public Team targetTeam;
    public int damage;
    public Transform targetTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 v = rb.velocity;
        v.y = 0;
        Vector3 correction = targetTransform.position - transform.position;
        correction.y = 0;
        correction.Normalize();
        v = Vector3.Lerp(v, correction * v.magnitude, aimbot);
        rb.velocity = v + Vector3.up * rb.velocity.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        CombatEntity hitEntity;
        if (other.TryGetComponent<CombatEntity>(out hitEntity))
        {
            if (hitEntity.team == targetTeam)
            {
                hitEntity.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
