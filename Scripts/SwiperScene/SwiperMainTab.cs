using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwiperMainTab : MonoBehaviour
{
   
    public TMP_FontAsset selectFont;   //ѡ������
    public TMP_FontAsset defualtFont;   //Ĭ������
    public List<Image> images; //��ť����ͼƬ
    public List<TextMeshProUGUI> textMeshProUGUIs;  //��ť��������
    public List<GameObject> MainTabs;  //��ť��������
    public List<GameObject> ContentList; //��ȡҪ��ʾ���ֲ�����

    public List<ScrollRect> Tumors;
    public List<ScrollRect> LungTumors;

    public GameObject TumorTab;
    public GameObject LungTumorTab;

    public TCPClient client;

    /// <summary>
    /// ��ʼ���ݾ�̬�����ж��Ǹ�������Ҫ��ʾ
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


    //�ı�ѡ������ͼƬ��ʽ
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
        //�ı�ͼƬ��ʽ
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
    //�ı�������ʾ
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