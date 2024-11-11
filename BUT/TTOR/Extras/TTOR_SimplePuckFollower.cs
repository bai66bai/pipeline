using UnityEngine;
using BUT.TTOR.Core.Utils;
using System.Collections.Generic;

namespace BUT.TTOR.Core
{
    public class TTOR_SimplePuckFollower : MonoBehaviour, TTOR_IPuckContent
    {
        public List<PuckData> PucksToFollow = new List<PuckData>();

        public bool FollowPosition = true;
        public bool FollowRotation = true;


        private TTOR_PuckTracker _tracker;

        private Transform _transform;
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private RectTransform _parentRectTransform;

        private Puck _assignedPuck;

        private void Awake()
        {
            _tracker = FindObjectOfType<TTOR_PuckTracker>();

            if (_tracker == null)
            {
                TTOR_Logger.LogError("Could not find a PuckTracker in the scene.", gameObject);
                return;
            }

            _transform = transform;

            // For objects on a canvas
            _rectTransform = GetComponent<RectTransform>();
            if (_rectTransform != null)
            {
                _canvas = GetComponentInParent<Canvas>();
                if (_canvas != null)
                {
                    _parentRectTransform = transform.parent.GetComponent<RectTransform>();
                }
            }
        }

        private void OnEnable()
        {
            if (_tracker != null) { _tracker.OnPucksUpdatedEvent.AddListener(OnPucksUpdated); }
        }

        private void OnDisable()
        {
            if (_tracker != null) { _tracker.OnPucksUpdatedEvent.RemoveListener(OnPucksUpdated); }
        }

        private void OnPucksUpdated(List<Puck> pucksList)
        {
            if (!_tracker || !gameObject.activeInHierarchy) { return; }

            // 找到被分配的Puck，追踪它的位置和角度
            if (_assignedPuck != null && PucksToFollow.Count == 0)
            {
                if (FollowPosition) { ApplyPosition(_assignedPuck); }
                if (FollowRotation) { ApplyRotation(_assignedPuck); }

                return;
            }

            // 没有分配需要跟随的Puck，则追踪所有Puck的位置和角度
            foreach (Puck puck in pucksList)
            {
                if (PucksToFollow.Count == 0 || PucksToFollow.Contains(puck.Data))
                {
                    if (FollowPosition) { ApplyPosition(puck); }
                    if (FollowRotation) { ApplyRotation(puck); }
                }

                return;
            }
        }

        private void ApplyRotation(Puck puck)
        {
            // _transform.rotation = puck.GetRotation();
        }

        private void ApplyPosition(Puck puck)
        {
            _transform.position = puck.GetPosition(_tracker.ProjectionDistance);

            // For gameobjects in a canvas
            if (_rectTransform != null)
            {
                Vector2 canvasPos = GetCanvasPosition();
                _rectTransform.anchoredPosition = canvasPos;
            }
        }

        private Vector2 GetCanvasPosition()
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

            screenPos = cam.WorldToScreenPoint(_transform.position);

            Vector2 canvasPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRectTransform, screenPos, _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam, out canvasPos);
            if (_parentRectTransform.gameObject != _canvas.gameObject)
            {
                _rectTransform.anchorMax = _parentRectTransform.anchorMax;
                _rectTransform.anchorMin = _parentRectTransform.anchorMin;
            }

            return canvasPos;
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
