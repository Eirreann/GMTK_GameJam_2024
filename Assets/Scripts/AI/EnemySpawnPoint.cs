using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Enemy
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] private EnemyBase _enemyPrefab;
        [SerializeField] private Vector3 _spawnArea = Vector3.one;
        [SerializeField] private int _enemyCount;

        private void Start()
        {
            StartCoroutine(_spawnEnemies());
        }

        private IEnumerator _spawnEnemies()
        {
            for(int i = 0; i < _enemyCount; i++)
            {
                float spawnRangeX = _spawnArea.x/2;
                float spawnRangeZ = _spawnArea.z/2;
                EnemyBase enemy = Instantiate(_enemyPrefab, transform);
                enemy.transform.localPosition = new Vector3(enemy.transform.localPosition.x + Random.Range(-spawnRangeX, spawnRangeX), enemy.transform.localPosition.y, enemy.transform.localPosition.x + Random.Range(-spawnRangeZ, spawnRangeZ));
                yield return new WaitForSeconds(0.25f);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _spawnArea);
        }
    }
}
