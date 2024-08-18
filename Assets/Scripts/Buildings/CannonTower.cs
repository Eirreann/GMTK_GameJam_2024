using GMTK_Jam.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public class CannonTower : TowerBase
    {
        protected override void towerLookAt(Transform target = null)
        {
            // DOES NOTHING :D
        }

        protected override void _spawnBullet(int damage, EnemyBase target)
        {
            Debug.Log("Bullet spawned");
            ProjectileBase bullet = pool.GetProjectile();
            (bullet as CannonProjectile).SetEnemiesInRange(targets);
            bullet.FireAtTarget(null, damage);
        }
    }
}
