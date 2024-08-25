using GMTK_Jam.Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public class TowerBase : MonoBehaviour, IScrollInteractable
    {
        [Header("Attributes")]
        [SerializeField] public Vector2 damageRange;
        [SerializeField] public Vector2 attackSpeedRange;
        [SerializeField] public Vector2 aOERange;
        [SerializeField] protected int maxLevel = 10;
        [SerializeField] protected int upgradeCost = 1;

        [Header("Components")]
        public Transform Model;
        public Transform TurretRotation;
        public Transform BulletSpawnPos;
        public CanvasGroup StatsUI;
        public TextMeshProUGUI DamageText;
        public TextMeshProUGUI CostText;
        public AudioClip OnFire;
        [SerializeField] protected TrailRenderer _line;

        protected AudioSource _audioSource;
        protected List<EnemyBase> targets = new List<EnemyBase>();
        protected ProjectilePool pool;
        protected int currentLevel;
        protected int minLevel = 1;
        protected bool isFiring = false;
        protected Quaternion startRot;
        protected float _lineSpd = 150f;
        protected Coroutine _aoeCoroutine;

        private float _fireCooldown;
        private Coroutine _scaleCoroutine;

        protected virtual void Start()
        {
            _audioSource = GetComponentInChildren<AudioSource>();
            AudioManager.Instance.PlayBuildAudio(_audioSource);
            currentLevel = minLevel;

            startRot = TurretRotation.localRotation;
            pool = GetComponent<ProjectilePool>();
            pool.Setup(BulletSpawnPos);
            _updateAoE();

            DamageText.text = "DMG " + getDamage().ToString();
            CostText.text = "Cost " + upgradeCost.ToString();
        }

        public virtual void OnScrollValue(bool direction)
        {
            if (direction && !GameManager.Instance.CanAffordUpgrade(upgradeCost)) return;

            currentLevel += direction ? 1 : -1;
            if (currentLevel <= maxLevel && currentLevel >= minLevel)
            {
                // Update scale
                Vector3 scaleAmount = new(0.1f, 0.1f, 0.1f);
                if(_scaleCoroutine != null)
                    StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = StartCoroutine(_scaleBuilding(Model.localScale += (direction ? scaleAmount : -scaleAmount)));

                // Update radius
                _updateAoE();

                // Update UIs and play audio
                GameManager.Instance.UpdatePlayerResource(direction ? -upgradeCost : upgradeCost);
                AudioManager.Instance.OnUpgradeStructure(direction);

                // Update AoE
                _updateAoE();
            }
            else
            {
                currentLevel = Mathf.Clamp(currentLevel, minLevel, maxLevel);
            }

            DamageText.text = "DMG " + getDamage().ToString();
            CostText.text = "Cost " + upgradeCost.ToString();
        }

        private IEnumerator _scaleBuilding(Vector3 targetScale)
        {
            float time = 0;
            while(time < 1)
            {
                Model.localScale = Vector3.Lerp(Model.localScale, targetScale, time);
                time += Time.deltaTime;
                yield return null;
            }
            Model.localScale = targetScale;
        }

        protected virtual void Update()
        {
            targets.RemoveAll(t => t == null); // Remove any destroyed targets
            isFiring = targets.Count > 0;

            if(isFiring)
            {
                EnemyBase target = targets[0];
                towerLookAt(target.transform);
                _fireWeapon(target);
            }
            else
            {
                towerLookAt(null);

                // TODO: Make a proper method for tracking cooldowns.
                if(_fireCooldown > 0)
                {
                    _fireCooldown -= Time.deltaTime;
                    if (_fireCooldown < 0)
                        _fireCooldown = 0;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Enemy")
            {
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                targets.Add(enemy);
                _onEnemyAdded();
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Enemy")
            {
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                if (targets.Contains(enemy))
                {
                    targets.Remove(enemy);
                    _onEnemyRemoved();
                }
            }
        }

        protected virtual void _onEnemyAdded()
        {

        }

        protected virtual void _onEnemyRemoved()
        {

        }
        protected virtual int getDamage()
        {
            if (currentLevel == minLevel)
                return (int)Mathf.Round(damageRange.x);

            float levelRatio = (float)currentLevel / (float)maxLevel;
            float range = damageRange.y - damageRange.x;
            float damage = damageRange.x + (range * levelRatio);
            return (int)Mathf.Round(damage);
        }

        protected virtual float getFireRate()
        {
            if (currentLevel == minLevel)
                return attackSpeedRange.x;

            float levelRatio = (float)currentLevel / (float)maxLevel;
            float range = attackSpeedRange.y - attackSpeedRange.x;
            float newFireRate = attackSpeedRange.x + (range * levelRatio);
            return attackSpeedRange.x + (range * levelRatio);
        }

        protected virtual float _getAoE()
        {
            if (currentLevel == minLevel)
                return aOERange.x;

            float levelRatio = (float)currentLevel / (float)maxLevel;
            float range = aOERange.y - aOERange.x;
            float newFireRate = aOERange.x + (range * levelRatio);
            return aOERange.x + (range * levelRatio);
        }

        // TODO: Damage, fire rate, and AoE are now all using the same scaling algorithm, and should be simplified into a single function, e.g. "_getValueFromRange()"?
        protected virtual float _getValueFromRange(Vector2 range)
        {
            if (currentLevel == minLevel)
                return range.x;

            float levelRatio = (float)currentLevel / (float)maxLevel;
            float difference = range.y - range.x;
            float newFireRate = range.x + (difference * levelRatio);
            return range.x + (difference * levelRatio);
        }

        protected virtual void towerLookAt(Transform target = null)
        {
            if (target != null)
                TurretRotation.LookAt(target.transform);
            else
                TurretRotation.localRotation = startRot;
        }

        protected virtual void _spawnBullet(int damage, EnemyBase target)
        {
            ProjectileBase bullet = pool.GetProjectile(currentLevel);
            bullet.FireAtTarget(target, damage);

            if(OnFire != null)
                _audioSource.PlayOneShot(OnFire);
        }

        private void _fireWeapon(EnemyBase target)
        {
            if(_fireCooldown == 0)
            {
                _fireCooldown = getFireRate();
                _spawnBullet(getDamage(), target);
            }
            else
            {
                _fireCooldown -= Time.deltaTime;
                if( _fireCooldown < 0 )
                    _fireCooldown = 0;
                //Debug.Log(_fireCooldown.ToString());
            }
        }

        protected virtual void _updateAoE()
        {
            var boundaryCollider = GetComponent<SphereCollider>();
            boundaryCollider.radius = _getAoE();
            if(_aoeCoroutine != null)
                StopCoroutine(_aoeCoroutine);
            _line.Clear();
            _line.enabled = false;
            _aoeCoroutine = StartCoroutine(_drawAoE());
        }

        protected virtual IEnumerator _drawAoE()
        {
            bool isDrawing = true;
            _line.transform.position = new Vector3(transform.position.x + _getAoE(), _line.transform.position.y, transform.position.z);
            _line.enabled = true;
            while (isDrawing)
            {
                _line.transform.RotateAround(transform.position, Vector3.up, _lineSpd * Time.deltaTime);
                yield return null;
            }
        }

        public void OnHover(bool state)
        {
            StatsUI.alpha = state ? 1 : 0;
        }
    }
}
