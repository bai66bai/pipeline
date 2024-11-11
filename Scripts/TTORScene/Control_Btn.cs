using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Control_Btn : MonoBehaviour
{

    public List<GameObject> BtnList;
    public List<GameObject> ShowContent;
    public TCPClient client;
    public List<GameObject> Texts;

    private int currentIndex;
    public int CurrentIndex
    {
        get => currentIndex;
        set {
            currentIndex = value;
            ChangeContent(CurrentIndex);
        }
    }

    public void OnClick(GameObject go)
    {
        BtnList.ForEach(u =>

        {      int index = BtnList.IndexOf(u);
            if(u.name == go.name)
            {
                TTORStore.BtnIndex = index;
                client.SendMessage($"TBtnName:{u.name}-{TTORStore.IP}");
                CurrentIndex = index;
                foreach (var item in ShowContent)
                {
                    int Sindex = ShowContent.IndexOf(item);
                    item.SetActive(Sindex == CurrentIndex);
                }
                Texts[index].SetActive(true);
                Image[] images = u.GetComponentsInChildren<Image>();
                images[0].enabled = true;
                images[1].enabled = false;  
            }
            else
            {
                Texts[index].SetActive(false);
                Image[] images = u.GetComponentsInChildren<Image>();
                images[0].enabled = false;
                images[1].enabled = true;
            }
        });
    }


    public void ChangeContent(int index)
    {
        BtnList.ForEach(u =>
        {
           int Sindex = BtnList.IndexOf(u);
            if(index == Sindex)
            {
                TTORStore.BtnIndex = index;
                client.SendMessage($"TBtnName:{u.name}-{TTORStore.IP}");
                foreach (var item in ShowContent)
                {
                    int index = ShowContent.IndexOf(item);
                    item.SetActive(index == CurrentIndex);
                }
                Texts[Sindex].SetActive(true);
                Image[] images = BtnList[Sindex].GetComponentsInChildren<Image>();
                images[0].enabled = true;
                images[1].enabled = false;
            }
            else
            {
                Texts[Sindex].SetActive(false);
                Image[] images = BtnList[Sindex].GetComponentsInChildren<Image>();
                images[0].enabled = false;
                images[1].enabled = true;
            }
        });
    }



}
