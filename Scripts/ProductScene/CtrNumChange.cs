using UnityEngine;
using TMPro;

public class NumberCountUp : MonoBehaviour
{
    public float targetNumber = 24; // Ŀ������
    public float duration = 3f; // ��������ʱ��
    public TextMeshProUGUI numberText; // ��ʾ���ֵ� TextMeshProUGUI ���

    private float currentNumber = 0; // ��ǰ����
    private float timer = 0; // ��ʱ��


    void Update()
    {
        if (currentNumber < targetNumber)
        {
            timer += Time.deltaTime;
            currentNumber = Mathf.SmoothStep(0, targetNumber, timer / duration);
            numberText.text = Mathf.RoundToInt(currentNumber).ToString(); // ��ʾ��������
        }
    }
}