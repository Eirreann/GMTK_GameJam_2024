using GMTK_Jam.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public class InfernoProjectile : ProjectileBase
    {
        private float maxScale = 1.85f; // Hardcoding this for now. >.<

        public override void FireAtTarget(EnemyBase target, int damage)
        {
            base.FireAtTarget(target, damage);
            GetComponent<TrailRenderer>().Clear();
            GetComponent<TrailRenderer>().widthMultiplier = 1 + (transform.localScale.x / maxScale);
            _fireCoroutine = StartCoroutine(_moveTowardTarget());
        }

        protected override void _returnToPool()
        {
            base._returnToPool();
            GetComponent<TrailRenderer>().Clear();
        }
    }
}
