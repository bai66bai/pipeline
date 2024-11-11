using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace BUT.TTOR.Core
{
    public class CalUI : MonoBehaviour
    {
        [SerializeField] private GameObject PuckAngleAndSurfaceAreaPanel;
        [SerializeField] private GameObject PuckCenterPanel;
        [SerializeField] private GameObject CalibrationSuccessPanel;
        [SerializeField] private CalPuckSelectionBarUI calPuckSelectionBarUI;
        [SerializeField] private CalAngleUI calAngleUI;
        public CalLocUI[] calLocUIs;
        [SerializeField] private TextMeshProUGUI puckSubtitleText;

        public UnityEvent<string> OnActivePuckChanged;
        public UnityEvent OnResetCalibration;
        public UnityEvent OnAcceptCalibrationAngleAndSurfaceArea;
        public UnityEvent OnAcceptCalibrationCenterOffsetAndForwardDirectionOffset;

        private string _activePuckId;

        private void Start()
        {
            PuckAngleAndSurfaceAreaPanel.SetActive(false);
            PuckCenterPanel.SetActive(false);
            CalibrationSuccessPanel.SetActive(false);

            for (int i = 0; i < calLocUIs.Length; i++)
            {
                CalLoc calloc = calLocUIs[i].gameObject.GetComponent<CalLoc>();
                int index = i;
                calloc.OnCalibratedEvent += () => {
                    CalUI_OnCalibratedEvent(index);
                };
            }
        }

        private void CalUI_OnCalibratedEvent(int locationID)
        {
            calLocUIs[locationID].HideInstruction();

            int nextLoc = locationID + 1;
            if (nextLoc < calLocUIs.Length)
            {
                calLocUIs[nextLoc].ShowInstruction();
            }
        }

        public void UpdateLocationCalibration(int locationID, float angle, float surface)
        {
            Debug.Log("UpdateLocationCalibration: " + locationID);

            if (locationID < 0 || locationID >= calLocUIs.Length)
            {
                Debug.LogWarning("WARNING: You are trying to update the calibration on a location that doesn't exist");
                return;
            }

            calLocUIs[locationID].UpdateCalibration(angle, surface);
            calLocUIs[locationID].HideInstruction();
        }

        public void SetLocationProgress(int locationID, float progress)
        {
            if (locationID < 0 || locationID >= calLocUIs.Length)
            {
                Debug.LogWarning("WARNING: You are trying to set the progress on a location that doesn't exist");
                return;
            }

            calLocUIs[locationID].SetProgress(progress);
        }

        public void UpdateOverallCalibration(float angle, float surface)
        {
            calAngleUI.UpdateCalibration(_activePuckId, angle, surface);
        }

        public void OnActivePuckToggleChanged(string activePuckID)
        {
            _activePuckId = activePuckID;

            puckSubtitleText.text = "Angle and surface area";
            PuckAngleAndSurfaceAreaPanel.SetActive(true);
            PuckCenterPanel.SetActive(false);
            CalibrationSuccessPanel.SetActive(false);

            calAngleUI.ResetCalibration();
            for (int i = 0; i < calLocUIs.Length; i++)
            {
                calLocUIs[i].ResetCalibration();
            }

            calLocUIs[0].ShowInstruction();

            SetEnableLocUIs(activePuckID != "");

            // TO DO: Set name of the puck
            //if(Config.PuckContents != null && (activePuckID-1 >= 0 && Config.PuckContents.Length - 1 >= activePuckID - 1 && activePuckID - 1 < Config.PuckContents.Length))
            //{
            //    puckSubtitleText.text = Config.PuckContents[activePuckID - 1].Title;
            //}
            OnActivePuckChanged?.Invoke(activePuckID);


            Debug.Log("activePuckID: " + activePuckID);

            if (activePuckID != "")
            {
                if (PuckDataManager.GetPuckDataByName(activePuckID).IsCalibrated)
                {
                    calAngleUI.ShowDeleteButton(activePuckID);
                    calAngleUI.ResetCalibration();
                    //calAngleUI.UpdateCalibration(PuckDataManager._puckData[activePuckID - 1].ID, PuckDataManager._puckData[activePuckID - 1].KeyAngle, PuckDataManager._puckData[activePuckID - 1].SurfaceArea);
                    //calAngleUI.acceptButton.gameObject.SetActive(false);
                }
            }
            else
            {
                calAngleUI.UpdateCalibration("", 0, 0);
                calAngleUI.ResetCalibration();
            }

        }

        private void SetEnableLocUIs(bool enable)
        {
            for (int i = 0; i < calLocUIs.Length; i++)
            {
                calLocUIs[i].gameObject.SetActive(enable);
            }
        }

        public void ResetCurrentCalibration()
        {
            calAngleUI.ResetCalibration();
        }

        public void OnPressedAcceptAngleAndSurfaceArea()
        {
            PuckAngleAndSurfaceAreaPanel.gameObject.SetActive(false);
            PuckCenterPanel.gameObject.SetActive(true);
            OnAcceptCalibrationAngleAndSurfaceArea?.Invoke();
        }

        public void OnPressedAcceptCenterOffsetAndForwardDirectionOffset()
        {
            PuckAngleAndSurfaceAreaPanel.gameObject.SetActive(false);
            PuckCenterPanel.gameObject.SetActive(false);
            OnAcceptCalibrationCenterOffsetAndForwardDirectionOffset?.Invoke();
        }

        public void OnPressedReset()
        {
            OnResetCalibration?.Invoke();
        }

        public void DeselectAll()
        {
            calPuckSelectionBarUI.DeselectAll();
        }

        public void ShowSuccessMessage()
        {
            PuckAngleAndSurfaceAreaPanel.SetActive(false);
            CalibrationSuccessPanel.SetActive(true);
        }
    }
}
