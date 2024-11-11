using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementState : MonoBehaviour
{
    //存储与之对应的显示内容
    public GameObject ShowContent;

    //控制他的显示隐藏

    public void Active(bool state)
    {
        ShowContent.SetActive(state);
    }
}
