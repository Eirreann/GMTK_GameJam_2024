using GMTK_Jam.Buildings;
using GMTK_Jam.Util;
using System;
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
        public string NoBuildTag = "Unbuildable";
        public float RotationIncrement = 10f;

        private BuildingPlacementArea _base;
        private BuildingData _data;
        private bool _isPlacing = false;
        private UnityAction<bool> _onPlacedCallback;

        private void Start()
        {
            GameManager.Instance.Player.OnScroll.AddListener(_onPlayerScroll);
        }

        private void _onPlayerScroll(bool state)
        {
            if(_base != null)
            {
                float rotIncrement = RotationIncrement * (state ? 1f : -1f);
                Vector3 baseRot = _base.transform.rotation.eulerAngles;
                _base.transform.rotation = Quaternion.Euler(baseRot.x, baseRot.y + rotIncrement, baseRot.z);
            }
        }

        private void Update()
        {
            if (!_isPlacing) return;

            if (!CameraHelper.IsPointerOverUIElement(CameraHelper.GetEventSystemRaycastResults()))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.tag == BuildTag || hit.collider.tag == NoBuildTag)
                    {
                        bool canBuild = hit.collider.tag == BuildTag;
                        _base.gameObject.SetActive(true);
                        _base.transform.position = hit.point;
                        _base.UpdateMat(canBuild);

                        if (canBuild && Input.GetMouseButtonDown(0))
                        {
                            if (GameManager.Instance.CanAffordUpgrade(_data.Cost))
                            {
                                _isPlacing = false;
                                TowerBase tower = Instantiate(_data.Prefab, BuildingParent);
                                tower.transform.position = hit.point;
                                tower.transform.rotation = _base.transform.rotation;
                                _reset();
                                _onPlacedCallback?.Invoke(true);
                            }
                            else
                            {
                                ClearPendingBuilding();
                            }
                        }
                    }
                    else
                    {
                        _base.UpdateMat(false);
                        _base.gameObject.SetActive(false);
                    }
                }
                else
                {
                    _base.UpdateMat(false);
                    _base.gameObject.SetActive(false);
                }
            }
        }

        public void StartPlacingBuilding(BuildingData data, UnityAction<bool> callback)
        {
            _reset();

            _data = data;
            _onPlacedCallback = callback;
            _base = Instantiate(_data.BasePrefab);
            _base.InitRadius(_data.Prefab.aOERange);
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
