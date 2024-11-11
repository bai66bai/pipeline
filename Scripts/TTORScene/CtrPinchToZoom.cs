using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CtrPinchToZoom : MonoBehaviour //, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform imageRectTransform;
    public RectTransform parentRectTransform;
    public float maxScale = 2f;
    public float minScale = 1f;

    private Vector2 initialPosition;
    private Vector3 initialScale;
    private bool isZoomed = false;
    private bool isDragging = false;
    private Vector2 lastTouchPosition;

    private void Start()
    {
        if (imageRectTransform == null || parentRectTransform == null)
        {
            Debug.LogError("Please assign the RectTransforms in the inspector.");
            return;
        }
        initialPosition = imageRectTransform.anchoredPosition;
        initialScale = imageRectTransform.localScale;
    }

    private void Update()
    {
        if (TouchCountInParent() >= 2)
        {
            if (Input.touchCount >= 2)
            {
                HandlePinchZoom();
            }
            else if (Input.touchCount == 1)
        {
            HandleDrag();
        }else if (Input.touchCount == 0 && isZoomed)
        {
            ResetImage();
        }
        
        }
        
    }

    private int TouchCountInParent()
    {
        int count = 0;
        foreach (Touch touch in Input.touches)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(parentRectTransform, touch.position))
            {
                count++;
            }
        }
        return count;
    }

    private Touch[] GetTouchesInParent()
    {
        // Get all touches within the parent RectTransform
        Touch[] touchesInParent = Array.FindAll(Input.touches, touch =>
            RectTransformUtility.RectangleContainsScreenPoint(parentRectTransform, touch.position));
        return touchesInParent;
    }

    private void HandlePinchZoom()
    {
        Touch[] touchesInParent = GetTouchesInParent();
        if (touchesInParent.Length >= 2)
        {
            Touch touch1 = touchesInParent[0];
            Touch touch2 = touchesInParent[1];

            Vector2 currentTouch1Pos = touch1.position;
            Vector2 currentTouch2Pos = touch2.position;

            float currentDistance = Vector2.Distance(currentTouch1Pos, currentTouch2Pos);
            float previousDistance = Vector2.Distance(
                (currentTouch1Pos - touch1.deltaPosition),
                (currentTouch2Pos - touch2.deltaPosition)
            );

            if (previousDistance != 0)
            {
                float scaleFactor = currentDistance / previousDistance;

                // Update scale and position
                Vector3 newScale = imageRectTransform.localScale * scaleFactor;
                if (newScale.x <= maxScale && newScale.x >= minScale)
                {
                    imageRectTransform.localScale = newScale;
                    ClampToParentBounds();
                    isZoomed = true;
                }
            }
        }
    }

    private void HandleDrag()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 currentTouchPosition = touch.position;
                Vector2 delta = currentTouchPosition - lastTouchPosition;
                lastTouchPosition = currentTouchPosition;

                // Move image considering scale
                Vector2 newAnchoredPosition = imageRectTransform.anchoredPosition + delta / imageRectTransform.localScale.x;
                imageRectTransform.anchoredPosition = newAnchoredPosition;
                ClampToParentBounds();
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
    }

    private void ResetImage()
    {
        imageRectTransform.localScale = initialScale;
        imageRectTransform.anchoredPosition = initialPosition;
        isZoomed = false;
    }

    private void ClampToParentBounds()
    {
        Vector3[] corners = new Vector3[4];
        imageRectTransform.GetWorldCorners(corners);

        RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRectTransform, corners[0], null, out Vector3 worldPoint1);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRectTransform, corners[2], null, out Vector3 worldPoint2);

        float width = worldPoint2.x - worldPoint1.x;
        float height = worldPoint2.y - worldPoint1.y;

        float imageWidth = imageRectTransform.rect.width * imageRectTransform.localScale.x;
        float imageHeight = imageRectTransform.rect.height * imageRectTransform.localScale.y;

        float offsetX = 0f;
        float offsetY = 0f;

        if (imageWidth > width)
        {
            offsetX = Mathf.Clamp(imageRectTransform.anchoredPosition.x, -imageWidth / 2f + width / 2f, imageWidth / 2f - width / 2f);
        }

        if (imageHeight > height)
        {
            offsetY = Mathf.Clamp(imageRectTransform.anchoredPosition.y, -imageHeight / 2f + height / 2f, imageHeight / 2f - height / 2f);
        }

        imageRectTransform.anchoredPosition = new Vector2(offsetX, offsetY);
    }
}
