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
        rb.velocity += (targetTransform.position - transform.position).normalized *
            aimbot / ((targetTransform.position - transform.position).sqrMagnitude + 1);
        Debug.Log((targetTransform.position - transform.position).normalized *
            aimbot / ((targetTransform.position - transform.position).sqrMagnitude + 1));
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
