using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private int _initPoolSize = 10;
        [SerializeField] private ProjectileBase _projectileTemplate;

        private Stack<ProjectileBase> _poolStack;
        private Transform _parent;

        public void Setup(Transform projectileParent)
        {
            _parent = projectileParent;
            _setupPool();
        }

        public ProjectileBase GetProjectile()
        {
            if(_poolStack.Count == 0)
            {
                return _createPoolInstance();
            }

            ProjectileBase nextInstance = _poolStack.Pop();
            nextInstance.gameObject.SetActive(true);
            return nextInstance;
        }

        public void ReturnToPool(ProjectileBase instance)
        {
            _poolStack.Push(instance);
            instance.transform.position = Vector3.zero; ;
            instance.gameObject.SetActive(false);
        }

        private void _setupPool()
        {
            _poolStack = new Stack<ProjectileBase>();
            for(int i = 0; i < _initPoolSize; i++)
            {
                ProjectileBase instance = _createPoolInstance();
                instance.gameObject.SetActive(false);
                _poolStack.Push(instance);
            }
        }

        private ProjectileBase _createPoolInstance()
        {
            ProjectileBase instance = Instantiate(_projectileTemplate, _parent);
            instance.Init(this);
            return instance;
        }
    }
}
