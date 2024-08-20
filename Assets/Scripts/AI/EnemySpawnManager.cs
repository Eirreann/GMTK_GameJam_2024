using GMTK_Jam.AI;
using GMTK_Jam.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GMTK_Jam.Enemy
{
    /// <summary>
    /// This class manages the enemy spawning, should be informed by the GameManager with waves logic
    /// </summary>
    public class EnemySpawnManager : MonoSingleton<EnemySpawnManager>
    {
        [HideInInspector] public List<EnemyBase> SpawnedEnemies = new List<EnemyBase>();

        [Header("Pathing")]
        [SerializeField] private Transform _spawnObscurer;
        [SerializeField] private EnemySpawnPoint _currentSpawnPoint;
        [SerializeField] private List<PathingCorner> _corners = new List<PathingCorner>();

        [Header("UI")]
        public GameObject WaveStarting;
        public UITooltip Tooltip;

        private float _obscurerSpeed = 2f;
        private bool _waveActive = false;
        private UnityAction _onWaveComplete;

        private void Update()
        {
            if(_currentSpawnPoint != null)
            {
                Vector3 targetPos = new(_currentSpawnPoint.transform.position.x, _spawnObscurer.position.y, _currentSpawnPoint.transform.position.z);
                _spawnObscurer.position = Vector3.Lerp(_spawnObscurer.position, targetPos, _obscurerSpeed * Time.deltaTime);
            }
        }

        public void StartWave(WaveOptions wave, UnityAction onWaveComplete)
        {
            _onWaveComplete = onWaveComplete;
            StartCoroutine(_startWave(wave));
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

        private IEnumerator _startWave(WaveOptions wave)
        {
            if (wave.HasTooltip)
            {
                Tooltip.gameObject.SetActive(true);
                Tooltip.ShowTooltip(wave.TooltipText);
            }

            float uiTime = 3f;
            yield return new WaitForSeconds(wave.WaveTime - uiTime);

            WaveStarting.SetActive(true);
            AudioManager.Instance.OnWaveStart();
            yield return new WaitForSeconds(uiTime);
            WaveStarting.SetActive(false);

            _waveActive = true;
            int currentIndex = 0;
            while (currentIndex < wave.Batches.Count && _waveActive)
            {
                BatchSettings settings = wave.Batches[currentIndex];
                EnemyBase enemyType = GameManager.Instance.GetEnemyObjectByType(settings.Enemy_Type);
                _currentSpawnPoint.SpawnEnemies(enemyType, settings.EnemyCount, _corners, (e) =>
                {
                    SpawnedEnemies.AddRange(e);
                });

                yield return new WaitForSeconds(settings.BatchTime);

                if (GameManager.Instance.State != GameState.ENDED)
                    currentIndex++;
                else
                    _waveActive = false;
            }

            while(SpawnedEnemies.Count > 0 && _waveActive)
            {
                SpawnedEnemies.RemoveAll(t => t == null); // Remove any destroyed enemies   

                if (GameManager.Instance.State == GameState.ENDED)
                    _waveActive = false;

                yield return null;
            }

            _onWaveComplete.Invoke();
        }
    }
}
