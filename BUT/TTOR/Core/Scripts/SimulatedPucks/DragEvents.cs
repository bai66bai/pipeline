using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BUT.TTOR.Core
{
    public class DragEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler
    {
        public UnityEvent<PointerEventData> OnDragEvent;
        public UnityEvent<PointerEventData> OnPointerDownEvent;
        public UnityEvent OnDoublePointerDownEvent;
        public UnityEvent<PointerEventData> OnPointerUpEvent;
        public UnityEvent OnDoublePointerUpEvent;
        public UnityEvent<PointerEventData> OnPointerEnterEvent;
        public UnityEvent<PointerEventData> OnPointerExitEvent;
        public UnityEvent<PointerEventData> OnScrollEvent;
        public UnityEvent<float, List<PointerTouchposition>> OnTwistEvent;

        public List<PointerTouchposition> TouchPositions = new List<PointerTouchposition>();

        //private bool _isTouching = false;
        private float _oldTwistAngle = 0;

        public void OnDrag(PointerEventData eventData)
        {
            OnDragEvent?.Invoke(eventData);

            PointerTouchposition pointerTouchposition = TouchPositions.Find(p => p.PointerEventData.pointerId == eventData.pointerId);
            if (pointerTouchposition != null)
            {
                pointerTouchposition.Position = eventData.position;
            }

            if (TouchPositions.Count < 2) { return; }

            Vector2 dir = TouchPositions[0].Position - TouchPositions[1].Position;
            float newAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float deltaAngle = Mathf.DeltaAngle(newAngle, _oldTwistAngle);
            _oldTwistAngle = newAngle;

            OnTwistEvent?.Invoke(deltaAngle, TouchPositions);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (TouchPositions.Count < 2)
            {
                TouchPositions.Add(new PointerTouchposition(eventData, eventData.position));
                if (TouchPositions.Count == 2) 
                {
                    Vector2 dir = TouchPositions[0].Position - TouchPositions[1].Position;
                    _oldTwistAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                    OnDoublePointerDownEvent?.Invoke(); 
                }
            }

            OnPointerDownEvent?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnPointerUpEvent?.Invoke(eventData);

            PointerTouchposition pointerTouchposition = TouchPositions.Find(p => p.PointerEventData.pointerId == eventData.pointerId);
            if (pointerTouchposition != null)
            {
                if (TouchPositions.Count == 2) { OnDoublePointerUpEvent?.Invoke(); }
                TouchPositions.Remove(pointerTouchposition);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterEvent?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitEvent?.Invoke(eventData);
        }

        public void OnScroll(PointerEventData eventData)
        {
            OnScrollEvent?.Invoke(eventData);
        }
    }

    public class PointerTouchposition
    {
        public PointerEventData PointerEventData;
        public Vector2 Position;

        public PointerTouchposition(PointerEventData pointerEventData, Vector2 position)
        {
            PointerEventData = pointerEventData;
            Position = position;
        }
    }
}
