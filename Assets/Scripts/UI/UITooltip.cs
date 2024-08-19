using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GMTK_Jam.UI
{
    public class UITooltip : MonoBehaviour
    {
        public TextMeshProUGUI TooltipText;
        public float DisplayTime = 10f;

        private CanvasGroup _canvas;
        private float _fadeSpeed = 1f;

        public void ShowTooltip(string text)
        {
            _canvas = GetComponent<CanvasGroup>();
            StartCoroutine(_show(text));
        }

        private IEnumerator _show(string text)
        {
            TooltipText.text = text;
            _canvas.alpha = 0;

            while(_canvas.alpha < 1)
            {
                _canvas.alpha += _fadeSpeed * Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(DisplayTime);

            while (_canvas.alpha > 0)
            {
                _canvas.alpha -= _fadeSpeed * Time.deltaTime;
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
