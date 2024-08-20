using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GMTK_Jam.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        public Button StartBtn;

        private void Start()
        {
            StartBtn.onClick.AddListener(_onStart);
        }

        private void _onStart()
        {
            // TODO: Set up intro card sequence
            SceneManager.LoadScene(1);
        }
    }
}
