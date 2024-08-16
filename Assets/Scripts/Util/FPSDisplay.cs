using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK_Jam.Util
{
    public class FPSDisplay : MonoBehaviour
    {
        public bool showDisplay = true;

        private float _deltaTime = 0.0f;
        private GUIStyle _style = GUIStyle.none;

        private float _sample = 0.0f;
        private float _fpsUpdateDelay = 0.0f;

        private void Awake()
        {
            _style = new GUIStyle();
            _style.alignment = TextAnchor.MiddleCenter;
            _style.fontSize = 24;// h * 2 / 100;
            _style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        private const float FPS_UPDATE_DELAY = 0.5f;
        void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;

            _fpsUpdateDelay -= Time.deltaTime;
            if (_fpsUpdateDelay <= 0.0f)
            {
                _sample = _deltaTime;
                _fpsUpdateDelay = FPS_UPDATE_DELAY;
            }
        }

        const float WIDTH = 250.0f;
        const float HEIGHT = 40.0f;
        const float BORDER = 10.0f;
        void OnGUI()
        {
            if (showDisplay)
            {
                Rect rect = new Rect(BORDER, Screen.height - BORDER - HEIGHT, WIDTH, HEIGHT);
                if (_sample > 0.0f)
                {
                    GUI.Box(rect, "");
                    float msec = _sample * 1000.0f;
                    float fps = 1.0f / _sample;
                    //string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
                    string text = ((int)fps).ToString() + " fps (" + msec.ToString("0.00") + " ms)";
                    GUI.Label(rect, text, _style);
                }
            }
        }
    }
}