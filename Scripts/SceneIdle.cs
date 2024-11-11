using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneIdle : MonoBehaviour
{

    public float timeoutSeconds = 5; // �û��޲����ĳ�ʱʱ�䣨�룩
    private float lastInteractionTime; // ��¼������ʱ��

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        // ����Ƿ��м������������ƶ�
        if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Space))
        {
            ResetTimer();
        }

        // ����Ƿ�ʱ
        if (Time.time - lastInteractionTime > timeoutSeconds)
        {
            SceneManager.LoadScene("ProductScene");
        }
    }

    private void OnEnable() => ResetTimer();

    // ���ü�ʱ��
    void ResetTimer()
    {
        lastInteractionTime = Time.time;
    }

}