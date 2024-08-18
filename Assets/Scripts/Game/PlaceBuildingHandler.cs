using GMTK_Jam.Buildings;
using GMTK_Jam.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GMTK_Jam.Player
{
    public class PlaceBuildingHandler : MonoBehaviour
    {
        public Transform BuildingParent;
        public string BuildTag = "Buildable";

        private BuildingPlacementArea _base;
        private BuildingData _data;
        private bool _isPlacing = false;
        private UnityAction<bool> _onPlacedCallback;

        private void Update()
        {
            if (!_isPlacing) return;

            if (!CameraHelper.IsPointerOverUIElement(CameraHelper.GetEventSystemRaycastResults()))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.tag == BuildTag)
                    {
                        _base.gameObject.SetActive(true);
                        _base.transform.position = hit.point;

                        if (Input.GetMouseButtonDown(0))
                        {
                            _isPlacing = false;
                            TowerBase tower = Instantiate(_data.Prefab, BuildingParent);
                            tower.transform.position = hit.point;
                            _reset();
                            _onPlacedCallback?.Invoke(true);
                        }
                    }
                    else
                        _base.gameObject.SetActive(false);
                }
                else
                    _base.gameObject.SetActive(false);
            }
        }

        public void StartPlacingBuilding(BuildingData data, UnityAction<bool> callback)
        {
            _reset();

            _data = data;
            _onPlacedCallback = callback;
            _base = Instantiate(_data.BasePrefab);
            _base.InitRadius(_data.Prefab.radius);
            _base.gameObject.SetActive(false);
            _isPlacing = true;
        }

        public void ClearPendingBuilding()
        {
            _isPlacing = false;
            _onPlacedCallback?.Invoke(false);
            _reset();
        }

        private void _reset()
        {
            if (_base != null)
            {
                Destroy(_base.gameObject);
                _base = null;
            }
        }
    }
}
