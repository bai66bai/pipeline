using BUT.TTOR.Core.Utils;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace BUT.TTOR.Core
{
    public class SimulatedPuckController : MonoBehaviour
    {
        private SimulatedPuck[] _debugPucks;
        private CanvasScaler _canvasScaler;
        private PuckSelectionBarUI _puckSelectionBarUI;

        [Header("Display settings")]
        public Vector2Int TargetDisplayResolution = new Vector2Int(1920, 1080);
        public float TargetDisplayDiagonalSizeInInch = 43;
        public float PrintedPuckDiameterInCentimeters = 12;

        private void Awake()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
            _debugPucks = transform.GetComponentsInChildren<SimulatedPuck>(true);
            _puckSelectionBarUI = transform.GetComponentInChildren<PuckSelectionBarUI>(true);

            SetUniqueFingerIdsForDebugTouches();

            SizePuckVisuals();

            if (_puckSelectionBarUI != null){ _puckSelectionBarUI.OnPuckToggleValueChanged.AddListener(OnPuckToggleValueChanged); }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _debugPucks.Length; i++)
            {
                _puckSelectionBarUI.DeselectAll();
                _debugPucks[i].gameObject.SetActive(false);
            }
        }

        private void OnPuckToggleValueChanged(string puckDataID, bool isOn)
        {
            GetDebugPuckByName(puckDataID).gameObject.SetActive(isOn);
        }


        public SimulatedPuck GetDebugPuckByName(string name)
        {
            SimulatedPuck sp = _debugPucks.ToList().Find(sp => sp.Data.name == name);
            if (sp == null)
            {
                TTOR_Logger.LogWarning("<color=orange><b>Could not GetPuckDataByName: </b></color> No ScriptableObject found with name: " + name);
                return null;
            }

            return sp;
        }

        private void SetUniqueFingerIdsForDebugTouches()
        {
            SimulatedTouch[] debugTouches = transform.GetComponentsInChildren<SimulatedTouch>(true);
            for (int i = 0; i < debugTouches.Length; i++)
            {
                debugTouches[i].SetFingerId(1000 + i);
            }
        }

        private void SizePuckVisuals()
        {
            for (int i = 0; i < _debugPucks.Length; i++)
            {
                _debugPucks[i].SizeImage(TargetDisplayResolution, TargetDisplayDiagonalSizeInInch, PrintedPuckDiameterInCentimeters, _canvasScaler.referencePixelsPerUnit);
            }
        }
    }
}
