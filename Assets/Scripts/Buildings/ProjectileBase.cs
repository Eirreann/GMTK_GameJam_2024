using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public abstract class ProjectileBase : MonoBehaviour
    {
        [SerializeField] protected float _projectileSpeed = 5f;

        protected ProjectilePool _pool;
        protected Transform _target;
        protected float _impactDistance = 0.5f;

        public void Init(ProjectilePool pool)
        {
            _pool = pool;
        }

        public virtual void FireAtTarget(Transform target)
        {
            _target = target;
            StartCoroutine(_moveTowardTarget());
        }

        private IEnumerator _moveTowardTarget()
        {
            while(Vector3.Distance(transform.position, _target.position) > _impactDistance)
            {
                //Debug.Log(Vector3.Distance(transform.position, _target.position));
                transform.position = Vector3.MoveTowards(transform.position, _target.position, _projectileSpeed * Time.deltaTime);

                var rotatePos = _target.position - transform.position;
                rotatePos.y = 0;
                var rotation = Quaternion.LookRotation(rotatePos);
                transform.rotation = rotation;

                yield return null;
            }

            _pool.ReturnToPool(this);
        }
    }
}
