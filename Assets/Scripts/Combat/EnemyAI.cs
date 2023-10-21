using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Combat
{
    [RequireComponent(typeof(CombatEntity))]
    public class EnemyAI : MonoBehaviour
    {
        public enum AIState
        {
            Patrol,
            FollowPath,
            FollowAgro,
            Attack
        }

        private CombatEntity selfCombatEntity;
        private NavMeshAgent agent;
        private Animator animator;
        private Vector3 currentPatrolTarget;
        private float lastAttackTime = -10f;

        [SerializeField] private BoxCollider patrolArea;
        [SerializeField] private float attackRange = 1f;
        [SerializeField] private float agroRange = 10f;
        [SerializeField] private int attackDamage = 1;
        [SerializeField] private float attackDelay = 1f;
        [SerializeField] private Transform target;

        [SerializeField][ReadOnly] private AIState currentState;
        [SerializeField][ReadOnly] private CombatEntity agro;

        private void Awake()
        {
            selfCombatEntity = GetComponent<CombatEntity>();
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (selfCombatEntity.alive)
            {
                FindAgro();
                UpdateAIState();

                if (currentState == AIState.Attack && Time.time - attackDelay > lastAttackTime)
                {
                    Attack();
                    lastAttackTime = Time.time;
                }
            }
        }

        private void UpdateAIState()
        {
            if (agro == null)
            {
                if (Distance(transform.position, currentPatrolTarget) < 0.01f || agent.velocity.sqrMagnitude <= 0.001f)
                {
                    currentPatrolTarget = RandomPointInBox(patrolArea);
                }
                agent.destination = currentPatrolTarget;
                currentState = AIState.Patrol;
            }
            else
            {
                if (Distance(transform.position, agro.transform.position) > attackRange)
                {
                    currentState = AIState.FollowAgro;
                    agent.destination = agro.transform.position;
                }
                else 
                {
                    currentState = AIState.Attack;
                    agent.destination = transform.position;
                }
            }
        }

        private void Attack()
        {
            agro.TakeDamage(attackDamage);
            animator.SetTrigger("Attack");
        }

        private Vector3 RandomPointInBox(BoxCollider boxCollider)
        {
            return boxCollider.bounds.center + new Vector3(
               (Random.value - 0.5f) * boxCollider.bounds.size.x,
               (Random.value - 0.5f) * boxCollider.bounds.size.y,
               (Random.value - 0.5f) * boxCollider.bounds.size.z
            );
        }


        private void FindAgro()
        {
            List<CombatEntity> plants = CombatManager.singleton.GetCombatEntitiesOfTeam(Team.Player);
            agro = null;
            foreach (CombatEntity plant in plants)
            {
                if (Distance(transform.position, plant.transform.position) <= agroRange)
                {
                    if (agro == null || agro.agroPriority < plant.agroPriority)
                    {
                        agro = plant;
                    }
                }
            }
        }

        private float Distance(Vector3 pos1, Vector3 pos2)
        {
            pos1.y = 0;
            pos2.y = 0;
            return Vector3.Distance(pos1, pos2);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, agroRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(patrolArea.transform.position + patrolArea.center + new Vector3(0,1,0), patrolArea.size);
            if (currentPatrolTarget != null)
            {
                Gizmos.DrawSphere(currentPatrolTarget + new Vector3(0,1,0), 0.5f);
            }
        }
    }
}