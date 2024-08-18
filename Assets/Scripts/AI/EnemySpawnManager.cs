using GMTK_Jam.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Enemy
{
    /// <summary>
    /// This class manages the enemy spawning, should be informed by the GameManager with waves logic
    /// </summary>
    public class EnemySpawnManager : MonoBehaviour
    {
        [Header("Pathing")]
        [SerializeField] private EnemySpawnPoint _currentSpawnPoint;
        [SerializeField] private List<PathingCorner> _corners = new List<PathingCorner>();

        private void Start()
        {
            UpdateSpawnPoint(_currentSpawnPoint);
            _currentSpawnPoint.SpawnEnemies(_corners);
        }

        /// <summary>
        /// Call this when adding a new chunk to the path, pass in chunk's spawn point, and add corners to pathing.
        /// </summary>
        /// <param name="spawnPoint">The spawn point to start spawning enemies from.</param>
        public void UpdateSpawnPoint(EnemySpawnPoint spawnPoint)
        {
            _currentSpawnPoint = spawnPoint;
            _currentSpawnPoint.CornersInChunk.ForEach(corner => _corners.Add(corner));
        }
    }
}
