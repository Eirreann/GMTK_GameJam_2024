using GMTK_Jam.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        [SerializeField] protected float _projectileSpeed = 5f;
        [SerializeField] protected float _impactDistance = 0.5f;

        protected ProjectilePool _pool;
        protected EnemyBase _target;
        protected int _damage;

        protected Coroutine _fireCoroutine;
        protected bool _isFired = false;

        public void Init(ProjectilePool pool)
        {
            _pool = pool;
        }

        public virtual void FireAtTarget(EnemyBase target, int damage)
        {
            _target = target;
            _damage = damage;
            _isFired = true;
        }

        protected virtual void Update()
        {
            if(_target == null && _isFired)
            {
                StopAllCoroutines();
                _isFired = false;
                _pool.ReturnToPool(this);
            }

        }

        protected virtual void _returnToPool()
        {
            _isFired = false;
            _pool.ReturnToPool(this);
        }

        protected IEnumerator _moveTowardTarget()
        {
            while(Vector3.Distance(transform.position, _target.transform.position) > _impactDistance)
            {
                //Debug.Log(Vector3.Distance(transform.position, _target.position));
                transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _projectileSpeed * Time.deltaTime);
                transform.LookAt(_target.transform);

                yield return null;
            }

            _target.RegisterHit(_damage);
            _returnToPool();
        }
    }
}
