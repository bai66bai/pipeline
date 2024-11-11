
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabBarCtrl : MonoBehaviour
{
    public List<TextMeshProUGUI> textMeshProUGUIs;
    public List<GameObject> SwiperItems;
    public List<Image> images;

    public GameObject CtrBtn;

    public GameObject ActiveSwiper;

    public TCPClient client;

    private readonly Color _defaultColor = new(0.63f, 0.63f, 0.63f, 1f);

    public void HandleClick(string textContent,bool shouldSend = true)
    {
        textMeshProUGUIs.ForEach(textMeshPro =>
        {
            if (textMeshPro.text != textContent)
            {
                textMeshPro.color = _defaultColor;
                textMeshPro.fontStyle = FontStyles.Normal;
            }
            else
            {
                if(shouldSend)
                    client.SendMessage($"btnName:{textMeshPro.text}");
                int index = textMeshProUGUIs.IndexOf(textMeshPro);
                textMeshPro.color = new(51/255f, 51/255f, 51/255f, 1f);
                textMeshPro.fontStyle = FontStyles.Bold;

             
                SwiperItems.ForEach(item =>
                {
                    int Sindex = SwiperItems.IndexOf(item);
                    if (Sindex == index)
                    {
                        ActiveSwiper = item;
                        item.SetActive(true);
                        CtrBtn.GetComponent<Turnthepage>().ChangeActive();
                        images[Sindex].enabled = true; 
                    }
                    else
                    {
                        item.SetActive(false);
                        images[Sindex].enabled = false;  
                    }
                });
            }
        });
    }

    //隐藏时将页面从重置
    private void OnDisable()
    {
        HandleClick("ALL", false);
    }
}
    