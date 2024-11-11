using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoBtnCtrl : MonoBehaviour
{
  public LevelLoader LevelLoader;

    public Image SelectImage;
    public Image UnSelectImage;
    private bool IsOpen = true;

    public void ClickVideoBtn(string SceneName)
    {
        if (IsOpen)
        {
            LevelLoader.LoadNewScene(SceneName);
            ChangeImage();
        }
    }



    private void ChangeImage()
    {
        SelectImage.enabled = IsOpen;
        UnSelectImage.enabled = !IsOpen;
    }
}
