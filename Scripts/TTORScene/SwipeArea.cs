using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeArea : MonoBehaviour
{
    // ����� RectTransform
    public RectTransform swipeArea;
    public CtrlDirection direction;
    // ��������С�������ж�Ϊ��Ч����
    public float minSwipeDistance = 50f;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool isSwiping = false;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // ��鴥���Ƿ���ָ��������
            if (IsTouchInSwipeArea(touch))
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        // ��ʼ����
                        startTouchPosition = touch.position;
                        isSwiping = true;
                        break;

                    case TouchPhase.Ended:
                        // ��������
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

    // ��⻬������
    private void DetectSwipeDirection()
    {
        float swipeDistanceX = endTouchPosition.x - startTouchPosition.x;

        // ȷ�������ľ��������Сֵ
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

    // �жϴ������Ƿ���ָ��������
    private bool IsTouchInSwipeArea(Touch touch)
    {
        // �Ѵ�������ת��Ϊ�����������ж�
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(swipeArea, touch.position, null, out localPoint);
        return swipeArea.rect.Contains(localPoint);
    }
}
