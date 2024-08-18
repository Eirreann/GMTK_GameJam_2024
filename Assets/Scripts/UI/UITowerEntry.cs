using GMTK_Jam.Buildings;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GMTK_Jam.UI
{
    public class UITowerEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _towerImage;

        private BuildingData _data;
        private UnityAction<BuildingData, bool> _hoverCallback;
        private UnityAction<BuildingData> _clickCallback;

        public void Init(BuildingData data, UnityAction<BuildingData, bool> hoverCallback, UnityAction<BuildingData> clickCallback)
        {
            _data = data;
            _hoverCallback = hoverCallback;
            _clickCallback = clickCallback;

            _towerImage.sprite = _data.BaseIcon;
            Button button = GetComponent<Button>();
            button.interactable = _data.Prefab != null && GameManager.Instance.CanAffordUpgrade(_data.Cost);
            button.onClick.AddListener(()=> _clickCallback.Invoke(_data));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hoverCallback.Invoke(_data, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hoverCallback.Invoke(_data, false);
        }
    }
}
