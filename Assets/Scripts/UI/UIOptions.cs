using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace GMTK_Jam.UI
{
    public class UIOptions : MonoBehaviour
    {
        [Header("Audio")]
        public AudioMixer Mixer;
        public Slider Master;
        public Slider Music;
        public Slider SFX;

        public void Start()
        {
            Master.onValueChanged.AddListener(_updateMasterAudio);
            Music.onValueChanged.AddListener(_updateMusicAudio);
            SFX.onValueChanged.AddListener(_updateSFXAudio);

            Master.value = PlayerPrefs.GetFloat("Master", 1f);
            Music.value = PlayerPrefs.GetFloat("Music", 1f);
            SFX.value = PlayerPrefs.GetFloat("SFX", 1f);
        }

        private void _updateMasterAudio(float value)
        {
            if (value == 0.0f)
                value = 0.0001f;

            Mixer.SetFloat("Master", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat("Master", value);
            //Debug.Log(value);
        }

        private void _updateMusicAudio(float value)
        {
            if (value == 0.0f)
                value = 0.0001f;

            Mixer.SetFloat("Music", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat("Music", value);
            //Debug.Log(value);
        }

        private void _updateSFXAudio(float value)
        {
            if (value == 0.0f)
                value = 0.0001f;

            Mixer.SetFloat("SFX", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat("SFX", value);
            //Debug.Log(value);
        }
    }
}