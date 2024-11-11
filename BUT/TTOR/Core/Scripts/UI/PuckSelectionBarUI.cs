using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BUT.TTOR.Core
{
    public class PuckSelectionBarUI : MonoBehaviour
    {
        [SerializeField] private GameObject puckTogglePrefab;
        [SerializeField] private bool _setupAsRadioButtons = false;

        private ToggleGroup _toggleGroup;
        private List<Toggle> _toggles = new List<Toggle>();

        public UnityEvent<string, bool> OnPuckToggleValueChanged;

        private void Awake()
        {
            CreatePuckToggleButtons();
        }

        private void CreatePuckToggleButtons()
        {
            PuckData[] puckData = PuckDataManager.GetPuckData();

            ToggleGroup toggleGroup = GetComponent<ToggleGroup>();
            if (_setupAsRadioButtons)
            {
                if (toggleGroup == null) { toggleGroup = gameObject.AddComponent<ToggleGroup>(); }
            }
            else if (toggleGroup != null)
            {
                toggleGroup.enabled = false;
            }

            for (int i = 0; i < puckData.Length; i++)
            {
                PuckToggle newPuckToggle = Instantiate(puckTogglePrefab, transform).GetComponent<PuckToggle>();
                newPuckToggle.Initialize(puckData[i]);

                Toggle toggle = newPuckToggle.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(x => { OnToggleValueChanged(toggle.GetComponent<PuckToggle>(), x); });

                if (_setupAsRadioButtons) { toggle.group = toggleGroup; }

                _toggles.Add(toggle);
            }


            DeselectAll();
        }

        private void OnToggleValueChanged(PuckToggle toggle, bool isOn)
        {
            OnPuckToggleValueChanged?.Invoke(toggle.PuckData.name, isOn);
        }

        public void DeselectAll()
        {
            if (_toggleGroup && _toggleGroup.enabled) { _toggleGroup.SetAllTogglesOff(); }
            else
            {
                for (int i = 0; i < _toggles.Count; i++)
                {
                    _toggles[i].isOn = false;
                }
            }
        }
    }
}
