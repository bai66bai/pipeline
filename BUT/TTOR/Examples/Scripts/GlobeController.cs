using BUT.TTOR.Core;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace BUT.TTOR.Examples
{
    public class GlobeController : MonoBehaviour
    {
        [Header ("References")]
        public Transform FocusPoint;
        public Transform Globe;
        public List<PuckDataPin> ListPins = new List<PuckDataPin>();

        [Header ("Settings")]
        public float ZoomSpeed = 0.5f;
        public float ReturnSpeed = 0.01f;
        public float RotationSpeed = 10.0f;

        [Space(10)]
        public UnityEvent<PinInfo> OnActivePinChange;

        private float _zoomDistance = 0f;
        private float _zoomMin = -300;
        private float _zoomMax = 300;
        private Vector3 _zoomDirection;

        private float _rotationToPinDuration = 2f;

        private Quaternion _globeRotationBeforePuckDown;
        private Quaternion _targetRotation = Quaternion.identity;
        private Vector2 _pucksPreviousForwardDirection;
        private Vector3 _globeStartPosition;
        private Puck _activePuck;
        private GameObject _activePin;
        private List<Puck> _activePucks = new List<Puck>();
        
        private void Start()
        {
            _globeStartPosition = Globe.position;
        }

        public void OnPuckCreated(Puck puck)
        {
            if (_activePucks.Count > 0)
            {
                return;
            } else
            {
                _activePucks.Add(puck);
                _globeRotationBeforePuckDown = Globe.rotation;
                PuckDataPin puckDataPin = ListPins.Find(puckDataPin => (puckDataPin.PuckData == puck.Data));
                _activePuck = puck;
                _activePin = puckDataPin.Pin;

                _activePin.GetComponentInChildren<MeshRenderer>().enabled = true;
                _pucksPreviousForwardDirection = _activePuck.Triangle.ForwardVector;

                OnActivePinChange?.Invoke(_activePin.GetComponent<PinInfo>());

                SetTargetRotation(puckDataPin.Pin);
            }
        }

        public void OnPuckRemoved(Puck puck)
        {
            if (_activePucks.Remove(puck))
            { 
                _globeRotationBeforePuckDown.x = 0f;
                _globeRotationBeforePuckDown.z = 0f;
                _targetRotation = _globeRotationBeforePuckDown;

                _activePin.GetComponentInChildren<MeshRenderer>().enabled = false;

                _activePuck = null;
                _activePin = null;

                OnActivePinChange?.Invoke(null);
            }

        }

        // Gets the pins rotation
        private void SetTargetRotation(GameObject pin)
        {
            if (pin != null)
            {
                Vector3 directionToFocusPoint = FocusPoint.position - Globe.position;
                Vector3 directionToPin = pin.transform.up;

                _targetRotation = Quaternion.FromToRotation(directionToPin, directionToFocusPoint) * Globe.rotation;
            }
        }

        void Update()
        {
            // Zooming in or out based on active puck
            if (_activePuck != null)
            {
                float rotationDelta = Vector3.SignedAngle(_activePuck.Triangle.ForwardVector, _pucksPreviousForwardDirection, Vector3.forward);
                _pucksPreviousForwardDirection = _activePuck.Triangle.ForwardVector;

                _zoomDistance += rotationDelta * ZoomSpeed;

                _zoomDistance = Math.Clamp(_zoomDistance, _zoomMin, _zoomMax);

                _zoomDirection = _globeStartPosition - FocusPoint.position;
            }

            Globe.position = _globeStartPosition + (_zoomDirection.normalized * _zoomDistance);

            // Returning globe to original rotation if no puck or pin is detected
            if (_activePuck == null && _activePin == null)
            {
                _zoomDistance = Mathf.Lerp(_zoomDistance, 0, ReturnSpeed);

                Quaternion rotationSpeed = Quaternion.Euler(0f, - RotationSpeed * Time.deltaTime, 0f);
                _targetRotation = _targetRotation * rotationSpeed;
            }

            if (_targetRotation != null)
            {
                Globe.rotation = Quaternion.Slerp(Globe.rotation, _targetRotation, _rotationToPinDuration * Time.deltaTime);
            }
        }
    }

    [Serializable]
    public struct PuckDataPin
    {
        public PuckData PuckData;
        public GameObject Pin;
    }
}