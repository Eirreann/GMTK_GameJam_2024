using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Buildings
{
    public class BuildingPlacementArea : MonoBehaviour
    {
        private Renderer _render;

        private void Start()
        {
            _render = GetComponent<Renderer>();
        }

        public void CanPlaceBuilding(bool state)
        {
            _render.material.color = state ? Color.green : Color.red;
        }
    }
}
