using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementState : MonoBehaviour
{
    //�洢��֮��Ӧ����ʾ����
    public GameObject ShowContent;

    //����������ʾ����

    public void Active(bool state)
    {
        ShowContent.SetActive(state);
    }
}
