using GMTK_Jam.Enemy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public class InfernoTower : TowerBase
    {
        [Header("Inferno Tower Properties")]
        [SerializeField] private float _upgradeCostFactor = 1f;
        [SerializeField] private bool _prioritiseTargets = true;
        [SerializeField] private AudioClip _audioLoop;

        private int _baseUpgradeCost;
        private bool _audioPlaying = false;
        private Coroutine _audioCoroutine;
        private Stack<int> _previousUpgradeCosts = new Stack<int>();

        protected override void Start()
        {
            base.Start();
            _baseUpgradeCost = upgradeCost;
        }

        protected override void _spawnBullet(int damage, EnemyBase target)
        {
            ProjectileBase bullet = pool.GetProjectile(currentLevel);
            bullet.FireAtTarget(target, damage);

            _audioPlaying = true;
            if(_audioCoroutine == null) 
                _audioCoroutine = StartCoroutine(_playAudio());
        }

        protected override void Update()
        {
            base.Update();

            if (_audioPlaying && targets.Count == 0)
            {
                StopCoroutine(_audioCoroutine);
                _audioCoroutine = null;
                _audioSource.Stop();
                _audioPlaying = false;
            }
        }

        private IEnumerator _playAudio()
        {
            _audioSource.loop = false;
            _audioSource.PlayOneShot(OnFire);
            yield return new WaitForSeconds(OnFire.length);

            _audioSource.loop = true;
            _audioSource.clip = _audioLoop;
            _audioSource.Play();
        }

        public override void OnScrollValue(bool direction)
        {
            if (direction && !GameManager.Instance.CanAffordUpgrade(upgradeCost)) return;

            currentLevel += direction ? scaleFactor : -scaleFactor;
            if (currentLevel <= maxLevel && currentLevel >= 0)
            {
                Vector3 scaleAmount = new(0.1f, 0.1f, 0.1f);
                Model.localScale = Model.localScale += (direction ? scaleAmount : -scaleAmount);
                if (direction)
                {
                    GameManager.Instance.UpdatePlayerResource(-upgradeCost);
                    _previousUpgradeCosts.Push(upgradeCost);
                }
                else
                {
                    int refund = _previousUpgradeCosts.Pop();
                    GameManager.Instance.UpdatePlayerResource(refund);
                }
                AudioManager.Instance.OnUpgradeStructure(direction);
            }
            else
            {
                currentLevel = Mathf.Clamp(currentLevel, 0, maxLevel);
            }

            float newCost = Mathf.Pow(_baseUpgradeCost, (_upgradeCostFactor * currentLevel));
            upgradeCost = Mathf.RoundToInt(newCost);

            DamageText.text = "DMG " + getDamage().ToString();
            CostText.text = "Cost " + upgradeCost.ToString();
        }

        protected override void _onEnemyAdded()
        {
            base._onEnemyAdded();

            if(_prioritiseTargets)
                targets = targets.OrderBy(t => t.Priority).ToList();
        }

        protected override void _onEnemyRemoved()
        {
            base._onEnemyRemoved();

            if (targets.Count > 0 && _prioritiseTargets)
                targets = targets.OrderBy(t => t.Priority).ToList();
        }

        protected override int getDamage()
        {
            float damageFloat = Mathf.Pow((baseDamage * 1.5f), (scaleFactor * currentLevel));
            int damage = Mathf.RoundToInt(damageFloat);
            return damage;
        }

        protected override float getFireRate()
        {
            // do nothing! :D
            return baseAttackSpeed;
        }
    }
}
