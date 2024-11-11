using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCastBtn : MonoBehaviour
{
    public Image ShowImage;
    public Image HeadImage;
    public TCPClient client;
    public Control_Btn btn;
    public List<GameObject> ContentList;
    public ControlContent ControlContent;
    public bool IsOnScreen
    {
        get => !IsScreenCasting;
    }

    public bool IsScreenCasting = true;

    private void OnEnable()
    {
        ContentList.ForEach(content =>
        {
            if (content.activeSelf == true)
            {
                TTORStore.BtnIndex = ContentList.IndexOf(content);
            }
        });
    }
    public void ChangeScreenCast(string MedicalName)
    {
        if (IsScreenCasting)
        {
            string BtnName = btn.BtnList[TTORStore.BtnIndex].name;
            string BtnParam = $"TBtnName|{BtnName}";
            string MedParam = $"MedicalName|{MedicalName}";
            string IDParam = $"ID|{TTORStore.IP}";
            client.SendMessage($"loadScene:TTOR_Scene:{BtnParam}:{MedParam}:{IDParam}");
            ChangeImage();
            ControlContent?.RestMBtn();
        }
        else
        {
            client.SendMessage($"close:screenCasting-{TTORStore.IP}");
            ChangeImage();
        }
        IsScreenCasting = !IsScreenCasting;
    }

    public void ChangeImage()
    {
        if (IsScreenCasting)
        {
            ShowImage.enabled = true;
            HeadImage.enabled = false;
        }
        else
        {
            ShowImage.enabled = false;
            HeadImage.enabled = true;
        }
    }
}
