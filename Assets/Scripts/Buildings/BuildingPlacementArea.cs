using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

namespace GMTK_Jam.Buildings
{
    public class BuildingPlacementArea : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        private Renderer _render;

        private int _lineSegments = 50;
        private float _radius;
        private bool _drawCircle = false;

        public void InitRadius(float radius)
        {
            _radius = radius;
            _drawCircle = true;
        }

        private void Update()
        {
            if(_drawCircle)
                _createCircle(_lineSegments, _radius);
        }

        private void _createCircle(int steps, float radius)
        {
            _lineRenderer.positionCount = steps;

            for (int currentStep = 0; currentStep < steps; currentStep++)
            {
                float circumferenceProgress = (float)currentStep / (steps - 1);

                float currentRadian = circumferenceProgress * 2 * Mathf.PI;

                float xScaled = Mathf.Cos(currentRadian);
                float zScaled = Mathf.Sin(currentRadian);

                float x = transform.position.x + (radius * xScaled);
                float y = transform.position.y;
                float z = transform.position.z + (radius * zScaled);

                Vector3 currentPosition = new Vector3(x, y, z);

                _lineRenderer.SetPosition(currentStep, currentPosition);
            }
        }
    }
}
