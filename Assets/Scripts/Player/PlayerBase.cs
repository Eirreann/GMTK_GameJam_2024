using GMTK_Jam.Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK_Jam.Player
{
    public class PlayerBase : MonoBehaviour, IScrollInteractable
    {
        [SerializeField] private Image _playerHealthbar;

        public void OnScrollValue(bool direction)
        {
            // TODO
        }

        public void UpdatePlayerHealthUI(int health)
        {
            _playerHealthbar.fillAmount = (float)health / (float)GameManager.Instance.PlayerMaxHealth;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<EnemyBase>() != null)
            {
                other.GetComponent<EnemyBase>().OnEnemyCollideWithBase();
            }
        }
    }
}
