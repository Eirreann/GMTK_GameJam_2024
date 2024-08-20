using GMTK_Jam.Enemy;
using System;
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
            // (Overriding to skip this function's base functionality
        }

        protected override void _spawnBullet(int damage, EnemyBase target)
        {
            ProjectileBase bullet = pool.GetProjectile(currentLevel);
            (bullet as CannonProjectile).Setup(BulletSpawnPos, targets);
            bullet.FireAtTarget(null, damage);

            if (OnFire != null)
                _audioSource.PlayOneShot(OnFire);
        }

        protected override int getDamage()
        {
            // TODO: Sort proper damage scaling
            int damage = baseDamage + (int)(currentLevel * scaleFactor);
            return damage;
        }
    }
}
