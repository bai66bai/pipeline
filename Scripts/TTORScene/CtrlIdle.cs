using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CtrlIdle : MonoBehaviour
{
    public float timeoutSeconds = 20; // 用户无操作的超时时间（秒）
    private float lastInteractionTime; // 记录最后操作时间


    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        if (TTORStore.IsShow)
        {
            // 检查是否有键盘输入或鼠标移动
            if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space))
            {
                ResetTimer();
            }
        }
        
        // 检查是否超时
        if (Time.time - lastInteractionTime > timeoutSeconds)
        {
            TTORStore.IsShow = false;
        }
    }

    // 重置计时器
    void ResetTimer()
    {
        lastInteractionTime = Time.time;
    }
}
