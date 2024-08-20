using GMTK_Jam.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.tvOS;
using UnityEngine.UIElements;

namespace GMTK_Jam.Buildings
{
    public class RicochetSubBullet : MonoBehaviour
    {
        private float _impactDistance;
        private float _projectileSpeed;
        private int _damage;

        public void FireSubBullet(float impactDistance, float projectileSpeed, int damage, EnemyBase target, float radius)
        {

            if (Vector3.Distance(transform.position, target.transform.position) > radius)
            {
                Destroy(gameObject);
            }
            else
            {
                _impactDistance = impactDistance;
                _projectileSpeed = projectileSpeed / 2;
                _damage = damage;
                StartCoroutine(_subBulletMoveTowardTarget(impactDistance, target));
            }
        }

        private IEnumerator _subBulletMoveTowardTarget(float impactDistance, EnemyBase target)
        {
            bool hasReachedDestination = false;
            while(target != null && !hasReachedDestination)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, _projectileSpeed * Time.deltaTime);
                transform.LookAt(target.transform);

                hasReachedDestination = Vector3.Distance(transform.position, target.transform.position) < _impactDistance;

                yield return null;
            }

            if(target != null)
                target.RegisterHit(_damage);
            Destroy(gameObject);
        }
    }
}
