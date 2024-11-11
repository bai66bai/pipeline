using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TtorCtrScroll : MonoBehaviour
{
    public ScrollRect scrollView;
    //Ö´ÐÐÊ±¼ä
    public float smoothTime = 1.0f;

 
    //void Start()
    //{
        
    //}

    //private void OnEnable()
    //{
    //    StartCoroutine(SmoothScrollCoroutine());  
    //}

    public void PublicScroll()=> StartCoroutine(SmoothScrollCoroutine());

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


    private void OnDisable()
    {
        scrollView.horizontalNormalizedPosition = 1.0f;
    }
}
