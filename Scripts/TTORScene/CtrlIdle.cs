using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CtrlIdle : MonoBehaviour
{
    public float timeoutSeconds = 20; // �û��޲����ĳ�ʱʱ�䣨�룩
    private float lastInteractionTime; // ��¼������ʱ��


    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        if (TTORStore.IsShow)
        {
            // ����Ƿ��м������������ƶ�
            if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space))
            {
                ResetTimer();
            }
        }
        
        // ����Ƿ�ʱ
        if (Time.time - lastInteractionTime > timeoutSeconds)
        {
            TTORStore.IsShow = false;
        }
    }

    // ���ü�ʱ��
    void ResetTimer()
    {
        lastInteractionTime = Time.time;
    }
}
