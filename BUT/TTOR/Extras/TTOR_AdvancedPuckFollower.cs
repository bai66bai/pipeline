using System.Collections.Generic;
using UnityEngine;

namespace BUT.TTOR.Core
{
    public class TTOR_AdvancedPuckFollower : MonoBehaviour, TTOR_IPuckContent
    {
        public List<PuckData> PucksToFollow = new List<PuckData>();

        public bool FollowPosition = true;
        public bool ResetPositionOnDisable = false;
        public FollowTypes PositionFollowType = FollowTypes.Direct;
        public float PositionSmoothFollowSpeed = 7.5f;
        public float ProjectionDistanceOffset = 0;

        public bool FollowRotation = true;
        public bool ResetRotationOnDisable = false;
        public FollowTypes RotationFollowType = FollowTypes.Direct;
        public float RotationSmoothFollowSpeed = 7.5f;
        [Space(20)]
        public bool PuckVelocityBlocksRotation = false;
        public float VelocityTreshold = 6;
        public float SecondsToHoldRotationBlock = 0.3f;
        [Tooltip("Uses the target pucks delta rotation to rotate this object instead of following the pucks absolute rotation.")]
        public bool KeepInitialRotation = false;
        [Space(10)]
        public bool LimitRotation = false;
        public float MinRotation = 0;
        public float MaxRotation = 270;



        private TTOR_PuckTracker _tracker;

        private Transform _transform;
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private RectTransform _parentRectTransform;

        private float _velocity;
        private Vector2 _previousPosition;
        private bool _rotationBlocked = false;
        private bool _isLiftingRotationBlock = false;

        private Vector3 _startPosition;

        private Quaternion _wantedRotation;
        private Vector3 _previousForward;
        [HideInInspector] public float totalRotation = 0;
        private Quaternion _previousRotation;

        private bool _isFirstFrameOnEnable = true;

        private Puck _assignedPuck;

        public enum FollowTypes
        {
            Direct,
            Smoothed
        }

        // Start is called before the first frame update
        void Awake()
        {
            _transform = transform;
            _wantedRotation = transform.rotation;
            _startPosition = transform.position;

            _tracker = GetComponentInParent<TTOR_PuckTracker>();
            if (_tracker == null)
            {
                _tracker = FindObjectOfType<TTOR_PuckTracker>();
            }

            if (_tracker == null)
            {
                Debug.LogError("Could not find a PuckTracker in the scene.");
                return;
            }
            else
            {
                _rectTransform = GetComponent<RectTransform>();
                if (_rectTransform)
                {
                    _canvas = GetComponentInParent<Canvas>();
                    if (_canvas != null)
                    {
                        _parentRectTransform = transform.parent.GetComponent<RectTransform>();
                    }
                }
            }

        }

        private void OnEnable()
        {
            _isFirstFrameOnEnable = true;

            if (_tracker != null)
            {
                _tracker.OnPucksUpdatedEvent.RemoveListener(OnPucksUpdated);
                _tracker.OnPucksUpdatedEvent.AddListener(OnPucksUpdated);
            }
        }

        private void OnDisable()
        {
            ResetFollower();

            if (_tracker != null)
            {
                _tracker.OnPucksUpdatedEvent.RemoveListener(OnPucksUpdated);
            }
        }

        private void OnPucksUpdated(List<Puck> pucks)
        {
            if (!_tracker || !gameObject.activeInHierarchy)
            {
                return;
            }

            // IF we don't specify certain puckData's to follow
            // AND we've gotten assigned a puck by a TTOR_PuckContentManager
            // THEN only follow the assignedPuck
            if (_assignedPuck != null && PucksToFollow.Count == 0)
            {
                FollowPuck(_assignedPuck);
                return;
            }

            foreach (Puck puck in pucks)
            {
                if (PucksToFollow.Count == 0 || PucksToFollow.Contains(puck.Data))
                {
                    FollowPuck(puck);
                }
            }
        }

        private void FollowPuck(Puck puck)
        {
            if (puck.Phase == PuckPhase.Lost) { return; }

            if (puck.Phase == PuckPhase.Removed) 
            { 
                ResetFollower();
                return;
            }

            if (KeepInitialRotation && (puck.Phase == PuckPhase.Staged || puck.Phase == PuckPhase.FoundAgain))
            {
                ResetVelocityVariablesAndDeltaRotation(puck);
            }

            if (PuckVelocityBlocksRotation)
            {
                Vector2 puckScreenPos = puck.GetScreenPosition();
                _velocity = (puckScreenPos - _previousPosition).magnitude;
                _previousPosition = puckScreenPos;
            }

            if (_isFirstFrameOnEnable)
            {
                _isFirstFrameOnEnable = false;
                ResetVelocityVariablesAndDeltaRotation(puck);
            }

            if (FollowPosition) { FollowPuckPosition(puck); }

            CheckRotationBlock();

            if (FollowRotation)
            {
                if (KeepInitialRotation)
                {
                    RotateWithDeltaRotation(puck);
                }
                else
                {
                    Rotate(puck);
                }
            }
        }

