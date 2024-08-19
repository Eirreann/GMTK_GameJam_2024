using GMTK_Jam;
using GMTK_Jam.Buildings;
using GMTK_Jam.Enemy;
using GMTK_Jam.Player;
using GMTK_Jam.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK_Jam
{
    public enum GameState { NOT_STARTED, ACTIVE, PAUSED, ENDED}

    public class GameManager : MonoSingleton<GameManager>
    {
        public GameState State = GameState.NOT_STARTED;

        [Header("Player")]
        public PlayerController Player;
        public PlayerBase PlayerBase;
        public int PlayerMaxHealth;
        public int PlayerMaxScale;

        [Header("Gameplay")]
        public EnemySpawnManager EnemyManager;
        public EnemyWavesObject WavesData;
        public BuildingDataSO BuildingData;

        [Header("UI")]
        public GameObject TempPause;
        public GameObject GameOverUI;
        public UIScaleBar ScaleBar;
        public Button BuyBtn;
        public TextMeshProUGUI BuyBtnText;
        public UITowerShop TowerShop;

        private PlaceBuildingHandler _buildingHandler;
        private int _currentHealth;
        private int _currentScale;
        private int _waveIndex = 0;
        private bool _paused = false;

        private void Start()
        {
            Player.InitInputs();
            _buildingHandler = GetComponent<PlaceBuildingHandler>();
            Cursor.lockState = CursorLockMode.Confined;
            _currentHealth = PlayerMaxHealth;
            _currentScale = PlayerMaxScale;
            UpdatePlayerResource(0);

            BuyBtn.onClick.AddListener(OpenBuyMenu);
            StartGame();
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

        public void StartGame()
        {
            State = GameState.ACTIVE;
            EnemyManager.StartWave(WavesData.Waves[_waveIndex], _onWaveCompleted);
        }

        private void _onWaveCompleted()
        {
            if(State == GameState.ACTIVE)
            {
                _waveIndex++;
                if(_waveIndex < WavesData.Waves.Count)
                    EnemyManager.StartWave(WavesData.Waves[_waveIndex], _onWaveCompleted);
                else
                    _endGame();
            }
        }

        public void PauseGame(bool state)
        {
            Time.timeScale = state ? 0 : 1;
            Player.EnableMovement(!state);
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Confined;
            State = state ? GameState.PAUSED : GameState.ACTIVE;
        }

        public void UpdatePlayerHealth(int mod)
        {
            _currentHealth += mod;
            PlayerBase.UpdatePlayerHealthUI(_currentHealth);

            if(_currentHealth <= 0)
            {
                // TODO: Game over logic
                _endGame();
            }
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
            //_buildingHandler.StartPlacingBuilding(BuildingData.Buildings[0]);
            if (TowerShop.gameObject.activeInHierarchy || State != GameState.ACTIVE) return;

            TowerShop.gameObject.SetActive(true);
            TowerShop.ShowBuyMenu(BuildingData.Buildings, _spawnBuildingPlacement);
            _buildingHandler.ClearPendingBuilding();
            BuyBtnText.text = "Cancel";
            PauseGame(true);
        }

        public bool CanAffordUpgrade(int cost)
        {
            return cost <= _currentScale;
        }

        public EnemyBase GetEnemyObjectByType(EnemyType type)
        {
            return WavesData.ReturnEnemyObject(type);
        }

        private void _endGame()
        {
            State = GameState.ENDED;
            GameOverUI.SetActive(true);
        }

        private void _spawnBuildingPlacement(BuildingData data)
        {
            _buildingHandler.StartPlacingBuilding(data, (state) =>
            {
                if(state)
                    UpdatePlayerResource(-data.Cost);

                BuyBtnText.text = "Buy";
            });
        }
    }
}
