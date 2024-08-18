using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public enum BuildingType { BASIC, CANNON, RICOCHET, INFERNO }
    [CreateAssetMenu(fileName = "Building Data", menuName = "GMTK_Jam/Building Data Object", order = 1)]
    public class BuildingDataSO : ScriptableObject
    {
        public List<BuildingData> Buildings;
    }

    [System.Serializable]
    public struct BuildingData
    {
        public BuildingType Type;
        public GameObject Prefab;
        public GameObject BasePrefab;
    }
}
