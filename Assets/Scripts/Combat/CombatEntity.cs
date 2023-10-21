using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Combat
{
    public class CombatEntity : MonoBehaviour
    {
        [SerializeField][ReadOnly] public int HP;
        public int maxHP;
        public Team team;
        public int agroPriority = 0;
        [System.NonSerialized] public bool alive = true;

        [SerializeField] private bool ragdoll;
        [SerializeField] private bool disableAtAwake;
        public bool DisableAtAwake => disableAtAwake;
        private Animator animator;

        private void Awake()
        {
            alive = true;
            animator = GetComponent<Animator>();
            if (DisableAtAwake)
            {
                enabled = false;
            }
        }

        private void Start()
        {
            HP = maxHP;
            if (CombatManager.singleton == null)
            {
                Debug.LogError("Na scenie ni ma CombatEntityManager :(");
            }
            CombatManager.singleton.AddNewCombatEntity(this);
        }

        public void TakeDamage(int amount)
        {
            if (animator)
            {
                animator.SetTrigger("TakeDamage");
            }
            HP -= amount;
            if (HP <= 0)
            {
                Die();
                HP = 0;
            }
        }

        private void Die()
        {
            alive = false;
            CombatManager.singleton.RemoveCombatEntity(this);
            if (ragdoll)
            {
                GetComponent<NavMeshAgent>().enabled = false;
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Animator>().enabled = false;
            }
            else if (animator)
            {
                animator.SetTrigger("Death");
            }
        }
    }
}