using BUT.TTOR.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrBtnActive : MonoBehaviour
{

    public List<Transform> Btns;

    public float interval = 0.15f; //物体移动时间间隔

    public float moveDuration = 0.1f;// 移动持续时间

    public RectTransform targetRectTransform;

    private Vector3 StartPosition = new Vector3 (-10.873f,-255f , 0f);

    private int puckCount = 0;

    private Vector3 EndPosition = new Vector3(-10.873f, 126f , 0f);

    private Vector3 originalEndPosition;

    private bool IsNeedHandle = false;
    void OnEnable()
    {
        originalEndPosition = EndPosition;
    }

    public void StartMove()
    {
        // 启动协程
        StartCoroutine(ExecuteRepeatedly());
    }


    // 定义协程
    IEnumerator ExecuteRepeatedly()
    {
        float actionTime = 0f;
        
        for (int i = 0; i < Btns.Count; i++)
        {
            
            actionTime += Time.deltaTime;
            float t = actionTime / interval;
            float actionPosition = StartPosition.y;
            float endPosition = i == 0 ? EndPosition.y : EndPosition.y - 90f;
            EndPosition.y = endPosition;

            StartCoroutine(MoveObject(Btns[i], EndPosition));
            yield return new WaitForSeconds(interval);
        }
    }


    IEnumerator MoveObject(Transform obj, Vector3 targetPos)
    {
        Vector3 startPos = obj.localPosition;
        float elapsedTime = 0.0f;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;

            float nextY = Mathf.SmoothStep(startPos.y, targetPos.y, t);
            obj.localPosition = new Vector3(startPos.x, nextY, startPos.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        obj.localPosition = targetPos; // 确保最后位置精确
    }


    void Update()
    {
        // 检测触摸
        if (IsNeedHandle)
        {
            bool hasAnyPointInRect = false;
            // 循环所有的触摸
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                // 判断触摸是否在UI元素的RectTransform范围内
                if (RectTransformUtility.RectangleContainsScreenPoint(targetRectTransform, touch.position))
                {
                    //将控制开关变为true
                    hasAnyPointInRect = true;
                    break;
                }
            }
            if (!hasAnyPointInRect)
            {
                IsNeedHandle = false;
                EndPosition = originalEndPosition;
                Btns.ForEach(u =>
                {
                    u.localPosition = StartPosition;
                });
                
            }

        }
    }


    public void AnyPuckCreated(Puck _)
    {
        puckCount++;
    }

    public void AnyPuckRemoved(Puck _)
    {

        puckCount--;
        if (puckCount < 1)
        {
            IsNeedHandle = true;
        }
    }

}
