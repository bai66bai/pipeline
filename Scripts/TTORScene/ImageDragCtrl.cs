using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageDragCtrl : MonoBehaviour
{
    public GameObject ImageToDrag;
    public TCPClient tcpClient;

    private float enableClickTime = 0;
    private float bounds = 0.5f;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = ImageToDrag.transform.localPosition;
    }

    private void OnEnable()
    {
        ImageToDrag.transform.localPosition = startPosition;
        ImageToDrag.transform.localScale = Vector3.one;
    }

    public void ToggleScale()
    {
        if (Time.time < enableClickTime + bounds) return;
        Vector3 targetScale = ImageToDrag.transform.localScale == new Vector3(1, 1, 1) ? new Vector3(2f, 2f, 2f) : new Vector3(1, 1, 1);
        Vector3 targetPosition = ImageToDrag.transform.localScale == new Vector3(1, 1, 1)? ImageToDrag.transform.localPosition : startPosition;
        string command = ImageToDrag.transform.localScale == new Vector3(1, 1, 1) ? "big" : "small";
        tcpClient.SendMessage($"changeSize:{command}-{TTORStore.IP}" );
        StartCoroutine(ChangeScale(targetScale, targetPosition, 0.3f));
    }

    IEnumerator ChangeScale(Vector3 scaleEnd,Vector3 positionEnd, float duration)
    {
        Vector3 scaleStart = ImageToDrag.transform.localScale;
        Vector3 positionStart = ImageToDrag.transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // ʹ��SmoothStep���в�ֵ����
            var calcScale = Mathf.SmoothStep(scaleStart.x, scaleEnd.x, elapsedTime / duration);
            var calcPosition = Mathf.SmoothStep(positionStart.x, positionEnd.x, elapsedTime / duration);

            ImageToDrag.transform.localScale = new(calcScale, calcScale, calcScale);
            ImageToDrag.transform.localPosition = new(calcPosition, calcPosition, calcPosition);

            // �ȴ���һ֡
            yield return null;
        }

        // ȷ���������ȷ���õ�Ŀ��ֵ
        ImageToDrag.transform.localScale = scaleEnd;
        ImageToDrag.transform.localPosition = positionEnd;
    }


    public void HandleDrag(BaseEventData baseEventData)
    {
        if (ImageToDrag.transform.localScale == Vector3.one) return;
        enableClickTime = Time.time;

        if (baseEventData is PointerEventData pointerData && ImageToDrag != null)
        {
            // ��ȡ����λ�ã�������ת��Ϊ��������
            Vector2 dragPosition = pointerData.position;

            // ����ͼƬ��λ�ø��津��λ��
            ImageToDrag.transform.position = dragPosition;
        }
    }


}
