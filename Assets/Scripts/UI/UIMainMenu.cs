using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GMTK_Jam.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        public Button StartBtn;
        public AudioMixer Mixer;

        private void Start()
        {
            StartBtn.onClick.AddListener(_onStart);

            Mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("Master", 1f)) * 20);
            Mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music", 1f)) * 20);
            Mixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SFX", 1f)) * 20);
        }

        private void _onStart()
        {
            SceneManager.LoadScene(1);
        }
    }
}
