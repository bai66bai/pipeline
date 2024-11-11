using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwiperMainTab : MonoBehaviour
{
   
    public TMP_FontAsset selectFont;   //选中字体
    public TMP_FontAsset defualtFont;   //默认字体
    public List<Image> images; //按钮背景图片
    public List<TextMeshProUGUI> textMeshProUGUIs;  //按钮文字内容
    public List<GameObject> MainTabs;  //按钮触发对象
    public List<GameObject> ContentList; //获取要显示的轮播区域

    public List<ScrollRect> Tumors;
    public List<ScrollRect> LungTumors;

    public GameObject TumorTab;
    public GameObject LungTumorTab;

    public TCPClient client;

    /// <summary>
    /// 开始根据静态变量判断那个内容需要显示
    /// </summary>
    private void Start()
    {
            ChangeStyle(SwiperStore.SelectedTab);
            ChangeShow(SwiperStore.SelectedTab);    
    }

    public void OnClick(GameObject go)
    {
        MainTabs.ForEach(tab =>
        {
            if(tab.name == go.name)
            {

                client.SendMessage($"loadScene:{tab.name}");
                int index = MainTabs.IndexOf(tab);
                ChangeStyle(index);  
                ChangeShow(index);
            }
        });
    }


    //改变选中字体图片样式
    public void ChangeStyle(int index)
    {  
        textMeshProUGUIs.ForEach(t =>
        {
            int tindex = textMeshProUGUIs.IndexOf(t);
            if (tindex == index)
            {
                t.font = selectFont;
                t.fontSize = 32;
                t.color = Color.white;
            }
            else
            {
                t.font = defualtFont;
                t.fontSize = 27;
                t.color = new(128 / 255f, 128 / 255f, 128 / 255f, 1f);
            }
        });
        //改变图片样式
        images.ForEach(t =>
        {
            int Index = images.IndexOf(t);
            if(Index == index)
            {
                t.color = new(49 / 255f, 94 / 255f, 205 / 255f, 1f);
            }
            else
            {
                t.color = Color.white;
            }
        });
    }
    //改变内容显示
    public void ChangeShow(int index)
    {
        if(index == 0)
        {
            ContentList[0].SetActive(true);
            ContentList[1].SetActive(false);
            LungTumors.ForEach(u =>
            {
                u.normalizedPosition = new Vector2(0,0);
                TouchSwiper touchSwiper = u.gameObject.GetComponent<TouchSwiper>();
                touchSwiper.ChangeDisAble();
            });
        }
        else
        {
            ContentList[1].SetActive(true);
            ContentList[0].SetActive(false);
            Tumors.ForEach(u =>
            {
                u.normalizedPosition = new Vector2(0, 0);
                TouchSwiper touchSwiper = u.gameObject.GetComponent<TouchSwiper>();
                touchSwiper.ChangeDisAble();
            });
        }
    }
}