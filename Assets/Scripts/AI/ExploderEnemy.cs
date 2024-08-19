using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Enemy
{
    public class ExploderEnemy : EnemyBase
    {
        [Header("Spawner Enemy")]
        [SerializeField] private EnemyBase _enemyToSpawn;
        [SerializeField] private int _spawnCount;

        protected override void _destroyEnemy()
        {

            for (int i = 0; i < _spawnCount; i++)
            {
                EnemyBase enemy = Instantiate(_enemyToSpawn, transform);
                enemy.transform.parent = null;
                enemy.InitEnemy(_corners);
                EnemySpawnManager.Instance.SpawnedEnemies.Add(enemy);
            }

            base._destroyEnemy();
        }
    }
}
