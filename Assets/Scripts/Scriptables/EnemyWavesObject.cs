using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Enemy
{
    public enum EnemyType { BASIC, BASIC_ELITE, TANK, SPAWNER, EXPLODER, EXPLODER_ELITE, TANK_ELITE, SPAWNER_ELITE }

    [CreateAssetMenu(fileName = "Enemy Waves", menuName = "GMTK_Jam/Enemy Waves Object", order = 1)]
    public class EnemyWavesObject : ScriptableObject
    {
        [Header("Enemy Prefabs")]
        public BasicEnemy Basic_Enemy;
        public BasicEnemy Basic_Enemy_Elite;
        public TankEnemy Tank_Enemy;
        public TankEnemy Tank_Enemy_Elite;
        public SpawnerEnemy Spawner_Enemy;
        public SpawnerEnemy Spawner_Enemy_Elite;
        public ExploderEnemy Exploder_Enemy;
        public ExploderEnemy Exploder_Enemy_Elite;

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
                case EnemyType.TANK_ELITE:
                    enemy = Tank_Enemy_Elite;
                    break;
                case EnemyType.SPAWNER:
                    enemy = Spawner_Enemy;
                    break;
                case EnemyType.SPAWNER_ELITE:
                    enemy = Spawner_Enemy_Elite;
                    break;
                case EnemyType.EXPLODER:
                    enemy = Exploder_Enemy;
                    break;
                case EnemyType.EXPLODER_ELITE:
                    enemy = Exploder_Enemy_Elite;
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

        [Header("Gameplay")]
        public bool UnlocksPath;

        [Header("UI")]
        [TextArea(1, 10)]
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
