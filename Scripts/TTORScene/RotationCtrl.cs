using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationCtrl : MonoBehaviour
{
    public float Delay = 0f;
    public Vector3 rotationSpeed = new(0, 0, 10);

    void Update()
    {
        if (Time.time < Delay) return;
        transform.Rotate(-rotationSpeed * Time.deltaTime);
    }
}
