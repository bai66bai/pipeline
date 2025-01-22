using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Deserialization : MonoBehaviour
{
    public CtrlIdle ctrlIdel;
    private string fileName = "Source.json";
    void Start()
    {
        StartCoroutine(ReadJSON());
    }

    private IEnumerator ReadJSON()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        // 使用UnityWebRequest读取JSON文件
        using UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
        // 发送请求并等待响应
        yield return www.SendWebRequest();

        // 检查是否有错误
        if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error reading JSON file: " + www.error);
        }
        else
        {
            // 解析JSON内容
            string json = www.downloadHandler.text;
            // 解析为SourceData对象
            SourceDate sourceDate = JsonConvert.DeserializeObject<SourceDate>(json);
            ctrlIdel.timeoutSeconds = sourceDate.IdelTime;
        }
    }
}
