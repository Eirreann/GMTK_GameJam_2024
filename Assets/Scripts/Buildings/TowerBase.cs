using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public abstract class TowerBase : MonoBehaviour, IScrollInteractable
    {
        [Header("Attack Attributes")]
        [SerializeField] protected int _baseDamage = 1;
        [SerializeField] protected float _attackSpeed = 5;

        [Header("Components")]
        public Transform Model;
        public Transform TowerPivot;
        public Transform BulletSpawnPos;

        protected List<GameObject> _targets = new List<GameObject>();
        protected SphereCollider _collider;
        protected ProjectilePool _pool;
        protected bool _isFiring = false;

        private TrailRenderer _line;
        private float _lineSpd = 150f;
        private float _fireCooldown;

        private void Start()
        {
            _line = GetComponentInChildren<TrailRenderer>();
            _collider = GetComponent<SphereCollider>();
            _pool = GetComponent<ProjectilePool>();
            _pool.Setup(BulletSpawnPos);
            StartCoroutine(_drawRadius());
        }

        public void OnScrollValue(bool direction)
        {
            // Don't shrink less that 1
            if (transform.localScale.x <= 1 && !direction) return;

            Vector3 scaleAmount = new(0.1f, 0.1f, 0.1f);
            transform.localScale = transform.localScale += (direction ? scaleAmount : -scaleAmount);
        }

        private void Update()
        {
            _isFiring = _targets.Count > 0;

            if(_isFiring)
            {
                Transform target = _targets[0].transform;
                TowerPivot.LookAt(target);
                _fireWeapon(target);
            }
            else
                _fireCooldown = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Enemy")
                _targets.Add(other.gameObject);

            // TODO: Sort list of enemies by enemy with priority when required
        }


        private void OnTriggerExit(Collider other)
        {
            if (_targets.Contains(other.gameObject))
                _targets.Remove(other.gameObject);
        }
        private void _fireWeapon(Transform target)
        {
            if(_fireCooldown == 0)
            {
                _fireCooldown = _attackSpeed;
                ProjectileBase bullet = _pool.GetProjectile();
                bullet.FireAtTarget(target);
            }
            else
            {
                //_fireCooldown -= Time.deltaTime;
                if( _fireCooldown < 0 )
                    _fireCooldown = 0;
                //Debug.Log(_fireCooldown.ToString());
            }
        }

        private IEnumerator _drawRadius()
        {
            bool isDrawing = true;
            _line.transform.position = new Vector3(_line.transform.position.x + _collider.radius, _line.transform.position.y, _line.transform.position.z);
            while (isDrawing)
            {
                _line.transform.RotateAround(transform.position, Vector3.up, _lineSpd * Time.deltaTime);
                yield return null;
            }
        }
    }
}
