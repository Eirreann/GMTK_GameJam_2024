using GMTK_Jam.Buildings;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GMTK_Jam.UI
{
    public class UITowerShop : MonoBehaviour
    {
        public UITowerEntry TowerEntryTemplate;

        [Header("Components")]
        public Transform EntryParent;
        public GameObject DescArea;
        public TextMeshProUGUI TowerHeader;
        public TextMeshProUGUI TowerDesc;
        public TextMeshProUGUI TowerBaseDamage;
        public TextMeshProUGUI TowerBaseFireRate;
        public TextMeshProUGUI TowerCost;

        [Header("Buttons")]
        public Button CloseBtn;

        private List<UITowerEntry> _towerBtns = new List<UITowerEntry>();
        private UnityAction<BuildingData> _callback;

        private void Start()
        {
            CloseBtn.onClick.AddListener(_onClose);
        }

        public void ShowBuyMenu(List<BuildingData> buildings, UnityAction<BuildingData> callback)
        {
            DescArea.SetActive(false);
            _callback = callback;

            if(_towerBtns.Count > 0)
            {
                _towerBtns.ForEach(t => Destroy(t.gameObject));
                _towerBtns.Clear();
            }

            TowerEntryTemplate.gameObject.SetActive(true);
            buildings.ForEach(b =>
            {
                var entry = Instantiate(TowerEntryTemplate, EntryParent);
                entry.Init(b, _onHover, _onClick);
                _towerBtns.Add(entry);
            });
        }

        private void _onHover(BuildingData data, bool state)
        {
            DescArea.SetActive(state);
            TowerHeader.text = data.Name;
            TowerDesc.text = data.Desc;
            TowerBaseDamage.text = data.Prefab != null ? data.Prefab.damageRange.x.ToString() : "0";
            TowerBaseFireRate.text = data.Prefab != null ? data.Prefab.attackSpeedRange.x.ToString() : "0";
            TowerCost.text = data.Prefab != null ? data.Cost.ToString() : "0";
            TowerCost.color = GameManager.Instance.CanAffordUpgrade(data.Cost) ? Color.green : Color.red;
        }

        private void _onClick(BuildingData data)
        {
            _callback.Invoke(data);
            _onClose();
        }

        private void _onClose()
        {
            BuildingData emptyData = new();
            _callback.Invoke(emptyData);
            GameManager.Instance.PauseGame(false);
            gameObject.SetActive(false);
        }
    }
}
