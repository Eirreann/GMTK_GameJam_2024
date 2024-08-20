using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GMTK_Jam.UI
{
    public class UIIntroScreen : MonoBehaviour
    {
        public List<Slide> Slides;

        [Header("Components")]
        public Image Image;
        public TextMeshProUGUI Text;
        public Button NextBtn;
        public TextMeshProUGUI ButtonTxt;

        private UnityAction _callback;
        private int _index = 0;

        private void Start()
        {
            NextBtn.onClick.AddListener(_nextSlide);
        }

        public void OnStart(UnityAction callback)
        {
            _callback = callback;
            _updateDisplay();
        }

        private void _updateDisplay()
        {
            Image.sprite = Slides[_index].Image;
            Text.text = Slides[_index].Text;
        }

        private void _nextSlide()
        {
            if (_index == Slides.Count - 1)
            {
                _callback.Invoke();
                gameObject.SetActive(false);
            }
            else
            {
                _index++;
                _updateDisplay();
                if (_index == Slides.Count - 1)
                    ButtonTxt.text = "Start";
            }
        }
    }

    [System.Serializable]
    public struct Slide
    {
        public Sprite Image;
        [TextArea(2, 10)]
        public string Text;
    }
}
