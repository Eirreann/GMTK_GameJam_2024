using GMTK_Jam;
using GMTK_Jam.Enemy;
using GMTK_Jam.Player;
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

        [Header("Gameplay")]
        public EnemyWavesObject WavesSettings;
        public EnemySpawnManager EnemyManager;

        [Header("UI")]
        public GameObject TempPause;

        private int _currentHealth;
        private bool _paused = false;

        private void Start()
        {
            Player.InitInputs();
            Cursor.lockState = CursorLockMode.Confined;
            _currentHealth = PlayerMaxHealth;
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
            // TODO
            _currentHealth += mod;
            PlayerBase.UpdatePlayerHealthUI(_currentHealth);
            Debug.Log("Player health modified by " + mod.ToString());
        }

        public void UpdatePlayerResource(int mod)
        {
            // TODO
        }
    }
}
