using GMTK_Jam.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public class RicochetTower : TowerBase
    {
        protected override void towerLookAt(Transform target = null)
        {
            base.towerLookAt(target);
            TurretRotation.localRotation = Quaternion.Euler(startRot.eulerAngles.z, TurretRotation.localRotation.eulerAngles.y, startRot.eulerAngles.z);
        }
    }
}
