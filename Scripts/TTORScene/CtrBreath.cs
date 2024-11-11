using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CtrBreath : MonoBehaviour
{
    private Material material;

    public float MinSize = 0.3f;
    public float MaxSize = 5f;
    public float SpeedPulse = 0.7f;
    private void Start()
    {
        material = GetComponent<Image>().material;
    }

    private void Update()
    {
        float breathFactor = Mathf.SmoothStep(0, 1, Mathf.PingPong(Time.time * SpeedPulse, 1));

        float breath = Mathf.Lerp(MinSize, MaxSize, breathFactor);
        material.SetFloat("_Strength",breath);
    }
}
