using GMTK_Jam.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public class BasicProjectile : ProjectileBase
    {
        public override void FireAtTarget(EnemyBase target, int damage)
        {
            base.FireAtTarget(target, damage);
            _fireCoroutine = StartCoroutine(_moveTowardTarget());
        }
    }
}
