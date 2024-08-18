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

        [Header("Components")]
        [SerializeField] private GameObject _modelParent;
        [SerializeField] private Renderer _modelRender;
        [SerializeField] private Material _damageMat;

        [Header("UI")]
        [SerializeField] private GameObject _healthbarCanvas;
        [SerializeField] private Image _healthBar;

        protected NavMeshAgent _agent;
        protected int _currentHealth;

        private List<PathingCorner> _corners;
        private PathingCorner _currentCornerTarget;
        private float _startingRadius;
        private bool _isUpdatingRadius = false;
        private Material _originalMat;
        private Coroutine _updateVisual;
        private Coroutine _updateRadius;
        private float _fadeTime = 5f;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _originalMat = _modelRender.material;
            _startingRadius = _agent.radius;
        }

        private void Start()
        {
            _agent.speed = _speed;
            _currentHealth = _health;
            _healthbarCanvas.SetActive(false);
        }

        private void Update()
        {
            if(!_agent.hasPath) return;

            bool movingOnBase = false;
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                if(_currentCornerTarget != null)
                    _corners.Remove(_currentCornerTarget);

                if (_corners.Count > 0)
                {
                    _currentCornerTarget = _corners[0];
                    _updateDestination(_currentCornerTarget.GetDestination());
                }
                else
                {
                    movingOnBase = true;
                    _currentCornerTarget = null;
                    _updateDestination(GameManager.Instance.PlayerBase.transform.position);
                }
            }

            if(_agent.remainingDistance <= _agent.stoppingDistance * 5 && !movingOnBase)
            {
                if (!_isUpdatingRadius && _agent.radius > 1f)
                {
                    // Gradually shrink the radius as it approaches corner
                    _agent.radius -= Time.deltaTime;
                }
            }
        }

        public void InitEnemy(List<PathingCorner> corners)
        {
            _corners = corners;
            if (_corners.Count > 0)
                _updateDestination(_corners[0].GetDestination());
            else
                _updateDestination(GameManager.Instance.PlayerBase.transform.position);
        }

        public void RegisterHit(int damage)
        {
            _currentHealth -= damage;

            if(_updateVisual != null)
            {
                StopCoroutine(_updateVisual);
                _updateVisual = null;
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

        private void _updateDestination(Vector3 target)
        {
            _agent.SetDestination(target);
            if(_agent.radius < _startingRadius)
            {
                if (_updateRadius != null)
                    StopCoroutine(_updateRadius);

                _updateRadius = StartCoroutine(_scaleUpRadius());
            }
            //_agent.radius = _startingRadius;
        }

        private IEnumerator _updateHealthVisual(bool isDead)
        {
            if (isDead)
            {
                GameManager.Instance.UpdatePlayerResource(_reward);
                _destroyEnemy();
            }
            else
            {
                StartCoroutine(_flipCarMat());
                _healthbarCanvas.SetActive(true);
                _healthBar.fillAmount = (float)_currentHealth / (float)_health;


                // TODO: Add nice fade in/out
                yield return new WaitForSeconds(_fadeTime);
                _healthbarCanvas.gameObject.SetActive(false);
            }
        }

        private IEnumerator _flipCarMat()
        {
            _modelRender.material = _damageMat;
            yield return new WaitForSeconds(0.1f);
            _modelRender.material = _originalMat;
        }

        private IEnumerator _scaleUpRadius()
        {
            Debug.Log("Scaling up radius");
            _isUpdatingRadius = true;
            yield return new WaitForSeconds(2f); // TODO: Update delay relative to speed?
            while (_agent.radius < _startingRadius)
            {
                _agent.radius += Time.deltaTime;
                yield return null;
            }

            if( _agent.radius > _startingRadius)
                _agent.radius = _startingRadius;

            _isUpdatingRadius = false;
        }

        private void _destroyEnemy()
        {
            StopAllCoroutines();
            _modelParent.SetActive(false);
            Destroy(gameObject);
        }
    }
}
