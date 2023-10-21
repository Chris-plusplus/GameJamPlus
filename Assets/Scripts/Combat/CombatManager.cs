using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Combat
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager singleton;

        private Dictionary<Team, List<CombatEntity>> combatEntities = new Dictionary<Team, List<CombatEntity>>();

        private void Awake()
        {
            singleton = this;
        }

        public void AddNewCombatEntity(CombatEntity newCombatEntity)
        {
            if (combatEntities.ContainsKey(newCombatEntity.team))
            {
                combatEntities[newCombatEntity.team].Add(newCombatEntity);
            }
            else
            {
                combatEntities[newCombatEntity.team] = new List<CombatEntity> { newCombatEntity };
            }
        }
        public void RemoveCombatEntity(CombatEntity combatEntity)
        {
            combatEntities[combatEntity.team].Remove(combatEntity);
        }

        public List<CombatEntity> GetCombatEntitiesOfTeam(Team team)
        {
            if (combatEntities.ContainsKey(team))
            {
                return combatEntities[team];
            }
            else
            {
                return new List<CombatEntity>();
            }
        }
    }

    public enum Team
    {
        Player,
        Enemy
    }
}