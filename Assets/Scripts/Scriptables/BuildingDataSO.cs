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
        [Header("Details")]
        public string Name;
        [TextArea(2, 10)]
        public string Desc;
        public int Cost;
        public BuildingType Type;
        [Header("Prefabs")]
        public TowerBase Prefab;
        public BuildingPlacementArea BasePrefab;
        [Header("Sprites")]
        public Sprite BaseIcon;
        public Sprite UpgradeIcon;
        public Sprite DowngradeIcon;
    }
}
