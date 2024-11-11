using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreatheCtrl : MonoBehaviour
{
    public float MinScalePulse = 0.8f; // ��С����ֵ
    public float MaxScalePulse = 1.2f; // �������ֵ
    public float SpeedPulse = 0.7f; // �����ٶ�
    public float Delay = 0f;

    void Update()
    {
        if (Time.time < Delay) return;
        float scaleFactor = Mathf.SmoothStep(0, 1, Mathf.PingPong(Time.time * SpeedPulse, 1));

        float scale = Mathf.Lerp(MinScalePulse, MaxScalePulse, scaleFactor);
        transform.localScale = new Vector3(scale, scale, 1);
    }
}
