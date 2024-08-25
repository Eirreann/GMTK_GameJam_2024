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
                TurretRotation.localRotation = Quaternion.Euler(startRot.eulerAngles.z, TurretRotation.localRotation.eulerAngles.y, startRot.eulerAngles.z);

                // Rotate gun
                GunRotation.LookAt(target.transform);
                GunRotation.localRotation = Quaternion.Euler(GunRotation.localRotation.eulerAngles.x, _startGunRot.eulerAngles.y, _startGunRot.eulerAngles.z);
            }
            else
            {
                TurretRotation.localRotation = startRot;
                GunRotation.localRotation = _startGunRot;
            }
        }
    }
}
