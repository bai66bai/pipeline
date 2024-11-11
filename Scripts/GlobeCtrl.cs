using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobeCtrl : MonoBehaviour
{
    public Transform Globe;

    public float ReturnSpeed = 0.01f;
    public float RotationSpeed = 10.0f;

    private float _rotationToPinDuration = 2f;
    private Quaternion _targetRotation = Quaternion.identity;

    // Update is called once per frame
    void Update()
    {
        Quaternion rotationSpeed = Quaternion.Euler(0f, -RotationSpeed * Time.deltaTime, 0f);
        _targetRotation *= rotationSpeed;

        if (_targetRotation != null)
        {
            Globe.rotation = Quaternion.Slerp(Globe.rotation, _targetRotation, _rotationToPinDuration * Time.deltaTime);
        }
    }
}
