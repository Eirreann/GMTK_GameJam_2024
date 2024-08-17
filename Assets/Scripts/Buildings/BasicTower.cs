using GMTK_Jam.Buildings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public class BasicTower : TowerBase
    {
        [Header("Additional rotation")]
        public Transform GunRotation;

        private Quaternion _startGunRot;

        protected override void Start()
        {
            base.Start();
            _startGunRot = GunRotation.localRotation;
        }

        protected override void towerLookAt(Transform target = null)
        {
            if (target != null)
            {
                // Rotate turret base
                TurretRotation.LookAt(target.transform);
                TurretRotation.localRotation = Quaternion.Euler(TurretRotation.localRotation.eulerAngles.x, startRot.eulerAngles.y, startRot.eulerAngles.z);

                // Rotate gun
                GunRotation.LookAt(target.transform);
                GunRotation.localRotation = Quaternion.Euler(_startGunRot.eulerAngles.x, GunRotation.localRotation.eulerAngles.y, _startGunRot.eulerAngles.z);
            }
            else
            {
                TurretRotation.localRotation = startRot;
                GunRotation.localRotation = _startGunRot;
            }
        }

        protected override int getDamage()
        {
            int damage = baseDamage + scaleFactor;
            return damage;
        }

        protected override float getFireRate()
        {
            float fireRate = baseAttackSpeed;
            float mod = (scaleFactor / maxScale) * 2;
            fireRate += mod;
            Debug.Log(string.Format("{0}, {1}", fireRate, mod));
            return fireRate;
        }
    }
}
