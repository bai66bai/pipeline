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
        // ��龲̬����ActiveDetailText���жϵ�ǰԪ��
        // 1.�����Ԫ��
        currentItem = menuItems
            .Where(m => m.Text == DetailStore.ActiveDetailText)
            .ToList()
            .First();
        activateCurrent();

        // 2.��������Ԫ�ص�λ��
        int currentItemIndex = menuItems.IndexOf(currentItem);
        ScrollToIndex(currentItemIndex);
    }


    /// <summary>
    /// ��ʼ��ק�ص�
    /// </summary>
    /// <param name="eventData">�¼�����</param>
    public void OnBeginDrag(PointerEventData eventData) { }


    /// <summary>
    /// ������ק�ص�
    /// </summary>
    /// <param name="eventData">�¼�����</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        UpdateNearestItemCenterPos();

        ActivateItemByPos(nearestPos);

        ScrollToPos(nearestPos);
    }


    /// <summary>
    /// ���¾��뵱ǰλ�������Ԫ������λ��
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
    /// ����ָ��λ�õ�Ԫ��
    /// </summary>
    /// <param name="pos">Ԫ�ص�λ��</param>
    private void ActivateItemByPos(float pos)
    {
        currentItem.InactivateItem();

        int currentIndex = itemCenterPosList.IndexOf(pos);
        currentItem = menuItems[currentIndex];
        activateCurrent();


    }


    /// <summary>
    /// ������ָ������
    /// </summary>
    /// <param name="index">item����</param>
    private void ScrollToIndex(int index) => StartCoroutine(ScrollToPositionIEnumerator(itemCenterPosList[index]));


    /// <summary>
    /// ������ָ��λ��
    /// </summary>
    /// <param name="pos">Ŀ��λ��</param>
    private void ScrollToPos(float pos) => StartCoroutine(ScrollToPositionIEnumerator(pos));


    /// <summary>
    /// ������ָ��λ�ã�Э�̵��ã�
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
