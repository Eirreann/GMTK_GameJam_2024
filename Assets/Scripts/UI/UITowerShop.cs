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
            TowerBaseDamage.text = data.Prefab != null ? data.Prefab.baseDamage.ToString() : "0";
            TowerBaseFireRate.text = data.Prefab != null ? data.Prefab.baseAttackSpeed.ToString() : "0";
        }

        private void _onClick(BuildingData data)
        {
            _callback.Invoke(data);
            _onClose();
        }

        private void _onClose()
        {
            GameManager.Instance.PauseGame(false);
            gameObject.SetActive(false);
        }
    }
}
