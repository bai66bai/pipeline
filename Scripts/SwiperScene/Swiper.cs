using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class TouchSwiper : MonoBehaviour, IEndDragHandler
{
    public Transform ContentTransform;

    public float smoothTime = 0.2f;

    public GameObject CtrBtn;

    public int itemSize;

    public float threshold = 0.1f;

    public float offset = 0;

    public TCPClient client;

    private List<RunCirCular> Swipers;

    private float itemNum;

    private float currentItem = 0;

    private float swipeStep;

    [HideInInspector] public ScrollRect scrollRect;

    private float targetHorizontalPosition = 0f;
    private float velocity = 0f;
    private bool isRight = false;
    private bool isChange = false;
    [HideInInspector] public int Index { get; private set; }

    private void Awake()
    {
        Index = 0;
        Swipers = ContentTransform.gameObject.GetComponentsInChildren<RunCirCular>().ToList();
        scrollRect = GetComponent<ScrollRect>();
        RunCir(Index);
        itemNum = scrollRect.content.childCount / (float)itemSize;
        swipeStep = 1f / (itemNum - 1);
        if ((itemNum - 1) < 1)
        {
            swipeStep = 1f;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var nextPos = FindNearestPosition(eventData.position.x, eventData.pressPosition.x);
        if(targetHorizontalPosition != nextPos)
        {
            isChange = true;
            targetHorizontalPosition = nextPos;
            StartCoroutine(SmoothMove());
        }
    }


    private IEnumerator SmoothMove()
    {
        
        while (Mathf.Abs(scrollRect.normalizedPosition.x - targetHorizontalPosition) > threshold)
        {
            float newPosition = Mathf.SmoothDamp(scrollRect.normalizedPosition.x, targetHorizontalPosition, ref velocity, smoothTime);
            scrollRect.normalizedPosition = new Vector2(newPosition, scrollRect.normalizedPosition.y);
            yield return new WaitForEndOfFrame();
        }

        if (isChange)
        {
            Index = isRight ? Index + 1 : Index - 1;
            RunCir(Index);
        }
        
        scrollRect.normalizedPosition = new Vector2(targetHorizontalPosition, scrollRect.normalizedPosition.y);
        CtrBtn.GetComponent<Turnthepage>().ChangeBtnColor(scrollRect.normalizedPosition.x);
        isChange = false;
    }

    //控制跑环
    private void RunCir(int Index)
    {
        Swipers.ForEach(item =>
        {
            if (item.index == Index)
            {
                item.LoadColor();
            }
        });
    }

    private float FindNearestPosition(float currentPosition, float endPosition)
    {
        // 判断滑动方向
        if (endPosition > currentPosition)
        {  // 右滑
            isRight = true;
            client.SendMessage($"right:1");
            return FindNearestPositionByDirection(swipeStep);
        }
        else
        {  // 左滑
            isRight = false;
            client.SendMessage($"left:1");
            return FindNearestPositionByDirection(-swipeStep);
        }
        
    }

    private float FindNearestPositionByDirection(float step)
    {
        float targetItem = currentItem + step;

        if (!(targetItem > 1 || targetItem < 0))
        {
            currentItem = targetItem;
        }
        else if (targetItem > 1)
        {
            currentItem = 1f;
        }
        else if (targetItem < 0)
        {
            currentItem = 0f;
        }

        return currentItem;
    }

    public void SwipeToRight()
    {
        isRight = true;
        isChange = true;
        targetHorizontalPosition = FindNearestPositionByDirection(swipeStep);
        StartCoroutine(SmoothMove());
        client.SendMessage($"right:1");
    }

    public void SwipeToLeft()
    {
        isRight = false;
        isChange = true;
        targetHorizontalPosition = FindNearestPositionByDirection(-swipeStep);
        StartCoroutine(SmoothMove());
        client.SendMessage($"left:1");
    }

    //重置当前进度恢复到初始状态
    public void ChangeDisAble()
    {
        smoothTime = 0.2f;
        itemSize = 6;
        threshold = 0.07f;
        offset = 0;
        itemNum = 0;
        currentItem = 0;
        targetHorizontalPosition = 0f;
        velocity = 0f;
        isRight = false;
        isChange=false;
        Index = 0;
    }

    private void OnDisable()
    {
        scrollRect.normalizedPosition = new Vector2(0, 0);
        ChangeDisAble();
    }
}
