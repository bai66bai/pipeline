using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollMenu : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    public List<MenuItem> menuItems;

    public float transitionTime = 0.25f;

    public TCPClient client;

    private MenuItem currentItem;

    private ScrollRect scrollRect;

    private float nearestPos = 1f;

    private readonly List<float> itemCenterPosList = new()
    {
        1f, 0.89871f, 0.80064f, 0.70156f, 0.59845f, 0.50168f, 0.40257f, 0.30257f, 0.20096f, 0.10131f, 0.002f
    };


    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void activateCurrent()
    {
        client.SendMessage($"detail:{currentItem.Text}");
        currentItem.ActivateItem();
    }

    //private void Update()
    //{
    //    Debug.Log(scrollRect?.normalizedPosition.y);
    //}

    void Start()
    {
        // 检查静态变量ActiveDetailText来判断当前元素
        // 1.激活该元素
        currentItem = menuItems
            .Where(m => m.Text == DetailStore.ActiveDetailText)
            .ToList()
            .First();
        activateCurrent();

        // 2.滚动到该元素的位置
        int currentItemIndex = menuItems.IndexOf(currentItem);
        ScrollToIndex(currentItemIndex);
    }


    /// <summary>
    /// 开始拖拽回调
    /// </summary>
    /// <param name="eventData">事件参数</param>
    public void OnBeginDrag(PointerEventData eventData) { }


    /// <summary>
    /// 结束拖拽回调
    /// </summary>
    /// <param name="eventData">事件参数</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        UpdateNearestItemCenterPos();

        ActivateItemByPos(nearestPos);

        ScrollToPos(nearestPos);
    }


    /// <summary>
    /// 更新距离当前位置最近的元素中心位置
    /// </summary>
    private void UpdateNearestItemCenterPos()
    {
        foreach (var item in itemCenterPosList)
        {
            if (Math.Abs(item - scrollRect.normalizedPosition.y) < Math.Abs(nearestPos - scrollRect.normalizedPosition.y))
                nearestPos = item;
        }
    }


    /// <summary>
    /// 激活指定位置的元素
    /// </summary>
    /// <param name="pos">元素的位置</param>
    private void ActivateItemByPos(float pos)
    {
        currentItem.InactivateItem();

        int currentIndex = itemCenterPosList.IndexOf(pos);
        currentItem = menuItems[currentIndex];
        activateCurrent();


    }


    /// <summary>
    /// 滚动到指定索引
    /// </summary>
    /// <param name="index">item索引</param>
    private void ScrollToIndex(int index) => StartCoroutine(ScrollToPositionIEnumerator(itemCenterPosList[index]));


    /// <summary>
    /// 滚动到指定位置
    /// </summary>
    /// <param name="pos">目标位置</param>
    private void ScrollToPos(float pos) => StartCoroutine(ScrollToPositionIEnumerator(pos));


    /// <summary>
    /// 滚动到指定位置（协程调用）
    /// </summary>
    private IEnumerator ScrollToPositionIEnumerator(float pos)
    {
        float elapsedTime = 0;
        float startY = scrollRect.normalizedPosition.y;
        while (elapsedTime < transitionTime)
        {
            float newY = Mathf.Lerp(startY, pos, elapsedTime / transitionTime);
            scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, newY);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, pos);
    }

    

}
