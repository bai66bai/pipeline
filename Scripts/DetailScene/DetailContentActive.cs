using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailContentActive : MonoBehaviour
{
    public List<Transform> Contents;

    public float interval = 0.15f; //物体移动时间间隔

    public float moveDuration = 0.1f;// 移动持续时间

    private Vector3 StartPosition = new Vector3(0f, -850f, 0f);

    private Vector3 EndPosition = new Vector3(0f, 0f, 0f);



    void OnEnable()
    {
        StartMove();

    }

    public void StartMove()
    {
        // 启动协程
        StartCoroutine(ExecuteRepeatedly());
    }


    // 定义协程
    IEnumerator ExecuteRepeatedly()
    {
        float actionTime = 0f;

        for (int i = 0; i < Contents.Count; i++)
        {

            actionTime += Time.deltaTime;
            float t = actionTime / interval;
            float actionPosition = StartPosition.y;
            StartCoroutine(MoveObject(Contents[i], EndPosition));
            yield return new WaitForSeconds(interval);
        }
    }


    IEnumerator MoveObject(Transform obj, Vector3 targetPos)
    {
        Vector3 startPos = obj.localPosition;
        float elapsedTime = 0.0f;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;

            float nextY = Mathf.SmoothStep(startPos.y, targetPos.y, t);
            obj.localPosition = new Vector3(startPos.x, nextY, startPos.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        obj.localPosition = targetPos; // 确保最后位置精确
    }


    private void OnDisable()
    {
        Contents.ForEach(u =>
        {
            u.localPosition = StartPosition;
        });
    }
}
