using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace BUT.TTOR.Core
{
    public class CalAngleUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI calibratedAngleText;
        [SerializeField] TextMeshProUGUI calibratedSurfaceText;
        public Button resetButton;
        public Button acceptButton;
        public Button deleteButton;

        private string _puckId = "";

        private void Start()
        {
            ResetCalibration();
        }

        public void ShowDeleteButton(string id)
        {
            _puckId = id;

            if (id != "")
            {
                deleteButton.gameObject.SetActive(PuckDataManager.GetPuckDataByName(_puckId).IsCalibrated);
            }
            else
            {
                deleteButton.gameObject.SetActive(false);
            }
        }

        public void UpdateCalibration(string puckId, float calibratedAngle, float calibratedSurface)
        {
            Debug.Log("UpdateCalibration: " + puckId);

            _puckId = puckId;
 
            calibratedAngleText.text = calibratedAngle.ToString("F2") + "°";
            calibratedSurfaceText.text = calibratedSurface.ToString("F2") + " cm²";

            deleteButton.gameObject.SetActive(false);
            resetButton.gameObject.SetActive(true);
            acceptButton.gameObject.SetActive(true);
        }

        public void ResetCalibration()
        {
            Debug.Log("ResetCalibration");

            calibratedAngleText.text = "";
            calibratedSurfaceText.text = "";
        
            deleteButton.gameObject.SetActive(false);

            if (_puckId != "")
            {
                if (PuckDataManager.GetPuckDataByName(_puckId).IsCalibrated)
                {
                    UpdateCalibration(_puckId, PuckDataManager.GetPuckDataByName(_puckId).KeyAngle, PuckDataManager.GetPuckDataByName(_puckId).SurfaceArea);
                    ShowDeleteButton(_puckId);
                }
            }

            resetButton.gameObject.SetActive(false);
            acceptButton.gameObject.SetActive(false);

            GetComponentInParent<CalUI>().calLocUIs[0].ShowInstruction();
        }
    }
}
