using GMTK_Jam.Enemy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public class CannonTower : TowerBase
    {
        [Header("Cannon Tower")]
        [SerializeField]protected float travelDistance = 50f;

        protected override void towerLookAt(Transform target = null)
        {
            // DOES NOTHING :D
            // (Overriding to skip this function's base functionality
        }

        protected override void _spawnBullet(int damage, EnemyBase target)
        {
            ProjectileBase bullet = pool.GetProjectile(currentLevel);
            (bullet as CannonProjectile).Setup(_getAoE(), BulletSpawnPos, targets);
            bullet.FireAtTarget(null, damage);

            if (OnFire != null)
                _audioSource.PlayOneShot(OnFire);
        }

        protected override void _updateAoE()
        {
            var rangeCollider = GetComponent<BoxCollider>();
            rangeCollider.center = new Vector3(rangeCollider.center.x, rangeCollider.center.y, _getAoE() / 2);
            rangeCollider.size = new Vector3(_getAoE() / 2, rangeCollider.size.y, _getAoE());
            if(_aoeCoroutine != null)
                StopCoroutine(_aoeCoroutine);
            _aoeCoroutine = StartCoroutine(_drawAoE());
        }

        protected override IEnumerator _drawAoE()
        {
            bool isDrawing = true;
            while (isDrawing)
            {
                // Spawn line
                GameObject line = Instantiate(_line.gameObject, transform);
                line.transform.position = new Vector3(_line.transform.position.x, line.transform.position.y, line.transform.position.z);
                line.transform.rotation = _line.transform.rotation;

                // Set destination
                Vector3 startPosition = line.transform.localPosition;
                Vector3 targetPosition = new Vector3(line.transform.localPosition.x, line.transform.localPosition.y, line.transform.localPosition.z + _getAoE());

                // Start animation
                while (line.transform.localPosition != targetPosition)
                {
                    line.transform.localPosition = Vector3.MoveTowards(line.transform.localPosition, targetPosition, _lineSpd * Time.deltaTime);
                    yield return null;
                }

                // Clean up old line and restart after delay
                StartCoroutine(_destroyLine(line));
                yield return new WaitForSeconds(1.5f);
            }
        }

        private IEnumerator _destroyLine(GameObject line)
        {
            yield return new WaitForSeconds(1.5f);
            Destroy(line);
        }
    }
}
