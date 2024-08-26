using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GMTK_Jam
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        public AudioMixer Mixer;

        [Header("Sources")]
        public AudioSource WaveStartSource;
        public AudioSource MusicSource;
        public AudioSource SFXSource;

        [Header("Sound Effects")]
        public AudioClip BuildStructure;
        public AudioClip UpgradeStructure;
        public AudioClip DowngradeStructure;
        public AudioClip TakeDamage;

        private void Start()
        {
            Mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("Master", 1f)) * 20);
            Mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music", 1f)) * 20);
            Mixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("SFX", 1f)) * 20);
        }

        public void OnWaveStart()
        {
            WaveStartSource.Play();
        }

        public void OnTakeDamage(AudioSource source)
        {
            SFXSource.PlayOneShot(TakeDamage);
        }

        public void OnUpgradeStructure(bool state)
        {
            SFXSource.Stop();
            SFXSource.PlayOneShot(state ? UpgradeStructure : DowngradeStructure);
        }

        public void PlayBuildAudio(AudioSource source)
        {
            source.PlayOneShot(BuildStructure);
        }
    }
}
