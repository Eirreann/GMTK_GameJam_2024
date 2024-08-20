using GMTK_Jam.Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GMTK_Jam.Buildings
{
    public class RicochetProjectile : ProjectileBase
    {
        [SerializeField] private RicochetSubBullet _bulletTemplate;

        private float _radius;

        public void SetRadius(float radius)
        {
            _radius = radius;
        }

        public override void FireAtTarget(EnemyBase target, int damage)
        {
            base.FireAtTarget(target, damage);
            _fireCoroutine = StartCoroutine(_moveTowardTarget());
        }

        public override void UpdateScale(int level)
        {
            // Do nothing :D
        }

        protected override IEnumerator _moveTowardTarget()
        {
            while (Vector3.Distance(transform.position, _target.transform.position) > _impactDistance)
            {
                //Debug.Log(Vector3.Distance(transform.position, _target.position));
                transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, _projectileSpeed * Time.deltaTime);
                transform.LookAt(_target.transform);

                yield return null;
            }

            _target.RegisterHit(_damage);
            _spawnRicochetBullets();
            _returnToPool();
        }

        private void _spawnRicochetBullets()
        {

            EnemySpawnManager.Instance.SpawnedEnemies.RemoveAll(t => t == null); // Remove any destroyed enemies   

            List<EnemyBase> enemies = new List<EnemyBase>();
            EnemySpawnManager.Instance.SpawnedEnemies.ForEach(e => enemies.Add(e));
            enemies = enemies.OrderBy((e) => Vector3.Distance(transform.position, e.transform.position)).ToList();
            if(enemies.Contains(_target))
                enemies.Remove(_target);

            int enemyCount = enemies.Count >= 3 ? 3 : enemies.Count;
            for (int i = 0; i < enemyCount; i++)
            {
                var target = enemies[i];
                var bullet = Instantiate(_bulletTemplate, transform);
                bullet.transform.parent = transform.parent;
                bullet.gameObject.SetActive(true);
                bullet.FireSubBullet(_impactDistance, _projectileSpeed, _damage, target, _radius);
            }
        }
    }
}
