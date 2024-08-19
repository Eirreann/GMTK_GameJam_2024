using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Enemy
{
    public enum EnemyType { BASIC, BASIC_ELITE, TANK }

    [CreateAssetMenu(fileName = "Enemy Waves", menuName = "GMTK_Jam/Enemy Waves Object", order = 1)]
    public class EnemyWavesObject : ScriptableObject
    {
        [Header("Enemy Prefabs")]
        public BasicEnemy Basic_Enemy;
        public BasicEnemy Basic_Enemy_Elite;
        public TankEnemy Tank_Enemy;

        [Header("Waves")]
        public List<WaveOptions> Waves;

        public EnemyBase ReturnEnemyObject(EnemyType enemyType)
        {
            EnemyBase enemy = null;
            switch (enemyType)
            {
                case EnemyType.BASIC:
                    enemy = Basic_Enemy;
                    break;
                case EnemyType.BASIC_ELITE:
                    enemy = Basic_Enemy_Elite;
                    break;
                case EnemyType.TANK:
                    enemy = Tank_Enemy;
                    break;
            }
            return enemy;
        }
    }

    [System.Serializable]
    public struct WaveOptions
    {
        [Header("Wave Customisation")]
        [Range(0, 60)]
        public int WaveTime;
        public List<BatchSettings> Batches;

        [Header("UI")]
        public bool HasTooltip;
        [TextArea(2, 10)]
        public string TooltipText;
    }

    [System.Serializable]
    public struct BatchSettings
    {
        [Header("Batch Customisation")]
        [Range(0, 60)]
        public int BatchTime;
        public EnemyType Enemy_Type;
        public int EnemyCount;
    }
}
