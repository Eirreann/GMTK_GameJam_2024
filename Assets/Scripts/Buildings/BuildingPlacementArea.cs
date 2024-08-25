using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.HableCurve;

namespace GMTK_Jam.Buildings
{
    public class BuildingPlacementArea : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private LineRenderer _lineRendererMax;

        [Header("Model")]
        [SerializeField] private List<Renderer> _renderers;
        [SerializeField] private Material AllowedMat;
        [SerializeField] private Material UnallowedMat;

        private Renderer _render;

        private int _lineSegments = 50;
        private Vector2 _radius;
        private bool _drawCircle = false;
        private bool _allowed = false;

        public void InitRadius(Vector2 radius)
        {
            _radius = radius;
            _drawCircle = true;
        }

        public void UpdateMat(bool state)
        {
            if(state != _allowed)
            {
                _allowed = state;
                _renderers.ForEach(r => r.material = state ? AllowedMat : UnallowedMat);
            }
        }

        private void Update()
        {
            if (_drawCircle)
            {
                _createCircle(_lineRenderer, _lineSegments, _radius.x);
                _createCircle(_lineRendererMax, _lineSegments, _radius.y);
            }
        }

        private void _createCircle(LineRenderer renderer, int steps, float radius)
        {
            renderer.positionCount = steps;

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

                renderer.SetPosition(currentStep, currentPosition);
            }
        }
    }
}
