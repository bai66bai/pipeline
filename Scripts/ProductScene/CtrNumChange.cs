using UnityEngine;
using TMPro;

public class NumberCountUp : MonoBehaviour
{
    public float targetNumber = 24; // 目标数字
    public float duration = 3f; // 动画持续时间
    public TextMeshProUGUI numberText; // 显示数字的 TextMeshProUGUI 组件

    private float currentNumber = 0; // 当前数字
    private float timer = 0; // 计时器


    void Update()
    {
        if (currentNumber < targetNumber)
        {
            timer += Time.deltaTime;
            currentNumber = Mathf.SmoothStep(0, targetNumber, timer / duration);
            numberText.text = Mathf.RoundToInt(currentNumber).ToString(); // 显示整数部分
        }
    }
}