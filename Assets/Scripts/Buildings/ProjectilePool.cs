using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

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

        public ProjectileBase GetProjectile(int level, bool updateScale = true)
        {
            if(_poolStack.Count == 0)
            {
                ProjectileBase instance = _createPoolInstance(level);
                return instance;
            }

            ProjectileBase nextInstance = _poolStack.Pop();
            nextInstance.gameObject.SetActive(true);
            _resetProjectilePos(nextInstance);
            if (updateScale)
                nextInstance.UpdateScale(level);
            else
                nextInstance.ResetScale();
            nextInstance.transform.parent = transform.parent;
            return nextInstance;
        }

        public void ReturnToPool(ProjectileBase instance)
        {
            _poolStack.Push(instance);

            _resetProjectilePos(instance);
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

        private ProjectileBase _createPoolInstance(int level = 0)
        {
            ProjectileBase instance = Instantiate(_projectileTemplate, _parent);
            instance.Init(this);
            instance.UpdateScale(level);
            instance.transform.parent = transform.parent;
            return instance;
        }

        private void _resetProjectilePos(ProjectileBase instance)
        {
            instance.transform.parent = _parent;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
        }
    }
}
