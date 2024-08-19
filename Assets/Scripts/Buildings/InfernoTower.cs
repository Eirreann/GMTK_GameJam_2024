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

        private int _baseUpgradeCost;

        protected override void Start()
        {
            base.Start();
            _baseUpgradeCost = upgradeCost;
        }

        public override void OnScrollValue(bool direction)
        {
            base.OnScrollValue(direction);
            float newCost = Mathf.Pow(_baseUpgradeCost, (_upgradeCostFactor * currentLevel));
            Debug.Log(newCost);
            upgradeCost = Mathf.RoundToInt(newCost);
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
    }
}
