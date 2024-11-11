using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BUT.TTOR.Core
{
    [RequireComponent(typeof(Collider2D), typeof(CalLocUI))]
    public class CalLoc : MonoBehaviour
    {
        private CalLocUI _ui;
        private Collider2D _collider;
        private List<int> _touchesInLocation = new List<int>();

        private bool _isCalibrating = false;
        private bool _isCalibrated = false;

        private float _calibrationProgress = 0;
        private float _calibratedAngle = 0;
        private float _calibratedSurface = 0;

        private const float CALIBRATION_TIME = 1f;
        private const int REQUIRED_TOUCHES = 3;

        public bool IsCalibrated => _isCalibrated;
        public float CalibratedAngle => _calibratedAngle;
        public float CalibratedSurface => _calibratedSurface;

        public delegate void OnCalibrated();
        public event OnCalibrated OnCalibratedEvent;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _ui = GetComponent<CalLocUI>();
        }

        public void ResetCalibration()
        {
            _isCalibrated = false;
            _calibrationProgress = 0;
            _calibratedAngle = 0;
            _calibratedSurface = 0;
            _touchesInLocation.Clear();
            _ui.ResetCalibration();
        }

        private void Update()
        {
            if (_isCalibrated) { return; }

            GetAllTouchesInLocation();

            if(_touchesInLocation.Count == REQUIRED_TOUCHES)
            {
                _isCalibrating = true;
            }
            else
            {
                _isCalibrating = false;
                _calibrationProgress = 0;
                _ui.SetProgress(_calibrationProgress);
            }

            if (_isCalibrating)
            {
                TickCalibrationTimer();
            }
        }

        private void GetAllTouchesInLocation()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (_collider == Physics2D.OverlapPoint(touch.position))
                {
                    if (!_touchesInLocation.Contains(touch.fingerId))
                    {
                        _touchesInLocation.Add(touch.fingerId);
                    }
                }
                else
                {
                    if (_touchesInLocation.Contains(touch.fingerId))
                    {
                        _touchesInLocation.Remove(touch.fingerId);
                    }
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    if (_touchesInLocation.Contains(touch.fingerId))
                    {
                        _touchesInLocation.Remove(touch.fingerId);
                    }
                }
            }
        }

        private void TickCalibrationTimer()
        {
            _calibrationProgress += Time.deltaTime / CALIBRATION_TIME;
            _ui.SetProgress(_calibrationProgress);

            if(_calibrationProgress >= 1)
            {
                Calibrate();
            }
        }

        private void Calibrate()
        {
            List<Touch> touchesList = Input.touches.ToList();
            Triangle triangle = new Triangle(touchesList.Find(t => t.fingerId == _touchesInLocation[0]), touchesList.Find(t => t.fingerId == _touchesInLocation[1]), touchesList.Find(t => t.fingerId == _touchesInLocation[2]));
            _calibratedSurface = triangle.GetSurfaceArea();
            _calibratedAngle = triangle.GetKeyAngle();

            _ui.UpdateCalibration(_calibratedAngle, _calibratedSurface);

            _isCalibrating = false;
            _isCalibrated = true;

            OnCalibratedEvent?.Invoke();
        }
    }
}
