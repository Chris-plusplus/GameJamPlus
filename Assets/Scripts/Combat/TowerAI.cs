using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(CombatEntity))]
    public class TowerAI : MonoBehaviour, IAIModule
    {
        [SerializeField] private int attackDamage = 1;
        [SerializeField] private float attackDelay = 3f;
        [SerializeField] private float attackRange = 8f;
        [SerializeField] private float projectileSpeed = 15f;

        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private Transform shootingPoint;
        [SerializeField] private Transform rotatablePart;  

        [SerializeField, ReadOnly] private CombatEntity agro;

        private Animator animator;
        private CombatEntity selfCombatEntity;
        private float lastAttackTime = -10f;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            selfCombatEntity = GetComponent<CombatEntity>();
            if (rotatablePart == null)
            {
                rotatablePart = transform;
            }
            if (selfCombatEntity.DisableAtAwake)
            {
                enabled = false;
            }
        }

        private void Update()
        {
            if (selfCombatEntity.alive)
            {
                UpdateAgro();

                if (agro != null)
                {
                    LookAtAgroTarget();

                    if (Time.time - lastAttackTime > attackDelay)
                    {
                        Attack();
                        lastAttackTime = Time.time;
                    }
                }
            }
        }

        private void UpdateAgro()
        {
            List<CombatEntity> enemies = CombatManager.singleton.GetCombatEntitiesOfTeam(Team.Enemy);
            agro = null;
            foreach (CombatEntity enemy in enemies)
            {
                // range check
                if (Distance(transform.position, enemy.transform.position) <= attackRange)
                {
                    // agro priority check
                    if (agro == null || agro.agroPriority < enemy.agroPriority)
                    {
                        agro = enemy;
                    }
                }
            }
        }

        private void LookAtAgroTarget()
        {
            Vector3 selfPosition = transform.position;
            selfPosition.y = 0f;
            Vector3 targetPosition = agro.transform.position;
            targetPosition.y = 0f;

            rotatablePart.rotation = Quaternion.LookRotation(-targetPosition + selfPosition, Vector3.up);
        }

        private void Attack()
        {
            if (animator)
            {
                animator.SetTrigger("Attack");
            }

            Projectile projectile = Instantiate(projectilePrefab, shootingPoint.position, rotatablePart.rotation);
            projectile.targetTeam = Team.Enemy;
            projectile.damage = attackDamage;
            projectile.targetTransform = agro.transform;
            Vector3 velocityVector = agro.transform.position - transform.position;
            velocityVector.y = 0;
            velocityVector.Normalize();
            Vector3 upwordsForce = Vector3.up * (Physics.gravity.magnitude * Distance(transform.position, agro.transform.position) / projectileSpeed / 2);
            projectile.SetUpVelocity(velocityVector * projectileSpeed + upwordsForce);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        private float Distance(Vector3 pos1, Vector3 pos2)
        {
            pos1.y = 0;
            pos2.y = 0;
            return Vector3.Distance(pos1, pos2);
        }

        public void SetActive(bool active) => enabled = active;
    }
}