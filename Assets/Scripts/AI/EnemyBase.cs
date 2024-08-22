using GMTK_Jam.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace GMTK_Jam.Enemy
{
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Attributes")]
        [SerializeField] private int _health = 3;
        [SerializeField] private float _speed = 5;
        [SerializeField] private int _reward = 1;
        [Range(5f, 1f)]
        public int Priority;

        [Header("Components")]
        [SerializeField] private GameObject _modelParent;
        [SerializeField] private List<Renderer> _modelRenderers;
        [SerializeField] private Material _damageMat;

        [Header("UI")]
        [SerializeField] private GameObject _healthbarCanvas;
        [SerializeField] private Image _healthBar;

        protected List<PathingCorner> _pathCorners = new List<PathingCorner>();
        protected List<PathToCorner> _corners = new List<PathToCorner>();
        protected NavMeshAgent _agent;
        protected int _currentHealth;

        private PathToCorner _currentCornerTarget;
        protected float _startingRadius;
        private bool _isUpdatingRadius = false;
        private List<Material> _originalMats = new List<Material>();
        private Coroutine _updateVisual;
        private Coroutine _updateCarMat;
        private Coroutine _updateRadius;
        private float _fadeTime = 5f;
        private Vector3 _lastDestination;
        private float _radiusMaxDistance;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _startingRadius = _agent.radius;

            for (int i = 0; i < _modelRenderers.Count; i++)
                _originalMats.Add(_modelRenderers[i].material);
        }

        private void Start()
        {
            _agent.speed = _speed;
            _currentHealth = _health;
            _healthbarCanvas.SetActive(false);
            _radiusMaxDistance = _agent.stoppingDistance * 5f;

            float variance = _speed * 0.05f;
            _speed = Random.Range(0, 2) == 0 ? _speed + variance : _speed - variance;
        }

        private void Update()
        {
            if(!_agent.hasPath) return;

            bool movingOnBase = false;
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                _agent.radius = 1f;
                _lastDestination = _currentCornerTarget.Corner.transform.position;

                if(_currentCornerTarget != null)
                    _corners.Remove(_currentCornerTarget);

                if (_corners.Count > 0)
                {
                    _currentCornerTarget = _corners[0];
                    _updateDestination(_currentCornerTarget);
                }
                else
                {
                    movingOnBase = true;
                    _currentCornerTarget = null;
                }
            }

            if (_agent.remainingDistance <= _radiusMaxDistance && !movingOnBase)
            {

                if (/*!_isUpdatingRadius && */_agent.radius > 1f)
                {
                    if (_updateRadius != null)
                        StopCoroutine(_updateRadius);

                    // Gradually shrink the radius as it approaches corner
                    _agent.radius -= 2 * Time.deltaTime;
                }
            }
        }

        public virtual void InitEnemy(List<PathingCorner> corners)
        {
            corners.ForEach(c=> _pathCorners.Add(c));
            StartCoroutine(_calculatePaths());
        }

        private IEnumerator _calculatePaths()
        {
            for (int i = 0; i < _pathCorners.Count; i++)
            {
                NavMeshPath cornerPath = new NavMeshPath();
                while (!
                _agent.CalculatePath(_pathCorners[i].GetDestination(), cornerPath))
                    yield return null;
                _corners.Add(new PathToCorner
                {
                    Path = cornerPath,
                    Corner = _pathCorners[i]
                });
            }
            NavMeshPath finalPath = new NavMeshPath();
            while (!
            _agent.CalculatePath(GameManager.Instance.PlayerBase.transform.position, finalPath))
                yield return null;
            _corners.Add(new PathToCorner
            {
                Path = finalPath,
                Corner = null
            });

            if (_corners.Count > 0)
                _updateDestination(_corners[0]);

        }

        public void RegisterHit(int damage)
        {
            _currentHealth -= damage;

            if(_updateVisual != null)
            {
                StopCoroutine(_updateVisual);
                _updateVisual = null;
                if(_updateCarMat != null)
                {
                    StopCoroutine( _updateCarMat);
                    _updateCarMat = null;
                }
            }
            _updateVisual = StartCoroutine(_updateHealthVisual(_currentHealth <= 0));
        }

        /// <summary>
        /// Call from Player Base script to apply on-hit damage to player.
        /// </summary>
        public void OnEnemyCollideWithBase()
        {
            _destroyEnemy();
            // Reduce player health by remaining enemy health
            GameManager.Instance.UpdatePlayerHealth(-_currentHealth);
        }

        protected virtual void _destroyEnemy()
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }

        private void _updateDestination(PathToCorner target/*Vector3 target*/)
        {
            StartCoroutine(_drawNextPath(target)); 
            if (_agent.radius < _startingRadius)
            {
                if (_updateRadius != null)
                    StopCoroutine(_updateRadius);

                _updateRadius = StartCoroutine(_scaleUpRadius());
            }
            //Debug.Log("Corners left: " + _corners.Count);
        }

        private IEnumerator _drawNextPath(PathToCorner target)
        {
            while (!_agent.SetPath(target.Path))
            {
                Debug.Log("Failed to assign...");
                int index = _corners.IndexOf(_currentCornerTarget);
                if(index > 0)
                    transform.position = _corners[index - 1].Corner.GetDestination();
                else
                    transform.position = _corners[index].Corner.GetDestination();
                yield return new WaitForEndOfFrame();
            }

            _currentCornerTarget = target;
        }

        private IEnumerator _updateHealthVisual(bool isDead)
        {
            if (isDead)
            {
                GameManager.Instance.UpdatePlayerResource(_reward, true);
                _destroyEnemy();
            }
            else
            {
                _updateCarMat = StartCoroutine(_flipCarMat());
                _healthbarCanvas.SetActive(true);
                _healthBar.fillAmount = (float)_currentHealth / (float)_health;


                // TODO: Add nice fade in/out
                yield return new WaitForSeconds(_fadeTime);
                _healthbarCanvas.gameObject.SetActive(false);
            }
        }

        private IEnumerator _flipCarMat()
        {
            _modelRenderers.ForEach(r => r.material = _damageMat);
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < _modelRenderers.Count; i++)
                _modelRenderers[i].material = _originalMats[i];
        }

        private IEnumerator _scaleUpRadius()
        {
            _isUpdatingRadius = true;
            while (Vector3.Distance(_lastDestination, transform.position) < _radiusMaxDistance)
                yield return null;

            while (_agent.radius < _startingRadius)
            {
                _agent.radius += Time.deltaTime;
                yield return null;
            }

            if( _agent.radius > _startingRadius)
                _agent.radius = _startingRadius;

            _isUpdatingRadius = false;
        }
    }

    public class PathToCorner
    {
        public NavMeshPath Path;
        public PathingCorner Corner;
    }
}
