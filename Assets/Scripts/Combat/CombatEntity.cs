using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class CombatEntity : MonoBehaviour
    {
        [SerializeField][ReadOnly] public int HP;
        public int maxHP;
        public Team team;
        public int agroPriority = 0;
        [System.NonSerialized] public bool alive = true;

        private Animator animator;

        private void Awake()
        {
            alive = true;
            animator = GetComponent<Animator>();
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
            if (animator)
            {
                animator.SetTrigger("Death");
            }
        }
    }
}