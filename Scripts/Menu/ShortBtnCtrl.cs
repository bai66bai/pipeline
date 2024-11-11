using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShortBtnCtrl : MonoBehaviour
{
    public GameObject ShortPanel;

    public Image SelectImage;
    public Image UnSelectImage;
    private bool IsOpen = true;

    public void ClickShortBtn()
    {
        if (IsOpen)
        {
            ShortPanel.SetActive(true);
            ChangeImage();
        }
        else
        {
            ShortPanel.SetActive(false);
            ChangeImage();
        }
        IsOpen = !IsOpen;
    }



    private void ChangeImage()
    {
        SelectImage.enabled = IsOpen;
        UnSelectImage.enabled = !IsOpen;
    }
}
