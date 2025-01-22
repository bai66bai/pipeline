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
        // 每帧都会执行的计数,用于检测Update是否正常工作
        AddText.text = time;

        if (Input.touchCount != 0)
        {
            if (!flag)
            {
                StartCoroutine(StartAccumulatingTimer());
                flag = true;
            }   
            // 用于检测是否存在触点
            ExistText.SetActive(true);
            string pointsParams = string.Empty;
            foreach (Touch touch in Input.touches)
            {
                // 用于打印每一个触点的参数
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
        while (true)  // 永远循环
        {
            elapsedTime += 1f;  // 每秒累加1秒
            Debug.Log("已累加时间: " + elapsedTime + " 秒");
            yield return new WaitForSeconds(1f);  // 等待1秒钟
        }
    }
}
