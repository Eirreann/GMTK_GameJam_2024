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

        private Material _originalMat;
        private Coroutine _updateVisual;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _originalMat = _modelRender.material;
        }

        private void Start()
        {
            _agent.speed = _speed;
            _agent.SetDestination(GameManager.Instance.PlayerBase.position);
            _currentHealth = _health;
            _healthbarCanvas.SetActive(false);
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

        private IEnumerator _updateHealthVisual(bool isDead)
        {
            if (isDead)
            {
                _modelParent.SetActive(false);
                // TODO: Update score
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(_flipCarMat());
                _healthbarCanvas.SetActive(true);
                _healthBar.fillAmount = (float)_currentHealth / (float)_health;


                // TODO: Add nice fade in/out
                yield return new WaitForSeconds(1f);
                _healthbarCanvas.gameObject.SetActive(false);
            }
        }

        private IEnumerator _flipCarMat()
        {
            _modelRender.material = _damageMat;
            yield return new WaitForSeconds(0.1f);
            _modelRender.material = _originalMat;
        }
    }
}
