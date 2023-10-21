using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Team target;
    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        CombatEntity hitEntity;
        if (other.TryGetComponent<CombatEntity>(out hitEntity))
        {
            if (hitEntity.team == target)
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
