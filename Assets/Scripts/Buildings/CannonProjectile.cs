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

        public void Setup(Transform muzzlePos, List<EnemyBase> enemies)
        {
            _targets = enemies;
            var targetOffset = new Vector3(muzzlePos.localPosition.x, muzzlePos.localPosition.y, muzzlePos.localPosition.z + _travelDistance);
            _targetPos = muzzlePos.TransformPoint(targetOffset);
        }

        public override void FireAtTarget(EnemyBase target, int damage)
        {
            base.FireAtTarget(target, damage);
            _fireCoroutine = StartCoroutine(_fireBullet());
        }

        private IEnumerator _fireBullet()
        {
            while (Vector3.Distance(transform.position, _targetPos) > 1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetPos, _projectileSpeed * Time.deltaTime);

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

            _isFired = false;
            _pool.ReturnToPool(this);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _impactDistance);
        }
    }
}
