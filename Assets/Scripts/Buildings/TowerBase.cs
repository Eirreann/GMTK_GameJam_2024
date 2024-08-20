using GMTK_Jam.Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public class TowerBase : MonoBehaviour, IScrollInteractable
    {
        [Header("Attributes")]
        [SerializeField] public int baseDamage = 1;
        [SerializeField] public float baseAttackSpeed = 5;
        [SerializeField] public Vector2 attackSpeedRange;
        [SerializeField] public float radius = 40f;
        [SerializeField] protected int scaleFactor = 1;
        [SerializeField] protected int maxLevel = 10;
        [SerializeField] protected int upgradeCost = 1;

        [Header("Components")]
        public Transform Model;
        public Transform TurretRotation;
        public Transform BulletSpawnPos;
        public CanvasGroup StatsUI;
        public TextMeshProUGUI DamageText;
        public TextMeshProUGUI CostText;
        [SerializeField] private TrailRenderer _line;
        public AudioClip OnFire;


        protected AudioSource _audioSource;
        protected List<EnemyBase> targets = new List<EnemyBase>();
        protected SphereCollider boundaryCollider;
        protected ProjectilePool pool;
        protected int currentLevel = 0;
        protected bool isFiring = false;
        protected Quaternion startRot;

        private float _lineSpd = 150f;
        private float _fireCooldown;

        protected virtual void Start()
        {
            _audioSource = GetComponentInChildren<AudioSource>();
            AudioManager.Instance.PlayBuildAudio(_audioSource);

            startRot = TurretRotation.localRotation;
            boundaryCollider = GetComponent<SphereCollider>();
            boundaryCollider.radius = radius;
            pool = GetComponent<ProjectilePool>();
            pool.Setup(BulletSpawnPos);
            StartCoroutine(_drawRadius());

            DamageText.text = "DMG " + getDamage().ToString();
            CostText.text = "Cost " + upgradeCost.ToString();
        }

        public virtual void OnScrollValue(bool direction)
        {
            if (direction && !GameManager.Instance.CanAffordUpgrade(upgradeCost)) return;

            currentLevel += direction ? scaleFactor : -scaleFactor;
            if (currentLevel <= maxLevel && currentLevel >= 0)
            {
                Vector3 scaleAmount = new(0.1f, 0.1f, 0.1f);
                Model.localScale = Model.localScale += (direction ? scaleAmount : -scaleAmount);
                GameManager.Instance.UpdatePlayerResource(direction ? -upgradeCost : upgradeCost);
                AudioManager.Instance.OnUpgradeStructure(direction);
            }
            else
            {
                currentLevel = Mathf.Clamp(currentLevel, 0, maxLevel);
            }

            DamageText.text = "DMG " + getDamage().ToString();
            CostText.text = "Cost " + upgradeCost.ToString();
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

            // TODO: Sort list of enemies by enemy with priority when required
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
            return baseDamage;
        }

        protected virtual float getFireRate()
        {
            if (currentLevel == 0)
                return attackSpeedRange.x;

            float levelRatio = ((float)currentLevel / (float)maxLevel);
            float range = attackSpeedRange.y - attackSpeedRange.x;
            float newFireRate = attackSpeedRange.x + (range * levelRatio);
            return attackSpeedRange.x + (range * levelRatio);
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

        private IEnumerator _drawRadius()
        {
            bool isDrawing = true;
            _line.transform.position = new Vector3(_line.transform.position.x + boundaryCollider.radius, _line.transform.position.y, _line.transform.position.z);
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
