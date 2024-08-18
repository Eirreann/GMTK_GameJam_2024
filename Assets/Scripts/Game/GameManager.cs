using GMTK_Jam;
using GMTK_Jam.Buildings;
using GMTK_Jam.Enemy;
using GMTK_Jam.Player;
using GMTK_Jam.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GMTK_Jam
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [Header("Player")]
        public PlayerController Player;
        public PlayerBase PlayerBase;
        public int PlayerMaxHealth;
        public int PlayerMaxScale;

        [Header("Gameplay")]
        public EnemySpawnManager EnemyManager;
        public EnemyWavesObject WavesSettings;
        public BuildingDataSO BuildingData;

        [Header("UI")]
        public GameObject TempPause;
        public UIScaleBar ScaleBar;

        private PlaceBuildingHandler _buildingHandler;
        private int _currentHealth;
        private int _currentScale;
        private bool _paused = false;

        private void Start()
        {
            Player.InitInputs();
            _buildingHandler = GetComponent<PlaceBuildingHandler>();
            Cursor.lockState = CursorLockMode.Confined;
            _currentHealth = PlayerMaxHealth;
            _currentScale = PlayerMaxScale;
            UpdatePlayerResource(0);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _paused = !_paused;
                PauseGame(_paused);
                TempPause.SetActive(_paused);
                // TODO: Proper pause menu?
            }
        }

        public void PauseGame(bool state)
        {
            Time.timeScale = state ? 0 : 1;
            Player.EnableMovement(!state);
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Confined;
        }

        public void UpdatePlayerHealth(int mod)
        {
            _currentHealth += mod;
            PlayerBase.UpdatePlayerHealthUI(_currentHealth);
            //Debug.Log("Player health modified by " + mod.ToString());
        }

        public void UpdatePlayerResource(int mod, bool updateMax = false)
        {
            _currentScale += mod;
            if (updateMax)
                PlayerMaxScale += mod;
            ScaleBar.UpdateScaleUI(mod, _currentScale, PlayerMaxScale);
        }

        public void OpenBuyMenu()
        {
            _buildingHandler.StartPlacingBuilding(BuildingData.Buildings[0]);
        }

        public bool CanAffordUpgrade(int cost)
        {
            return cost <= _currentScale;
        }
    }
}
