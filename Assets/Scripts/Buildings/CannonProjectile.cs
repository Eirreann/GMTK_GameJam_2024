using GMTK_Jam.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public class CannonProjectile : ProjectileBase
    {
        [SerializeField] private float _travelDistance = 50f;

        private Vector3 _targetPos;
        private List<EnemyBase> _targets;

        protected override void Update()
        {
            // Do nothing : D
        }

        public void SetEnemiesInRange(List<EnemyBase> enemies)
        {
            _targets = enemies;
        }

        public override void FireAtTarget(EnemyBase target, int damage)
        {
            base.FireAtTarget(target, damage);
            _targetPos = new(transform.position.x + _travelDistance, transform.position.y, transform.position.z);
            _fireCoroutine = StartCoroutine(_fireBullet());
        }

        private IEnumerator _fireBullet()
        {
            while (Vector3.Distance(transform.position, _targetPos) > 1f)
            {
                Debug.Log(Vector3.Distance(transform.position, _targetPos));
                transform.position = Vector3.MoveTowards(transform.position, _targetPos, _projectileSpeed * Time.deltaTime);
                //transform.LookAt(_target.transform);

                List<EnemyBase> enemiesHit = new List<EnemyBase>();
                _targets.ForEach((e) =>
                {
                    if (!enemiesHit.Contains(e))
                    {
                        if (Vector3.Distance(transform.position, e.transform.position) < _impactDistance)
                        {
                            e.RegisterHit(_damage);
                            enemiesHit.Add(e);
                        }
                    }
                });

                yield return null;
            }

            Debug.Log("Returned to pool");
            _isFired = false;
            _pool.ReturnToPool(this);
        }
    }
}
