using GMTK_Jam.Buildings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Player
{
    public class PlaceBuildingHandler : MonoBehaviour
    {
        public Transform BuildingParent;
        public string BuildTag = "Buildable";

        private GameObject _base;
        private BuildingData _data;
        private bool _isPlacing = false;

        private void Update()
        {
            if (!_isPlacing) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.tag == BuildTag)
                {
                    _base.SetActive(true);
                    _base.transform.position = hit.point;

                    if (Input.GetMouseButtonDown(0))
                    {
                        _isPlacing = false;
                        GameObject tower = Instantiate(_data.Prefab, BuildingParent);
                        tower.transform.position = hit.point;
                        Destroy(_base);
                    }
                }
                else
                    _base.SetActive(false);
            }
        }

        public void StartPlacingBuilding(BuildingData data)
        {
            if(_isPlacing) return;

            _data = data;
            _base = Instantiate(_data.BasePrefab);
            _base.SetActive(false);
            _isPlacing = true;
        }
    }
}
