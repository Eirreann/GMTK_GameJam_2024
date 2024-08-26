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
        private List<EnemyBase> _targets = new List<EnemyBase>();
        private List<EnemyBase> _targetsHit = new List<EnemyBase>();

        protected override void Update()
        {
            // Do nothing : D
        }

        public void Setup(float travelDistance, Transform muzzlePos, List<EnemyBase> enemies)
        {
            // update travel distance
            _travelDistance = travelDistance;

            // Update bullet scale
            _impactDistance = transform.lossyScale.x * 1.25f;

            // Get reference list of enemies from tower (not currently used as we're instead using list of _all_ enemies - may revert if we sort out collider issue)
            enemies.ForEach(e => _targets.Add(e));

            // Set travel endpoint
            _targetPos = transform.position + muzzlePos.transform.forward * _travelDistance;
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

                List<EnemyBase> enemies = new List<EnemyBase>();
                EnemySpawnManager.Instance.SpawnedEnemies.ForEach((e) => enemies.Add(e));
                enemies.RemoveAll(e => e == null);
                enemies.ForEach(enemy =>
                {
                    if (!_targetsHit.Contains(enemy))
                    {
                        if (Vector3.Distance(transform.position, enemy.transform.position) < _impactDistance)
                        {
                            _targetsHit.Add(enemy);
                            enemy.RegisterHit(_damage);
                        }
                    }
                });

                yield return null;
            }

            _returnToPool();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _impactDistance);
        }
    }
}
