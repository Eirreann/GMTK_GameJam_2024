using GMTK_Jam.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.GridLayoutGroup;

namespace GMTK_Jam.Enemy
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [Header("Pathing")]
        public List<PathingCorner> CornersInChunk;

        [Header("Enemy Settings")]
        //[SerializeField] private EnemyBase _enemyPrefab;
        [SerializeField] private Vector3 _spawnArea = Vector3.one;
        //[SerializeField] private int _enemyCount;
        [SerializeField] private float _spawnFrequency = 0.25f;

        private UnityAction<List<EnemyBase>> _onEnemiesAdded;

        public void SpawnEnemies(EnemyBase prefab, int count, List<PathingCorner> corners, UnityAction<List<EnemyBase>> onEnemiesAdded)
        {
            _onEnemiesAdded = onEnemiesAdded;
            StartCoroutine(_spawnEnemies(prefab, count, corners));
        }

        private IEnumerator _spawnEnemies(EnemyBase prefab, int count, List<PathingCorner> corners)
        {
            List<EnemyBase> enemies = new List<EnemyBase>();
            for(int i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(_spawnFrequency);

                float spawnRangeX = _spawnArea.x/2;
                float spawnRangeZ = _spawnArea.z/2;
                EnemyBase enemy = Instantiate(prefab, transform);
                enemy.transform.localPosition = new Vector3(enemy.transform.localPosition.x + Random.Range(-spawnRangeX, spawnRangeX), enemy.transform.localPosition.y, enemy.transform.localPosition.z + Random.Range(-spawnRangeZ, spawnRangeZ));
                enemy.InitEnemy(corners);
                enemies.Add(enemy);
            }
            _onEnemiesAdded.Invoke(enemies);
        }

        private const float GIZMO_LINE_LENGTH = 5f;
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _spawnArea);
            Gizmos.DrawLine(transform.position, transform.position + (transform.forward * GIZMO_LINE_LENGTH));
        }
    }
}
