using System.Collections;
using TMPro;
using UnityEngine;

public class TestTouch : MonoBehaviour
{
    public TextMeshProUGUI AddText;
    public GameObject ExistText;
    public TextMeshProUGUI paramText;
    private float elapsedTime = 0f;
 
    private bool flag = false;
    private string time;
    // Update is called once per frame
    void Update()
    {
        time = $"phase: {elapsedTime}";
        // ÿ֡����ִ�еļ���,���ڼ��Update�Ƿ���������
        AddText.text = time;

        if (Input.touchCount != 0)
        {
            if (!flag)
            {
                StartCoroutine(StartAccumulatingTimer());
                flag = true;
            }   
            // ���ڼ���Ƿ���ڴ���
            ExistText.SetActive(true);
            string pointsParams = string.Empty;
            foreach (Touch touch in Input.touches)
            {
                // ���ڴ�ӡÿһ������Ĳ���
                pointsParams +=
                    $"phase: {touch.phase} \n" +
                    $"pressure: {touch.pressure} \n" +
                    $"position: {touch.position}\n\n";
            }
            paramText.text = pointsParams;
        }
        else
        {
            if (flag)
            {
                flag = false;
                StopCoroutine(StartAccumulatingTimer());
            }
            
            ExistText.SetActive(false);
        }
    }


    public void resatTime()
    {
        elapsedTime = 0f;
    }

    IEnumerator StartAccumulatingTimer()
    {
        while (true)  // ��Զѭ��
        {
            elapsedTime += 1f;  // ÿ���ۼ�1��
            Debug.Log("���ۼ�ʱ��: " + elapsedTime + " ��");
            yield return new WaitForSeconds(1f);  // �ȴ�1����
        }
    }
}
