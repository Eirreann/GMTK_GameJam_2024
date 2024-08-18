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
        [SerializeField] protected int baseDamage = 1;
        [SerializeField] protected float baseAttackSpeed = 5;
        [SerializeField] protected Vector2 minMaxAttackSpeed = new Vector2(1, 10);
        [SerializeField] protected int maxScale = 10;
        [SerializeField] protected int upgradeCost = 1;

        [Header("Components")]
        public Transform Model;
        public Transform TurretRotation;
        public Transform BulletSpawnPos;
        public TextMeshProUGUI DamageText;

        protected List<EnemyBase> targets = new List<EnemyBase>();
        protected SphereCollider boundaryCollider;
        protected ProjectilePool pool;
        protected int scaleFactor = 0;
        protected bool isFiring = false;
        protected Quaternion startRot;

        private TrailRenderer _line;
        private float _lineSpd = 150f;
        private float _fireCooldown;

        protected virtual void Start()
        {
            startRot = TurretRotation.localRotation;
            _line = GetComponentInChildren<TrailRenderer>();
            boundaryCollider = GetComponent<SphereCollider>();
            pool = GetComponent<ProjectilePool>();
            pool.Setup(BulletSpawnPos);
            StartCoroutine(_drawRadius());

            DamageText.text = "Damage-" + getDamage().ToString();
        }

        public virtual void OnScrollValue(bool direction)
        {
            if (direction && !GameManager.Instance.CanAffordUpgrade(upgradeCost)) return;

            scaleFactor += direction ? 1 : -1;
            if (scaleFactor <= maxScale && scaleFactor >= 0)
            {
                Vector3 scaleAmount = new(0.1f, 0.1f, 0.1f);
                Model.localScale = Model.localScale += (direction ? scaleAmount : -scaleAmount);
                GameManager.Instance.UpdatePlayerResource(direction ? -upgradeCost : upgradeCost);
            }
            else
            {
                scaleFactor = Mathf.Clamp(scaleFactor, 0, 10);
            }

            DamageText.text = "Damage-" + getDamage().ToString();
        }

        private void Update()
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
                //_fireCooldown = 0;
            }
                
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Enemy")
            {
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                targets.Add(enemy);
            }

            // TODO: Sort list of enemies by enemy with priority when required
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Enemy")
            {
                EnemyBase enemy = other.GetComponent<EnemyBase>();
                if (targets.Contains(enemy))
                    targets.Remove(enemy);
            }
        }

        protected virtual int getDamage()
        {
            return baseDamage;
        }

        protected virtual float getFireRate()
        {
            return baseAttackSpeed;
        }

        protected virtual void towerLookAt(Transform target = null)
        {
            if (target != null)
                TurretRotation.LookAt(target.transform);
            else
                TurretRotation.localRotation = startRot;
        }

        private void _fireWeapon(EnemyBase target)
        {
            if(_fireCooldown == 0)
            {
                _fireCooldown = getFireRate();
                ProjectileBase bullet = pool.GetProjectile();
                bullet.FireAtTarget(target, getDamage());
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
    }
}
