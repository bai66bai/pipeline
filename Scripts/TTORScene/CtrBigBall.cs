using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class CtrBigBall : MonoBehaviour
{

    public Transform rotationCenter;  // 旋转中心
    public float rotationSpeed = 30f;  // 旋转速度
    public float radius = 50f;         // 旋转半径
    //public float startTime;         // 启动时间
    public float StopRotation; //停止角度

    public float ScaleTime; //缩放的时间

    public float angle;  // 当前角度

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
        // 计算新的位置

        if (reverse > 0 ? angle >= StopRotation : angle <= StopRotation) {
            ShouldStop = true;
        };


        angle = reverse > 0 ? angle += rotationSpeed * Time.deltaTime : angle -= rotationSpeed * Time.deltaTime;
        float radians = angle * Mathf.Deg2Rad;

        Vector2 newPosition = new(Mathf.Cos(radians) * radius + rotationCenter.localPosition.x, Mathf.Sin(radians) * radius + rotationCenter.localPosition.y);

        // 更新UI元素的位置
        transform.localPosition = newPosition;

        // 保持UI元素的方向不变
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
        // 当物体取消激活时，恢复到原始位置
        transform.localScale = originalLocalScale;
        angle = originalAngle;
        ShouldStop = false;
    }

}
