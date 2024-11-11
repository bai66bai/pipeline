using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class BtnControl : MonoBehaviour
{

    public List<GameObject> buttons;
    private Camera mainCamera;

    /// <summary>
    /// 项目中模型
    /// </summary>
    public GameObject Modlel;

    //存储项目中的数据坐标
    Quaternion OncologyEuler = Quaternion.Euler(0f, 180f, 0f);
    Vector3 OncologyPosition;
    //Quaternion DefualtRot = Quaternion.Euler(0f, 155f, 0f);
    Quaternion DefualtRot = Quaternion.Euler(0f, 180f, 0f);
    Vector3 DefualtPos = new Vector3(0f, 0f, 0f);


    public float endScale = 3f;       
    public float duration = 1f;      
    private float currentTime = 1f;
    public List<GameObject> Pins;
    //控制按钮
    private bool isBegin = true;
    //判断是否移动完成
    private bool isFinished = true;

    private void Start()
    {
        mainCamera = Camera.main;

    }


    public  void OnClick(GameObject go)
    {
        if (isFinished)
        {
            buttons.ForEach(p =>
                    {

                        if (p.name == go.name)
                        {
                            endScale = go.name == "SkeletonBtn" ? 7.5f : 3f;
                            //获取当前按钮的索引
                            int tIndex = buttons.IndexOf(p);
                            OncologyPosition = Pins[tIndex].transform.localPosition; 
                            ManagementState MyScript = Pins[tIndex].GetComponent<ManagementState>();
                            MyScript.Active(true);
                            EventTrigger eventTrigger = p.GetComponent<EventTrigger>();
                            eventTrigger.enabled = false;
                            Image[] images = p.GetComponentsInChildren<Image>();
                            images[0].enabled = true;
                            images[1].enabled = false;   
                        }
                        else
                        {
                            int tIndex = buttons.IndexOf(p);
                            ManagementState MyScript = Pins[tIndex].GetComponent<ManagementState>();
                            MyScript.Active(false);
                            EventTrigger eventTrigger = p.GetComponent<EventTrigger>();
                            eventTrigger.enabled = true;
                            Image[] images = p.GetComponentsInChildren<Image>();
                            images[0].enabled = false;
                            images[1].enabled = true;
                        }
                    });
        
                    if (isBegin)
                    {
                        StartCoroutine(ScaleCamera(isBegin, duration));
                        StartCoroutine(MoveCameraCoroutine(OncologyPosition, OncologyEuler,duration , true));
                        isBegin = false;
                        isFinished = false;
                    }
                    else
                    {
                        StartCoroutine(ScaleCamera(isBegin, duration * 0.5f));      
                        StartCoroutine(MoveCameraCoroutine(DefualtPos, DefualtRot, duration * 0.5f,false ));
                        isFinished = false;
            }
        }
    }



    /// <summary>
    /// 使协程成实现模型的旋转 摄像机的移动
    /// </summary>
    /// <param name="pos">相机移动目标位置</param>
    /// <param name="rot">物体旋转角度</param>
    /// <param name="calcDuration">执行时间</param>
    /// <param name="isChangeBtn">是否点击其他按钮</param>
    /// <returns></returns>  
    IEnumerator MoveCameraCoroutine(Vector3 pos, Quaternion rot ,float calcDuration,bool isChangeBtn )
    {
        Vector3 startPosition = mainCamera.transform.localPosition; 
        Quaternion startRotation = Modlel.transform.rotation;  
        float elapsedTime = 0f;

        while (elapsedTime < calcDuration)
        {
            float t = elapsedTime / calcDuration;
            float nextX = Mathf.SmoothStep(startPosition.x, pos.x, t);
            float nextY = Mathf.SmoothStep(startPosition.y, pos.y, t);
            float nextZ = Mathf.SmoothStep(startPosition.z, pos.z, t);
            mainCamera.transform.localPosition = new Vector3(nextX,nextY,nextZ);
            Modlel.transform.rotation = Quaternion.Lerp(startRotation, rot, t);            
            elapsedTime += Time.deltaTime;           
            yield return null;
        }
        // 确保摄像头最终达到目标位置
        //mainCamera.transform.localPosition = pos;
        //Modlel.transform.rotation = rot;
        if (!isChangeBtn)
        {
            StartCoroutine(ScaleCamera(!isChangeBtn, calcDuration*2));
            StartCoroutine(MoveCameraCoroutine(OncologyPosition, OncologyEuler, calcDuration*2, true));
            isFinished = false;
        }
    }


    //使用协程实现摄像机的放大缩小
    private IEnumerator ScaleCamera(bool IsScale, float calcDuration)
    {
            float startScale = mainCamera.orthographicSize;
            currentTime = 0f;
            while (currentTime < calcDuration)
            {
                currentTime += Time.deltaTime;  
                float t = currentTime / calcDuration;  
                float scale = Mathf.SmoothStep(startScale, IsScale ? endScale : 4.5f, t); 
                mainCamera.orthographicSize = scale;  
                yield return null;
            }
            isFinished = true ;
    }
}
