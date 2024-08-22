using GMTK_Jam.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GMTK_Jam.Enemy
{
    public class SpawnerEnemy : EnemyBase
    {
        [Header("Spawner Enemy")]
        [SerializeField] private EnemyBase _enemyToSpawn;
        [SerializeField] private Transform _spawnLocation;
        [SerializeField] private float _spawnInterval;
        [SerializeField] private int _spawnCount;

        private Coroutine _spawnCoroutine;

        public override void InitEnemy(List<PathingCorner> corners)
        {
            base.InitEnemy(corners);
            _spawnCoroutine = StartCoroutine(_spawnEnemies());
        }

        protected override void _destroyEnemy()
        {
            if(_spawnCoroutine != null)
                StopCoroutine(_spawnCoroutine);

            base._destroyEnemy();
        }

        private IEnumerator _spawnEnemies()
        {
            yield return new WaitForSeconds(_spawnInterval);

            for(int i = 0; i < _spawnCount; i++)
            {
                EnemyBase enemy = Instantiate(_enemyToSpawn, _spawnLocation);
                enemy.transform.parent = transform.parent;
                List<PathingCorner> newCornersList = new List<PathingCorner>();
                _corners.ForEach((c) =>
                {
                    if (c.Corner != null)
                        newCornersList.Add(c.Corner);
                });
                enemy.InitEnemy(newCornersList);
                EnemySpawnManager.Instance.SpawnedEnemies.Add(enemy);
            }

            _spawnCoroutine = StartCoroutine(_spawnEnemies());
        }
    }
}
