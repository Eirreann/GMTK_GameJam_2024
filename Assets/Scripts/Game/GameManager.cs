using GMTK_Jam;
using GMTK_Jam.Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GMTK_Jam
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public PlayerController Player;
        public Transform PlayerBase;
        public GameObject TempPause;

        private bool _paused = false;

        private void Start()
        {
            Player.InitInputs();
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _paused = !_paused;
                PauseGame(_paused);
                TempPause.SetActive(_paused);
            }
        }

        public void PauseGame(bool state)
        {
            Time.timeScale = state ? 0 : 1;
            Player.EnableMovement(!state);
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Confined;
        }
    }
}
