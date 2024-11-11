using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class CtrBigBall : MonoBehaviour
{

    public Transform rotationCenter;  // ��ת����
    public float rotationSpeed = 30f;  // ��ת�ٶ�
    public float radius = 50f;         // ��ת�뾶
    //public float startTime;         // ����ʱ��
    public float StopRotation; //ֹͣ�Ƕ�

    public float ScaleTime; //���ŵ�ʱ��

    public float angle;  // ��ǰ�Ƕ�

    public int reverse;

    private static bool ShouldStop = false;

    private Vector3 originalLocalScale;

    private float originalAngle;

    void OnEnable()
    {
        originalLocalScale = transform.localScale;
        originalAngle = angle;
    }

    void Update()
    {
        if (ShouldStop) return;

        StartCoroutine(ScaleBall());
        // �����µ�λ��

        if (reverse > 0 ? angle >= StopRotation : angle <= StopRotation) {
            ShouldStop = true;
        };


        angle = reverse > 0 ? angle += rotationSpeed * Time.deltaTime : angle -= rotationSpeed * Time.deltaTime;
        float radians = angle * Mathf.Deg2Rad;

        Vector2 newPosition = new(Mathf.Cos(radians) * radius + rotationCenter.localPosition.x, Mathf.Sin(radians) * radius + rotationCenter.localPosition.y);

        // ����UIԪ�ص�λ��
        transform.localPosition = newPosition;

        // ����UIԪ�صķ��򲻱�
        transform.rotation = Quaternion.identity;
    }

    IEnumerator ScaleBall()
    {
        

        Vector3 StartScale = transform.localScale;
        float elapsedTime = 0.0f;
        while (elapsedTime < ScaleTime)
        {
            float t = elapsedTime / ScaleTime;
            float nextX = Mathf.Lerp(StartScale.x , 1f,t);
            float nextY = Mathf.Lerp(StartScale.y , 1f,t);
            transform.localScale = new Vector3(nextX, nextY, 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }


    }

    void OnDisable()
    {
        // ������ȡ������ʱ���ָ���ԭʼλ��
        transform.localScale = originalLocalScale;
        angle = originalAngle;
        ShouldStop = false;
    }

}
