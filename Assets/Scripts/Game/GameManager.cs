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
using UnityEngine.SceneManagement;
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

        [Header("Pathing")]
        [SerializeField] private List<PathChunk> _chunks;
        private int _chunkIndex = 0;

        [Header("UI")]
        public UIIntroScreen IntroScreen;
        public GameObject PauseScreen;
        public GameObject TutorialScreen;
        public UIScaleBar ScaleBar;
        public Button BuyBtn;
        public Button SpeedUpBtn;
        public TextMeshProUGUI BuyBtnText;
        public UITowerShop TowerShop;
        public Color SpeedUpBtnEnabledColour;
        public Color SpeedUpBtnDisabledColour;

        [Header("Game Over")]
        public GameObject GameOverWinUI;
        public GameObject GameOverLoseUI;
        public List<Button> EndGameButtons;

        [Header("Debug")]
        public bool DontEndGameOnDeath = false;
        public bool SkipIntro = false;

        private PlaceBuildingHandler _buildingHandler;
        private int _currentHealth;
        private int _currentScale;
        private int _waveIndex = 0;
        private bool _paused = false;
        private bool _isSpedUp = false;
        private bool _isBuyMenuOpen = false;
        private bool _isPlacingBuilding = false;
        private float _currentTimescale = 1;

        private void Start()
        {
            Player.InitInputs();
            _buildingHandler = GetComponent<PlaceBuildingHandler>();
            //Cursor.lockState = CursorLockMode.Confined;
            _currentHealth = PlayerMaxHealth;
            _currentScale = PlayerMaxScale;
            UpdatePlayerResource(0);
            PlayerBase.UpdatePlayerHealthUI(_currentHealth);

            BuyBtn.onClick.AddListener(OpenBuyMenu);
            SpeedUpBtn.onClick.AddListener(_speedUpGame);
            EndGameButtons.ForEach(b => b.onClick.AddListener(_onRestart));


            //PlayerPrefs.SetInt("DoneTutorial", 0);
            if (PlayerPrefs.GetInt("DoneTutorial", 0) == 0)
            {
                TutorialScreen.SetActive(true);
                PlayerPrefs.SetInt("DoneTutorial", 1);
            }

            if (SkipIntro)
                StartGame();
            else
            {
                IntroScreen.gameObject.SetActive(true);
                IntroScreen.OnStart(() => StartGame());
            }
        }

        private void _speedUpGame()
        {
            _isSpedUp = !_isSpedUp;
            _currentTimescale = _isSpedUp ? 5 : 1;
            Time.timeScale = _currentTimescale;

            if (_isSpedUp)
            {
                SpeedUpBtn.GetComponentInChildren<TextMeshProUGUI>().text = ">>";
                SpeedUpBtn.GetComponent<Image>().color = SpeedUpBtnEnabledColour;
            }
            else
            {
                SpeedUpBtn.GetComponentInChildren<TextMeshProUGUI>().text = ">";
                SpeedUpBtn.GetComponent<Image>().color = SpeedUpBtnDisabledColour;
            }
        }

        private void _onRestart()
        {
            SceneManager.LoadScene(0);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (_isBuyMenuOpen) return;

                _paused = !_paused;
                PauseGame(_paused);
                PauseScreen.SetActive(_paused);
                // TODO: Proper pause menu?
            }

            if (Input.GetKeyUp(KeyCode.B))
            {
                OpenBuyMenu();
            }
        }

        public void StartGame()
        {
            State = GameState.ACTIVE;

            _chunks.ForEach(chunk => chunk.BuildSurface.SetActive(false));
            _chunks[_chunkIndex].BuildSurface.SetActive(true);
            EnemyManager.UpdateSpawnPoint(_chunks[_chunkIndex].SpawnPoint);
            EnemyManager.StartWave(_waveIndex + 1, WavesData.Waves[_waveIndex], _onWaveCompleted);
        }

        public void PauseGame(bool state)
        {
            if (State == GameState.ENDED || _isBuyMenuOpen) return;

            Time.timeScale = state ? 0 : _currentTimescale;
            Player.EnableMovement(!state);
            //Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Confined;
            State = state ? GameState.PAUSED : GameState.ACTIVE;
        }

        public void UpdatePlayerHealth(int mod)
        {
            if (State == GameState.ENDED) return;

            _currentHealth += mod;
            PlayerBase.UpdatePlayerHealthUI(_currentHealth);

            if (_currentHealth <= 0)
            {
                // TODO: Game over logic
                _endGame(false);
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
            PauseGame(!_isBuyMenuOpen);
            _isBuyMenuOpen = !_isBuyMenuOpen;

            if (_isBuyMenuOpen)
            {
                TowerShop.gameObject.SetActive(true);
                TowerShop.ShowBuyMenu(BuildingData.Buildings, _spawnBuildingPlacement);
                _buildingHandler.ClearPendingBuilding();
                BuyBtnText.text = "Cancel";
                //PauseGame(true);
            }
            else
            {
                TowerShop.gameObject.SetActive(false);
                _buildingHandler.ClearPendingBuilding();
                BuyBtnText.text = "Buy";

                if (!_isPlacingBuilding)
                    PauseGame(false);
            }
        }

        public bool CanAffordUpgrade(int cost)
        {
            return cost <= _currentScale;
        }

        public EnemyBase GetEnemyObjectByType(EnemyType type)
        {
            return WavesData.ReturnEnemyObject(type);
        }

        public void PreviewCost(bool state, int cost = 0)
        {
            if (state)
            {
                if (CanAffordUpgrade(cost))
                    ScaleBar.PreviewCost(cost, PlayerMaxScale);
                else
                    ScaleBar.PreviewCost(false);
            }
            else
                ScaleBar.PreviewCost(false);
        }

        private void _onWaveCompleted()
        {
            if(State == GameState.ACTIVE)
            {
                _waveIndex++;
                if(_waveIndex < WavesData.Waves.Count)
                {
                    if (WavesData.Waves[_waveIndex].UnlocksPath)
                    {
                        _chunkIndex++;
                        if(_chunkIndex < _chunks.Count)
                            EnemyManager.UpdateSpawnPoint(_chunks[_chunkIndex].SpawnPoint);
                        _chunks[_chunkIndex].BuildSurface.SetActive(true);
                    }
                    EnemyManager.StartWave(_waveIndex + 1, WavesData.Waves[_waveIndex], _onWaveCompleted);
                }
                else
                    _endGame(true);
            }
        }

        private void _endGame(bool state)
        {
            if (DontEndGameOnDeath) return;

            State = GameState.ENDED;
            Player.DeregisterInputs();
            Player.enabled = false;

            if (_isSpedUp)
                _speedUpGame();

            if (state)
                GameOverWinUI.SetActive(true);
            else
                GameOverLoseUI.SetActive(true);
        }

        private void _spawnBuildingPlacement(BuildingData data)
        {
            _isBuyMenuOpen = false;

            if (data.Prefab != null)
            {
                _isPlacingBuilding = true;
                _buildingHandler.StartPlacingBuilding(data, (state) =>
                {
                    if (state)
                        UpdatePlayerResource(-data.Cost);

                    _isPlacingBuilding = false;
                    BuyBtnText.text = "Buy";
                });
            }
            else
            {
                BuyBtnText.text = "Buy";
            }
        }
    }

    [System.Serializable]
    public struct PathChunk
    {
        public EnemySpawnPoint SpawnPoint;
        public GameObject BuildSurface;
    }
}