        private void CheckRotationBlock()
        {
            if (PuckVelocityBlocksRotation && _velocity > VelocityTreshold)
            {
                _rotationBlocked = true;
            }
            else if (PuckVelocityBlocksRotation && _velocity < VelocityTreshold)
            {
                if (_rotationBlocked && !_isLiftingRotationBlock)
                {
                    _isLiftingRotationBlock = true;
                    Invoke(nameof(LiftRotationBlock), SecondsToHoldRotationBlock);
                }
            }
        }

        private void ResetVelocityVariablesAndDeltaRotation(Puck p)
        {
            _previousPosition = p.GetScreenPosition();
            _previousRotation = p.GetRotation();
            _previousForward = p.Triangle.ForwardVector;
            LiftRotationBlock();
        }

        public void ResetFollower()
        {
            if (ResetPositionOnDisable)
            {
                _transform.position = _startPosition;
            }
            if (ResetRotationOnDisable) 
            {
                _wantedRotation = Quaternion.identity;
                _previousRotation = Quaternion.identity;
                if (_transform) _transform.rotation = _wantedRotation;
                totalRotation = 0;
            }
        }

        private void FollowPuckPosition(Puck p)
        {
            Vector3 targetPosition = p.GetPosition(_tracker.ProjectionDistance + ProjectionDistanceOffset);

            // World
            if(_rectTransform == null)
            {
                if (PositionFollowType == FollowTypes.Direct)
                {
                    _transform.position = targetPosition;
                }
                else if(PositionFollowType == FollowTypes.Smoothed)
                {
                    _transform.position = Vector3.Lerp(_transform.position, targetPosition, PositionSmoothFollowSpeed * Time.deltaTime);
                }
            }
            // UI
            else if (_rectTransform != null)
            {
                Vector2 screenPos;
                Camera cam;
                if (_canvas.worldCamera != null)
                {
                    cam = _canvas.worldCamera;
                }
                else
                {
                    cam = Camera.main;
                }

                screenPos = cam.WorldToScreenPoint(targetPosition);

                Vector2 targetCanvasPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, screenPos, _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam, out targetCanvasPos);
                if (_parentRectTransform.gameObject != _canvas.gameObject)
                {
                    _rectTransform.anchorMax = _parentRectTransform.anchorMax;
                    _rectTransform.anchorMin = _parentRectTransform.anchorMin;
                }

                if (PositionFollowType == FollowTypes.Direct)
                {
                    _rectTransform.anchoredPosition = targetCanvasPos;
                }
                else if (PositionFollowType == FollowTypes.Smoothed)
                {
                    _rectTransform.anchoredPosition = Vector2.Lerp(_rectTransform.anchoredPosition, targetCanvasPos, PositionSmoothFollowSpeed * Time.deltaTime);
                }
            }
        }

        private void Rotate(Puck p)
        {
            if (PuckVelocityBlocksRotation && _rotationBlocked) { return; }

            if (FollowRotation && RotationFollowType == FollowTypes.Smoothed)
            {
                _transform.rotation = p.GetLerpedRotation(RotationSmoothFollowSpeed * Time.deltaTime);
            }
            else if (FollowRotation)
            {
                _transform.rotation = p.GetRotation();
            }
        }

        private void RotateWithDeltaRotation(Puck p)
        {
            Quaternion qDelta = p.GetRotation() * Quaternion.Inverse(_previousRotation);
            _previousRotation = p.GetRotation();


            if (PuckVelocityBlocksRotation && _rotationBlocked)
            {
                _previousForward = p.Triangle.ForwardVector;
                return;
            }

            if (LimitRotation)
            {
                float signedAngle = Vector2.SignedAngle(p.Triangle.ForwardVector, _previousForward);
                _previousForward = p.Triangle.ForwardVector;
                float newTotalRotation = totalRotation + signedAngle;

                if (newTotalRotation < MinRotation)
                {
                    float newAngle = totalRotation - MinRotation;
                    totalRotation = MinRotation;
                    _wantedRotation = _wantedRotation * Quaternion.Euler(_tracker.TargetCamera.transform.forward * newAngle);
                }
                else if (newTotalRotation > MaxRotation)
                {
                    float newAngle = totalRotation - MaxRotation;
                    totalRotation = MaxRotation;
                    _wantedRotation = _wantedRotation * Quaternion.Euler(_tracker.TargetCamera.transform.forward * newAngle);
                }
                else
                {
                    totalRotation = newTotalRotation;
                    _wantedRotation = _wantedRotation * qDelta;
                }
            }
            else
            {
                _wantedRotation = _wantedRotation * qDelta;
            }

            if (RotationFollowType == FollowTypes.Smoothed)
            {
                _transform.rotation = Quaternion.LerpUnclamped(_transform.rotation, _wantedRotation, RotationSmoothFollowSpeed * Time.deltaTime);
            }
            else
            {
                _transform.rotation = _wantedRotation;
            }
        }

        private void LiftRotationBlock()
        {
            _rotationBlocked = false;
            _isLiftingRotationBlock = false;
        }

        public void AssignPuck(Puck puck)
        {
            _assignedPuck = puck;
        }

        public void RemovePuck(Puck puck)
        {
            _assignedPuck = null;
        }
    }
}