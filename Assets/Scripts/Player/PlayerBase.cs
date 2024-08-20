using GMTK_Jam.Enemy;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK_Jam.Player
{
    public class PlayerBase : MonoBehaviour, IScrollInteractable
    {
        [SerializeField] private Image _playerHealthbar;
        [SerializeField] private TextMeshProUGUI _playerHealthText;

        private AudioSource _audio;

        private void Start()
        {
            _audio = GetComponentInChildren<AudioSource>();
        }

        public void OnScrollValue(bool direction)
        {
            // TODO
        }

        public void UpdatePlayerHealthUI(int health)
        {
            _playerHealthbar.fillAmount = (float)health / (float)GameManager.Instance.PlayerMaxHealth;
            _playerHealthText.text = $"{health}/\n{GameManager.Instance.PlayerMaxHealth}";

            if (health < 0)
                AudioManager.Instance.OnTakeDamage(_audio);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<EnemyBase>() != null)
            {
                other.GetComponent<EnemyBase>().OnEnemyCollideWithBase();
            }
        }

        public void OnHover(bool state)
        {

        }
    }
}
