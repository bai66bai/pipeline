using UnityEngine;
using UnityEngine.Events;
using BUT.TTOR.Core.Utils;
using System;

namespace BUT.TTOR.Core
{
    [CreateAssetMenu(fileName = "Puck", menuName = "BUT/TTOR/PuckData")]
    public class PuckData : ScriptableObject
    {
        public float KeyAngle;
        public float SurfaceArea;
        public Vector2S CenterOffset;
        public float ForwardDirectionOffset;

        //public bool IsCalibrated = false;
        private bool _isCalibrated = false;

        public UnityEvent OnCalibrationStatusChangedEvent = new UnityEvent();

        public bool IsCalibrated { 
            get => _isCalibrated;
            set {
                //Debug.Log("set isCalib= " + value + " : " + ID);
                if (_isCalibrated != value)
                {
                    _isCalibrated = value;
                    OnCalibrationStatusChangedEvent.Invoke();
                }
            }
        }

        public void ResetValues()
        {
            KeyAngle = 0;
            SurfaceArea = 0;
            CenterOffset = Vector2.zero;
            ForwardDirectionOffset = 0;
            IsCalibrated = false;
        }

        public void SetData(SerializablePuckData serializablePuckData, bool isCalibrated = true)
        {
            KeyAngle = serializablePuckData.KeyAngle;
            SurfaceArea = serializablePuckData.Surface;
            CenterOffset = serializablePuckData.CenterOffset;
            ForwardDirectionOffset = serializablePuckData.ForwardDirectionOffset;

            IsCalibrated = isCalibrated;
        }

#if UNITY_EDITOR
        [ContextMenu("Save PuckData")]
        public void SavePuckData()
        {
            PuckDataManager.SaveData(this);
        }
#endif
    }

    [System.Serializable]
    public class SerializablePuckData
    {
        public float KeyAngle;
        public float Surface;
        public Vector2S CenterOffset;
        public float ForwardDirectionOffset;

        public SerializablePuckData(PuckData puckData)
        {
            KeyAngle = puckData.KeyAngle;
            Surface = puckData.SurfaceArea;
            CenterOffset = puckData.CenterOffset;
            ForwardDirectionOffset = puckData.ForwardDirectionOffset;
        }
    }
}
