using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        [Header("Sources")]
        public AudioSource MusicSource;
        public AudioSource SFXSource;

        [Header("Sound Effects")]
        public AudioClip WaveStart;
        public AudioClip BuildStructure;
        public AudioClip UpgradeStructure;
        public AudioClip DowngradeStructure;
        public AudioClip TakeDamage;

        public void OnWaveStart()
        {
            SFXSource.PlayOneShot(WaveStart);
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
