using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BUT.TTOR.Core;
using BUT.TTOR.Core.Utils;

namespace BUT.TTOR.Calibration
{
    [RequireComponent(typeof(CalUI))]
    public class CalibrationManager : MonoBehaviour
    {
        [SerializeField] private RectTransform _centeringReference;
        private CalUI _ui;
        private CalLoc[] _calibrationLocations;
        private TTOR_PuckTracker _puckTracker;

        private PuckData[] _puckData;
        private PuckData _selectedPuckData;

        private float _averageAngle = 0;
        private float _averageSurfaceArea = 0;
        private Vector2 _centerOffset = Vector2.zero;
        private float _forwardDirectionOffset = 0;

        private CalibrationStage _calibrationStage = CalibrationStage.None;

        private enum CalibrationStage
        {
            None,
            AngleAndSurfaceArea,
            CenterOffsetAndForwardDirectionOffset
        }

        private void Start()
        {
            _ui = GetComponent<CalUI>();
            _calibrationLocations = GetComponentsInChildren<CalLoc>(true);
            _puckTracker = GetComponentInChildren<TTOR_PuckTracker>();
            _puckData = PuckDataManager.GetPuckData();

            _ui.OnActivePuckChanged.AddListener(OnSelectedPuckChanged);
            _ui.OnAcceptCalibrationAngleAndSurfaceArea.AddListener(AcceptAngleAndSurfaceCalibration);
            _ui.OnResetCalibration.AddListener(ResetCalibration);
            _ui.OnAcceptCalibrationCenterOffsetAndForwardDirectionOffset.AddListener(AcceptCenterOffsetAndForwardDirection);
            _puckTracker.OnPucksUpdatedEvent.AddListener(OnPucksUpdated);

            for (int i = 0; i < _calibrationLocations.Length; i++)
            {
                _calibrationLocations[i].OnCalibratedEvent += OnLocationCalibrated;
            }
        }

        private void OnDestroy()
        {
            _ui.OnActivePuckChanged.RemoveListener(OnSelectedPuckChanged);
            _ui.OnAcceptCalibrationAngleAndSurfaceArea.RemoveListener(AcceptAngleAndSurfaceCalibration);
            _ui.OnResetCalibration.RemoveListener(ResetCalibration);
            _puckTracker.OnPucksUpdatedEvent.RemoveListener(OnPucksUpdated);
            for (int i = 0; i < _calibrationLocations.Length; i++)
            {
                _calibrationLocations[i].OnCalibratedEvent -= OnLocationCalibrated;
            }
        }

        private void OnSelectedPuckChanged(string selectedPuckId)
        {
            List<PuckData> puckDataList = _puckData.ToList();
            _selectedPuckData = puckDataList.Find(data => data.name == selectedPuckId);
            ResetCalibration();

            _calibrationStage = CalibrationStage.AngleAndSurfaceArea;
        }

        private void OnLocationCalibrated()
        {
            int calibratedLocations = 0;
            for (int i = 0; i < _calibrationLocations.Length; i++)
            {
                if (_calibrationLocations[i].IsCalibrated)
                {
                    calibratedLocations++;
                }
            }

            if(calibratedLocations == _calibrationLocations.Length)
            {
                CalibrateAngleAndSurfaceArea();
            }
        }

        private void CalibrateAngleAndSurfaceArea()
        {
            _averageAngle = 0;
            _averageSurfaceArea = 0;

            for (int i = 0; i < _calibrationLocations.Length; i++)
            {
                if (_calibrationLocations[i].IsCalibrated)
                {
                    _averageAngle += _calibrationLocations[i].CalibratedAngle;
                    _averageSurfaceArea += _calibrationLocations[i].CalibratedSurface;
                }
            }
            _averageAngle /= _calibrationLocations.Length;
            _averageSurfaceArea /= _calibrationLocations.Length;

            _ui.UpdateOverallCalibration(_averageAngle, _averageSurfaceArea);
        }

        public void AcceptAngleAndSurfaceCalibration()
        {
            // Set and save data
            _selectedPuckData.KeyAngle = _averageAngle;
            _selectedPuckData.SurfaceArea = _averageSurfaceArea;
            PuckDataManager.SaveData(_selectedPuckData);

            _calibrationStage = CalibrationStage.CenterOffsetAndForwardDirectionOffset;
        }

        private void OnPucksUpdated(List<Puck> pucks)
        {
            if (_calibrationStage != CalibrationStage.CenterOffsetAndForwardDirectionOffset) { return; }

            _centerOffset = Vector2.zero;
            _forwardDirectionOffset = 0;

            Puck puckToCalibrate = null;
            for (int i = 0; i < pucks.Count; i++)
            {
                if(pucks[i].Data == _selectedPuckData)
                {
                    puckToCalibrate = pucks[i];
                    break;
                }
            }

            if (puckToCalibrate == null) { return; }

            Vector2 puckCenter = puckToCalibrate.GetScreenPosition();
            _centerOffset = puckCenter - (Vector2)_centeringReference.position;
            puckToCalibrate.Triangle.GetKeyAngle();
            _forwardDirectionOffset = Vector2.SignedAngle(puckToCalibrate.Triangle.ForwardVector, Vector2.up);
            //Debug.Log(_forwardDirectionOffset);
        }

        public void DeleteActivePuckData()
        {
            if (_selectedPuckData == null)
            {
                Debug.LogWarning("No puck selected!");
                return;
            }

            PuckDataManager.DeleteData(_selectedPuckData.name);
        }

        public void AcceptCenterOffsetAndForwardDirection()
        {
            if(_centerOffset == Vector2.zero || _forwardDirectionOffset == 0) { return; }

            _selectedPuckData.CenterOffset = _centerOffset;
            _selectedPuckData.ForwardDirectionOffset = _forwardDirectionOffset;
            PuckDataManager.SaveData(_selectedPuckData);

            _ui.DeselectAll();
            ResetCalibration();

            _ui.ShowSuccessMessage();
        }

        public void ResetCalibration()
        {
            _averageAngle = 0;
            _averageSurfaceArea = 0;
            _centerOffset = Vector2.zero;
            _forwardDirectionOffset = 0;

            for (int i = 0; i < _calibrationLocations.Length; i++)
            {
                _calibrationLocations[i].ResetCalibration();
            }
            _ui.ResetCurrentCalibration();

            _calibrationStage = CalibrationStage.None;
        }

        public void ReturnToPreviousScene()
        {
            TTOR_CalibrationSceneLoader.LoadPreviousScene();
        }
    }
}
