using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeArea : MonoBehaviour
{
    // 区域的 RectTransform
    public RectTransform swipeArea;
    public CtrlDirection direction;
    // 滑动的最小距离来判定为有效滑动
    public float minSwipeDistance = 50f;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool isSwiping = false;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // 检查触摸是否在指定区域内
            if (IsTouchInSwipeArea(touch))
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        // 开始滑动
                        startTouchPosition = touch.position;
                        isSwiping = true;
                        break;

                    case TouchPhase.Ended:
                        // 结束滑动
                        if (isSwiping)
                        {
                            endTouchPosition = touch.position;
                            DetectSwipeDirection();
                            isSwiping = false;
                        }
                        break;
                }
            }
        }
    }

    // 检测滑动方向
    private void DetectSwipeDirection()
    {
        float swipeDistanceX = endTouchPosition.x - startTouchPosition.x;

        // 确保滑动的距离大于最小值
        if (Mathf.Abs(swipeDistanceX) >= minSwipeDistance)
        {
            if (swipeDistanceX > 0)
            {
                direction.ClickLeftBtn();
            }
            else
            {
                direction.ClickRightBtn();
            }
        }
    }

    // 判断触摸点是否在指定区域内
    private bool IsTouchInSwipeArea(Touch touch)
    {
        // 把触摸坐标转换为世界坐标再判断
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(swipeArea, touch.position, null, out localPoint);
        return swipeArea.rect.Contains(localPoint);
    }
}
