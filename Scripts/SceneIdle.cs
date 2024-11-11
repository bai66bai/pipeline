using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneIdle : MonoBehaviour
{

    public float timeoutSeconds = 5; // 用户无操作的超时时间（秒）
    private float lastInteractionTime; // 记录最后操作时间

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        // 检查是否有键盘输入或鼠标移动
        if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space))
        {
            ResetTimer();
        }

        // 检查是否超时
        if (Time.time - lastInteractionTime > timeoutSeconds)
        {
            SceneManager.LoadScene("ProductScene");
        }
    }

    private void OnEnable() => ResetTimer();

    // 重置计时器
    void ResetTimer()
    {
        lastInteractionTime = Time.time;
    }

}