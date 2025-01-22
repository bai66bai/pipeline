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

        // ʹ��UnityWebRequest��ȡJSON�ļ�
        using UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
        // �������󲢵ȴ���Ӧ
        yield return www.SendWebRequest();

        // ����Ƿ��д���
        if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error reading JSON file: " + www.error);
        }
        else
        {
            // ����JSON����
            string json = www.downloadHandler.text;
            // ����ΪSourceData����
            SourceDate sourceDate = JsonConvert.DeserializeObject<SourceDate>(json);
            ctrlIdel.timeoutSeconds = sourceDate.IdelTime;
        }
    }
}
