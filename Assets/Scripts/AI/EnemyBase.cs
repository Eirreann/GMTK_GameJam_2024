using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GMTK_Jam.Enemy
{
    public abstract class EnemyBase : MonoBehaviour
    {
        [SerializeField] private float _speed = 5;

        protected NavMeshAgent _agent;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = _speed;
            _agent.SetDestination(GameManager.Instance.PlayerBase.position);
        }
    }
}
