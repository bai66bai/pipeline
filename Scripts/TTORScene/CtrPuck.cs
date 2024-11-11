using BUT.TTOR.Core;
using UnityEngine;

public class CtrPuck : MonoBehaviour
{
    //��ȡ����ʱ��Ҫչʾ���ص���Ϸ����
    public GameObject PuckDisplay;
    public Control_Btn ControlBtn;
    public int gap = 30;
    private float projectionDistance;
    public int CompareIndex;

    private TTOR_PuckTracker tracker;
    public ScreenCastBtn ScreenCastBtn;
    public static string currentPuckName = string.Empty;

    // private CtrBtnActive ctrBthActive;

    // Ҫ����UIԪ�ص�RectTransform
    public RectTransform targetRectTransform;
    private void Start()
    {
        GameObject trackerObj = GameObject.Find("TTOR_PuckTracker");
        // ctrBthActive = GetComponent<CtrBtnActive>();

        tracker = trackerObj.GetComponent<TTOR_PuckTracker>();
        projectionDistance = tracker.ProjectionDistance;
    }


    private bool IsNeedHandle = false;

    void Update()
    {
        // ��ⴥ��
        if (IsNeedHandle && Input.touchCount > 0)
        {
            bool flag = false;
            // ѭ�����еĴ���
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                // �жϴ����Ƿ���UIԪ�ص�RectTransform��Χ��
                if (RectTransformUtility.RectangleContainsScreenPoint(targetRectTransform, touch.position))
                {
                    //�����ƿ��ر�Ϊtrue
                    flag = true;
                }
            }
            IsNeedHandle = flag;
            if (!IsNeedHandle) hasExecuted = false;
            //����д���  �Ҵ��㲻�ڷ�Χ�ھ�������ʾ
            if (!IsNeedHandle)
            {
                PuckDisplay.SetActive(false);
             
            }
        }
        else if (IsNeedHandle && Input.touchCount == 0)
        {

            PuckDisplay.SetActive(false);
            //����ִ�а�ť����
            hasExecuted = false;
            IsNeedHandle = false;
        }
    }

    private int currentIndex
    {
        get => ControlBtn.CurrentIndex;
        set
        {
            ControlBtn.CurrentIndex = value;
        }
    }

    private float lastAngle;

    private float topBounds
    {
        get
        {
            var bounds = lastAngle - gap;
            return bounds < 0 ? 360 + bounds : bounds;
        }
    }

    private float bottomBounds
    {
        get
        {
            var bounds = lastAngle + gap;
            return bounds > 360 ? bounds - 360 : bounds;
        }
    }
    private bool hasExecuted = false;
    //�������ƶ�ʱ��Ч��
    public void PuckMove(Puck puck)
    {
        if (currentPuckName != string.Empty)
        {
            if (currentPuckName != puck.Data.name) return;
        }
        else
        {
            currentPuckName = puck.Data.name;
        }


        //��ȡ�������ó�ʼλ��
        var actionPositin = puck.GetPosition(projectionDistance);
        //Debug.Log(topBounds);

        if (IsInArea(actionPositin))
        {

            if (!hasExecuted)
            {
                //ctrBthActive.StartMove();
                hasExecuted = true;
            }
            PuckDisplay.SetActive(true);
            float degree = puck.GetRotation().eulerAngles.z;
            float topDifference;

            if (degree < 180 && topBounds > 180 && Mathf.Abs(degree - 180) > 60)
                topDifference = (degree + 360) - topBounds;
            else
                topDifference = degree - topBounds;
            if (topDifference < 0)
            {
                lastAngle = degree;

                currentIndex = currentIndex == 0 ? 0 : currentIndex - 1;

                return;
            }

            float bottomDifference;
            if (bottomBounds < 180 && degree > 180 && Mathf.Abs(degree - 180) > 60)
                bottomDifference = (bottomBounds + 360) - degree;
            else if (degree < 180 && bottomBounds > 180 && Mathf.Abs(degree - 180) > 60)
                bottomDifference = bottomBounds - (degree + 360);
            else
                bottomDifference = bottomBounds - degree;
            if (bottomDifference < 0)
            {
                lastAngle = degree;

                currentIndex = currentIndex == CompareIndex ? CompareIndex : currentIndex + 1;

                return;
            }

        }
        else
        {
            PuckDisplay.SetActive(false);
        }
    }
    //�����Ƴ�ʱ������Ч��
    public void PackRemoved(Puck puck)
    {
        if(currentPuckName == puck.Data.name)
        {
            //��ʼִ�м���Ƿ��д���
            IsNeedHandle = true;
            currentPuckName = string.Empty;
        }
    }

    private bool IsInArea(Vector3 puckPos) {
      return  (puckPos.x > -6.5 && puckPos.x < -2
             && puckPos.y > -1 && puckPos.y < 3.27);
    }

}
