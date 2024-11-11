using BUT.TTOR.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreakTTORIdle : MonoBehaviour
{
    public List<Image> images;

    public float fadeDuration = 1.0f;

    // Ҫ����UIԪ�ص�RectTransform
    public RectTransform targetRectTransform;

    private int puckCount = 0;

    private Image bgImage;

    private Material bgMaterial;

    private bool IsNeedHandle = false;

    private Coroutine BackCorutine;

    private HashSet<PuckData> puckSet = new();

    private void Start()
    {
        bgImage = GetComponent<Image>();
        bgMaterial = bgImage.material;
        bgMaterial.SetFloat("_Progress", 0);
    }

    void Update()
    {
        // ��ⴥ��
        if (IsNeedHandle)
        {
            bool hasAnyPointInRect = false;
            // ѭ�����еĴ���
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                // �жϴ����Ƿ���UIԪ�ص�RectTransform��Χ��
                if (RectTransformUtility.RectangleContainsScreenPoint(targetRectTransform, touch.position))
                {
                    //�����ƿ��ر�Ϊtrue
                    hasAnyPointInRect = true;
                    break;
                }
            }
            if (!hasAnyPointInRect && puckCount == 0)
            {
                IsNeedHandle = false;
                BackCorutine = StartCoroutine(CircularEraseBack());   
            }
        }
    }



    public void AnyPuckCreated(Puck addedPuck)
    {
        if (puckSet.Contains(addedPuck.Data))
            return;
        if (puckCount == 0)
        {
            if(BackCorutine != null)
                StopCoroutine(BackCorutine);
            StartCoroutine(CircularEraseIn());

            foreach (var item in images)
            {
                item.color = new(item.color.r, item.color.g, item.color.b, 0);
            }

            foreach (var item in images)
            {
                StartCoroutine(FadeOut(item));
            }

        }
        puckSet.Add(addedPuck.Data);
        puckCount++;
    }

    public void AnyPuckRemoved(Puck removedPuck)
    {
        if(puckSet.Contains(removedPuck.Data))
        {
            puckSet.Remove(removedPuck.Data);

            puckCount--;
        }
        
        if (puckCount <1)
        {
            IsNeedHandle = true;
        }
    }


    IEnumerator FadeOut(Image imageToFade)
    {
        float counter = 0f;

        // ��ȡImage����ĳ�ʼ��ɫ
        Color imageColor = imageToFade.color;

        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.SmoothStep(1.0f, 0f, counter / fadeDuration);

            // ����Image����ɫ��͸����
            imageToFade.color = new Color(imageColor.r, imageColor.g, imageColor.b, alpha);
            yield return null; // �ȴ�һ֡
        }
        bgImage.enabled = false;
        // ȷ��alphaֵ����Ϊ��ȫ͸��
        imageToFade.color = new Color(imageColor.r, imageColor.g, imageColor.b, 0);
    }

    IEnumerator FadeIn(Image imageToFade)
    {
        float counter = 0f;

        // ��ȡImage����ĳ�ʼ��ɫ
        Color imageColor = imageToFade.color;

        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.SmoothStep(0f, 1.0f, counter / fadeDuration);

            // ����Image����ɫ��͸����
            imageToFade.color = new Color(imageColor.r, imageColor.g, imageColor.b, alpha);
            yield return null; // �ȴ�һ֡
        }

        // ȷ��alphaֵ����Ϊ��ȫ͸��
        imageToFade.color = new Color(imageColor.r, imageColor.g, imageColor.b, 1.0f);
    }

    IEnumerator CircularEraseIn()
    {
        float counter = 0f;

        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            float targetProgress = Mathf.SmoothStep(0f, 1.0f, counter / fadeDuration);

            bgMaterial.SetFloat("_Progress", targetProgress);
            yield return null; // �ȴ�һ֡
        }

        bgMaterial.SetFloat("_Progress", 1.0f);
    }

    IEnumerator CircularEraseBack()
    {
        bgImage.enabled = true;
        float counter = 0f;

        while (counter < fadeDuration)
        {
            counter += Time.deltaTime;
            float targetProgress = Mathf.SmoothStep(1.0f, 0f, counter / fadeDuration);

            bgMaterial.SetFloat("_Progress", targetProgress);
            yield return null; // �ȴ�һ֡
        }

        bgMaterial.SetFloat("_Progress", 0f);

        

        foreach (var item in images)
        {
            StartCoroutine(FadeIn(item));
        }
    }
}
