using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK_Jam.UI
{
    public class UIScaleBar : MonoBehaviour
    {
        public Image BarImage;
        public Image PreviewImage;
        public TextMeshProUGUI BarValueText;
        public TextMeshProUGUI UpdateAmntText;

        private string _valueText;
        private Coroutine _upgradeCoroutine;

        private void Awake()
        {
            _valueText = BarValueText.text;
            UpdateAmntText.gameObject.SetActive(false);
        }

        public void UpdateScaleUI(int mod, int current, int max)
        {
            BarImage.fillAmount = (float)current / (float)max;
            BarValueText.text = _valueText.Replace("#", $"{current} / {max}");

            if(mod != 0)
            {
                if (_upgradeCoroutine != null)
                    StopCoroutine(_upgradeCoroutine);
                _upgradeCoroutine = StartCoroutine(_showUpgradeAmount(mod));
            }
        }

        public void PreviewCost(int cost, int max)
        {
            PreviewImage.gameObject.SetActive(true);
            PreviewImage.fillAmount = (float)cost / (float)max;
        }

        public void PreviewCost(bool state)
        {
            PreviewImage.gameObject.SetActive(state);
        }

        private IEnumerator _showUpgradeAmount(int amnt)
        {
            UpdateAmntText.gameObject.SetActive(true);
            UpdateAmntText.text = amnt > 0 ? "+" + amnt.ToString() : amnt.ToString();
            UpdateAmntText.color = amnt > 0 ? Color.green : Color.red;

            yield return new WaitForSeconds(1f);

            UpdateAmntText.gameObject.SetActive(false);
        }
    }
}
