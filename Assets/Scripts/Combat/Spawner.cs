using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private BoxCollider patrolArea;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private int maxEnemiesCount = 3;
        [SerializeField] private List<CombatEntity> aliveEniemies;
        [SerializeField] private float spawnColldown = 10f;

        private float lastSpawnTime = 0f;

        private void Update()
        {
            for (int i = 0; i < aliveEniemies.Count; i++)
            {
                CombatEntity combatEntity = aliveEniemies[i];
                if (combatEntity == null || combatEntity.alive == false)
                {
                    aliveEniemies.Remove(combatEntity);
                    i--;
                }
            }

            if (aliveEniemies.Count < maxEnemiesCount && Time.time - lastSpawnTime > spawnColldown)
            {
                SpawnEnemy();
                lastSpawnTime = Time.time;
            }
        }

        private void SpawnEnemy()
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, new Quaternion(0, 0, 0, 0));
            CombatEntity combatEntity = enemy.GetComponent<CombatEntity>();
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            enemyAI.patrolArea = patrolArea;
            aliveEniemies.Add(combatEntity);
        }
    }
}