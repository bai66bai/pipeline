using BUT.TTOR.Core;
using UnityEngine;

public class CtrPuck : MonoBehaviour
{
    //获取到当时需要展示隐藏的游戏物体
    public GameObject PuckDisplay;
    public Control_Btn ControlBtn;
    public int gap = 30;
    private float projectionDistance;
    public int CompareIndex;

    private TTOR_PuckTracker tracker;
    public ScreenCastBtn ScreenCastBtn;
    public static string currentPuckName = string.Empty;

    // private CtrBtnActive ctrBthActive;

    // 要检测的UI元素的RectTransform
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
        // 检测触摸
        if (IsNeedHandle && Input.touchCount > 0)
        {
            bool flag = false;
            // 循环所有的触摸
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                // 判断触摸是否在UI元素的RectTransform范围内
                if (RectTransformUtility.RectangleContainsScreenPoint(targetRectTransform, touch.position))
                {
                    //将控制开关变为true
                    flag = true;
                }
            }
            IsNeedHandle = flag;
            if (!IsNeedHandle) hasExecuted = false;
            //如果有触点  且触点不在范围内就隐藏显示
            if (!IsNeedHandle)
            {
                PuckDisplay.SetActive(false);
             
            }
        }
        else if (IsNeedHandle && Input.touchCount == 0)
        {

            PuckDisplay.SetActive(false);
            //可以执行按钮操作
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
    //检测冰壶移动时的效果
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


        //获取冰壶放置初始位置
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
    //冰壶移除时触发的效果
    public void PackRemoved(Puck puck)
    {
        if(currentPuckName == puck.Data.name)
        {
            //开始执行检测是否有触点
            IsNeedHandle = true;
            currentPuckName = string.Empty;
        }
    }

    private bool IsInArea(Vector3 puckPos) {
      return  (puckPos.x > -6.5 && puckPos.x < -2
             && puckPos.y > -1 && puckPos.y < 3.27);
    }

}
