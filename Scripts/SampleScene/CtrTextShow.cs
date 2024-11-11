using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CtrTextShow : MonoBehaviour
{


    public TextMeshProUGUI textComponent; // 需要指定的 Text 组件
    public float delay = 0.1f; // 每个字符出现的时间间隔

    private string fullText;
    private string currentText = "";

    private void Start()
    {
        if (textComponent != null)
        {
            fullText = textComponent.text;
            textComponent.text = "";
            StartCoroutine(ShowText());
        }
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            currentText = fullText.Substring(0, i + 1);
            textComponent.text = currentText;
            yield return new WaitForSeconds(delay);
        }
    }


}
