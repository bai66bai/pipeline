using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CtrTextShow : MonoBehaviour
{


    public TextMeshProUGUI textComponent; // ��Ҫָ���� Text ���
    public float delay = 0.1f; // ÿ���ַ����ֵ�ʱ����

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
