using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Enemy
{
    public enum EnemyType { BASIC, ADVANCED }

    [CreateAssetMenu(fileName = "Enemy Waves", menuName = "GMTK_Jam/Enemy Waves Object", order = 1)]
    public class EnemyWavesObject : ScriptableObject
    {
        [Header("Enemy Prefabs")]
        public BasicEnemy Basic_Enemy;

        [Header("Waves")]
        public List<WaveOptions> Waves;
    }

    [System.Serializable]
    public struct WaveOptions
    {
        [Range(0, 60)]
        public int WaveTime;
        public List<BatchSettings> Batches;
        [TextArea(2, 10)]
        public string TooltipText;
    }

    [System.Serializable]
    public struct BatchSettings
    {
        [Range(0, 60)]
        public int BatchTime;
        public EnemyType Enemy_Type;
        public int EnemyCount;
    }
}
