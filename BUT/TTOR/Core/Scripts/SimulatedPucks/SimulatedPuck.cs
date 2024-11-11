using BUT.TTOR.Core.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BUT.TTOR.Core
{
    public class SimulatedPuck : MonoBehaviour
    {
        [Header("Settings")]
        public float OverrideDiameterInCentimeters = 0;

        [Header("References")]
        public PuckData Data;
        public RectTransform[] TouchPoints;

        [Header("Visuals")]
        public RectTransform VisualsRectTransform;
        public Image HoverImage;
        public TextMeshProUGUI PuckIdText;
        public Color HoverColor;
        public Color DraggingColor;

        private RectTransform _rectTransform;
        private DragEvents _dragEvents;
        private Canvas _canvas;

        private bool _isDragging = false;
        private bool _isTouching = false;
        private bool _isTwisting = false;
        private PointerEventData _grabbingPointerEventData;
        private TTOR_PuckTracker _pucktracker;
        private SerializablePuckData _originalPuckData;

        private bool _puckWasCalibrated = false;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _dragEvents = transform.GetComponentInChildren<DragEvents>(true);
            _canvas = transform.root.GetComponentInChildren<Canvas>();

            _dragEvents.OnPointerEnterEvent.AddListener(OnPointerEnter);
            _dragEvents.OnPointerExitEvent.AddListener(OnPointerExit);
            _dragEvents.OnPointerDownEvent.AddListener(OnPointerDown);
            _dragEvents.OnPointerUpEvent.AddListener(OnPointerUp);
            _dragEvents.OnDragEvent.AddListener(OnDrag);
            _dragEvents.OnScrollEvent.AddListener(OnScroll);
            _dragEvents.OnTwistEvent.AddListener(OnTwist);
            _dragEvents.OnDoublePointerDownEvent.AddListener(OnDoublePointerDown);
            _dragEvents.OnDoublePointerUpEvent.AddListener(OnDoublePointerUp);

            HoverImage.gameObject.SetActive(false);

            _originalPuckData = new SerializablePuckData(Data);
            PositionTouchPoints();
        }

        private void PositionTouchPoints()
        {
            TTOR_PuckTracker _pucktracker = FindObjectOfType<TTOR_PuckTracker>();
            if (!_pucktracker)
            {
                TTOR_Logger.LogWarning("SimulatedPuck could not find a PuckTracker in the scene.");
                return;
            }

            // 3D printed pucks are only good if they're between 25 and 135 degrees key angle.
            // we want our simulated pucks to be similar as real ones.
            float deltaAngle = (135f - 25f) / _pucktracker.MaxSimultaniousPucks;

            Data.KeyAngle = 25f + (deltaAngle * transform.GetSiblingIndex());
            //Debug.Log("Calculated debug keyAngle to: " + Data.KeyAngle + " : " + transform.GetSiblingIndex());

            // 60 degrees is a bad angle, stay away from it, we don't know which way is forward otherwise.
            if (Mathf.Abs(60f - Data.KeyAngle) < (deltaAngle / 4f))
            {
                if (Data.KeyAngle < 60f)
                {
                    Data.KeyAngle -= (deltaAngle / 2f);
                }
                else
                {
                    Data.KeyAngle += (deltaAngle / 2f);
                }
                //Debug.Log("Data.KeyAngle is to close to 60, changing to: " + Data.KeyAngle);
            }

            Data.SurfaceArea = 10 / TTOR_PuckTracker.ScreenDPI * TTOR_PuckTracker.ScreenDPI;
            Data.CenterOffset = Vector2.zero;
            Data.ForwardDirectionOffset = 0;

            PuckIdText.text = Data.name;

            // create 3 debugTouches and position them so they resemble the puck
            float pixelArea = (Data.SurfaceArea * 0.155f) * TTOR_PuckTracker.ScreenDPI * TTOR_PuckTracker.ScreenDPI; //convert to pixels (1 square cm is 0.155f sqauare inch)

            float apexAngleRad = Data.KeyAngle * Mathf.PI / 180f;

            // Calculate base
            //float baseWidth = Mathf.Sqrt((4 * pixelArea) / Mathf.Tan(apexAngleRad/2f));
            float baseWidth = 2f * Mathf.Sqrt(pixelArea * Mathf.Tan(apexAngleRad / 2f));

            // Calculate height
            float height = (2 * pixelArea) / baseWidth;

            TouchPoints[0].anchoredPosition = new Vector2(0, height / 2f);
            TouchPoints[1].anchoredPosition = new Vector2(-baseWidth / 2f, -height / 2f);
            TouchPoints[2].anchoredPosition = new Vector2(baseWidth / 2f, -height / 2f);

            // calc offset
            Vector2 offset = (TouchPoints[0].anchoredPosition + TouchPoints[1].anchoredPosition + TouchPoints[2].anchoredPosition) / 3f;

            TouchPoints[0].anchoredPosition -= offset;
            TouchPoints[1].anchoredPosition -= offset;
            TouchPoints[2].anchoredPosition -= offset;
        }


        private void OnDestroy()
        {
            if (!_dragEvents) { return; }
            _dragEvents.OnPointerEnterEvent.RemoveListener(OnPointerEnter);
            _dragEvents.OnPointerExitEvent.RemoveListener(OnPointerExit);
            _dragEvents.OnPointerDownEvent.RemoveListener(OnPointerDown);
            _dragEvents.OnPointerUpEvent.RemoveListener(OnPointerUp);
            _dragEvents.OnDragEvent.RemoveListener(OnDrag);
            _dragEvents.OnScrollEvent.RemoveListener(OnScroll);
            _dragEvents.OnTwistEvent.RemoveListener(OnTwist);
            _dragEvents.OnDoublePointerDownEvent.RemoveListener(OnDoublePointerDown);
            _dragEvents.OnDoublePointerUpEvent.RemoveListener(OnDoublePointerUp);
        }

        private void OnEnable()
        {
            _puckWasCalibrated = Data.IsCalibrated;
            _rectTransform.anchoredPosition = Vector2.zero;
            PositionTouchPoints();
        }

        private void OnDisable()
        {
            ResetPuckData();
        }

        private void ResetPuckData()
        {
            Data.SetData(_originalPuckData, _puckWasCalibrated);
        }

        public void SizeImage(Vector2 targetDisplaResolution, float targetDisplayDiagonalSizeInInch, float printedPuckDiameterInCentimeters, float canvasReferencePixelsPerUnit)
        {
            if(OverrideDiameterInCentimeters > 0)
            {
                printedPuckDiameterInCentimeters = OverrideDiameterInCentimeters;
            }
            float printedPuckDiameterInInches = printedPuckDiameterInCentimeters * 0.393701f;


            float pixelSize = printedPuckDiameterInInches * TTOR_PuckTracker.ScreenDPI;

#if UNITY_EDITOR
            int DPI = Mathf.RoundToInt(Mathf.Sqrt(Mathf.Pow(targetDisplaResolution.x, 2) + Mathf.Pow(targetDisplaResolution.y, 2)) / targetDisplayDiagonalSizeInInch);
            pixelSize = printedPuckDiameterInInches * DPI;
#endif

            // Size visuals
            VisualsRectTransform.sizeDelta = new Vector2(pixelSize, pixelSize);
        }

        #region MOVEMENT
        private void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isDragging) 
            { 
                if (_isTouching) 
                { 
                    _isTouching = false;
                    return;
                }
                HoverImage.color = HoverColor; 
                HoverImage.gameObject.SetActive(true);
            }
        }
        private void OnPointerExit(PointerEventData eventData)
        {
            if (!_isDragging)
            {
                HoverImage.gameObject.SetActive(false);
            }
        }
        private void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Left)
            {
                if(_grabbingPointerEventData != null) { return; }

                _grabbingPointerEventData = eventData;

                _isDragging = true;

                if (eventData.button == PointerEventData.InputButton.Left && Input.touchCount > 0) { _isTouching = true; }

                HoverImage.gameObject.SetActive(true);
                HoverImage.color = DraggingColor;

                if (_isTwisting) { return; }

                Vector2 mousePos = eventData.position;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform.parent as RectTransform, mousePos, eventData.pressEventCamera, out Vector2 localPoint);
                if(Input.touchCount == 0) { _rectTransform.localPosition = localPoint; }
            }
        }
        private void OnPointerUp(PointerEventData eventData)
        {
            if (_grabbingPointerEventData != null && (eventData.pointerId == _grabbingPointerEventData.pointerId || (eventData.button == PointerEventData.InputButton.Left && _isTouching)))
            {
                _isDragging = false;
                _grabbingPointerEventData = null;

                HoverImage.color = HoverColor;
            }
        }
        private void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging || _isTwisting) { return; }
            
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
        private void OnScroll(PointerEventData eventData)
        {
            if (!_isDragging) { return; }

            float scrollDelta = eventData.scrollDelta.y;
            float rotationSpeed = 5f;

            _rectTransform.Rotate(Vector3.forward, scrollDelta * rotationSpeed);
        }
        private void OnDoublePointerUp()
        {
            _isTwisting = false;
        }

        private void OnDoublePointerDown()
        {
            _isTwisting = true;
        }

        private void OnTwist(float angleDelta, List<PointerTouchposition> pointerTouchpositions)
        {
            float twistRotationSpeed = 1.5f;
            _rectTransform.Rotate(Vector3.forward, -angleDelta * twistRotationSpeed);

            if(Input.touchCount >= 2)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform.parent as RectTransform, pointerTouchpositions[0].Position, pointerTouchpositions[0].PointerEventData.pressEventCamera, out Vector2 localPoint1);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform.parent as RectTransform, pointerTouchpositions[1].Position, pointerTouchpositions[1].PointerEventData.pressEventCamera, out Vector2 localPoint2);
                _rectTransform.anchoredPosition = (localPoint1 + localPoint2) / 2;
            }
        }
        #endregion
    }
}

