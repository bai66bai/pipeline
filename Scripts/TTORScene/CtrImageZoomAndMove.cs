using UnityEngine;
using UnityEngine.EventSystems;

public class CtrImageZoomAndMove : MonoBehaviour //, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public RectTransform imageTransform; // 图片的RectTransform
    public RectTransform parentTransform; // 父级的RectTransform
    public float moveSmoothing = 0.1f; // 移动平滑参数

    private bool isDragging = false;
    private bool isZooming = false;

    private float initialDistance;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        if (imageTransform == null)
            imageTransform = GetComponent<RectTransform>();

        if (parentTransform == null)
            parentTransform = imageTransform.parent.GetComponent<RectTransform>();

        initialScale = imageTransform.localScale;
        initialPosition = imageTransform.localPosition;
        targetPosition = initialPosition;
    }

    private void Update()
    {
        if (Input.touchCount == 2)
        {
            // 双指缩放
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (IsTouchInsideParent(touch1.position) && IsTouchInsideParent(touch2.position))
            {
                if (!isZooming)
                {
                    isZooming = true;
                    initialDistance = Vector2.Distance(touch1.position, touch2.position);
                    initialScale = imageTransform.localScale; // 更新初始缩放比例
                }

                float currentDistance = Vector2.Distance(touch1.position, touch2.position);
                float scaleFactor = currentDistance / initialDistance;

                Vector3 newScale = initialScale * scaleFactor;
                newScale = ClampScale(newScale);

                imageTransform.localScale = newScale;
            }
        }
        else if (Input.touchCount == 1 && !isZooming)
        {
            // 单指拖动
            Touch touch = Input.GetTouch(0);

            if (IsTouchInsideParent(touch.position))
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    isDragging = true;
                    Vector2 delta = touch.deltaPosition;
                    Vector3 newPosition = imageTransform.localPosition + (Vector3)delta;

                    newPosition = ClampPosition(newPosition);
                    targetPosition = newPosition;
                }
            }
        }

        if (isDragging || isZooming)
        {
            // 平滑移动
            imageTransform.localPosition = Vector3.Lerp(imageTransform.localPosition, targetPosition, moveSmoothing);
        }

        if (Input.touchCount == 0)
        {
            // 复位操作
            isDragging = false;
            isZooming = false;

            if (imageTransform.localScale == initialScale)
            {
                imageTransform.localPosition = initialPosition;
                targetPosition = initialPosition;
            }
        }
    }

    private bool IsTouchInsideParent(Vector2 touchPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTransform, touchPosition, null, out Vector2 localPoint);
        return parentTransform.rect.Contains(localPoint);
    }

    private Vector3 ClampScale(Vector3 scale)
    {
        float minScale = 0.5f; // 设置最小缩放
        float maxScale = 3.0f; // 设置最大缩放
        scale.x = Mathf.Clamp(scale.x, minScale, maxScale);
        scale.y = Mathf.Clamp(scale.y, minScale, maxScale);
        return scale;
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        Vector3 minPosition = parentTransform.rect.min - imageTransform.rect.min * imageTransform.localScale.x;
        Vector3 maxPosition = parentTransform.rect.max - imageTransform.rect.max * imageTransform.localScale.x;

        position.x = Mathf.Clamp(position.x, minPosition.x, maxPosition.x);
        position.y = Mathf.Clamp(position.y, minPosition.y, maxPosition.y);
        return position;
    }
}
