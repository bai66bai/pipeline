using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BUT.TTOR.Core
{
    public class CalLocUI : MonoBehaviour
    {
        [SerializeField] private Image ringImage;
        [SerializeField] private TextMeshProUGUI angleText;
        [SerializeField] private TextMeshProUGUI surfaceText;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private Image progressImage;

        public Color CalibrationDoneColor;

        private bool _isCalibrated = false;
        public bool IsCalibrated => _isCalibrated;

        private void Start()
        {
            ResetCalibration();
            instructionText.gameObject.SetActive(false);
        }

        public void UpdateCalibration(float angle, float surface)
        {
            ringImage.color = CalibrationDoneColor;
            angleText.text = angle.ToString("F2") + "°";
            surfaceText.text = surface.ToString("F2") + "cm²";

            _isCalibrated = true;
        }

        public void SetProgress(float progress)
        {
            progressImage.fillAmount = progress;
            if (progress >= 1)
            {
                progressImage.gameObject.SetActive(false);
            }
        }

        public void ShowInstruction()
        {
            instructionText.gameObject.SetActive(true);
        }

        public void HideInstruction()
        {
            instructionText.gameObject.SetActive(false);
        }

        public void ResetCalibration()
        {
            ringImage.color = Color.white;
            angleText.text = "";
            surfaceText.text = "";
            instructionText.text = "Place puck here";
            SetProgress(0);
            progressImage.gameObject.SetActive(true);

            _isCalibrated = false;
        }


    }
}
