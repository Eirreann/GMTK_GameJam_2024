using GMTK_Jam.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public class RicochetTower : TowerBase
    {
        protected override void _spawnBullet(int damage, EnemyBase target)
        {
            ProjectileBase bullet = pool.GetProjectile(currentLevel);
            (bullet as RicochetProjectile).SetRadius(radius);
            bullet.FireAtTarget(target, damage);

            if (OnFire != null)
                _audioSource.PlayOneShot(OnFire);
        }

        protected override void towerLookAt(Transform target = null)
        {
            base.towerLookAt(target);
            TurretRotation.localRotation = Quaternion.Euler(startRot.eulerAngles.z, TurretRotation.localRotation.eulerAngles.y, startRot.eulerAngles.z);
        }

        protected override int getDamage()
        {
            // TODO: Sort proper damage scaling
            int damage = baseDamage + (currentLevel * scaleFactor);
            return damage;
        }
    }
}
