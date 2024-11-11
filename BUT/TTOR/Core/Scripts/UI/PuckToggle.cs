using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace BUT.TTOR.Core
{
    public class PuckToggle : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _idText;
        [SerializeField] private Image _checkMark;

        public bool ShowCalibrationStatus = false;

        private PuckData _puckData;

        public PuckData PuckData { get => _puckData; }

        public void Initialize(PuckData puckData)
        {
            _puckData = puckData;
            _idText.text = puckData.name;

            gameObject.name = "Toggle - " + puckData.name;
                
            _checkMark.enabled = ShowCalibrationStatus && puckData.IsCalibrated;

            puckData.OnCalibrationStatusChangedEvent.AddListener(OnCalibrationStatusChanged);
        }

        private void OnCalibrationStatusChanged()
        {
            _checkMark.enabled = ShowCalibrationStatus && _puckData.IsCalibrated;
        }
    }
}
