using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CtrScroll : MonoBehaviour
{
    public ScrollRect scrollView; 
    //执行时间
    public float smoothTime = 1.0f; 

    void Start()
    {
        StartCoroutine(SmoothScrollCoroutine());
    }
    //控制进度条滚动出现
    IEnumerator SmoothScrollCoroutine()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < smoothTime)
        {
            float t = elapsedTime / smoothTime;
            float targetPosition = Mathf.SmoothStep(1.0f, 0.0f, t);
            scrollView.horizontalNormalizedPosition = targetPosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        scrollView.horizontalNormalizedPosition = 0.0f;
    }
}
