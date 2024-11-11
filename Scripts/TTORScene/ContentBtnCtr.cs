using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class ContentBtnCtr : MonoBehaviour
{
    public List<GameObject> BtnCtrs; 
    public List<GameObject> ContentList;
    public List<TextMeshProUGUI> textMeshProUGUIs;
    public List<Image> BgImages;

   
    public void BtnClick(GameObject go)
    {

        

        BtnCtrs.ForEach(c =>
        {
            if (c.name == go.name)
            {
                int index = BtnCtrs.IndexOf(c);
                ChangeText(index); 
                ChangeContentActive(index); 
                Image image = c.GetComponentInChildren<Image>();
                image.enabled = false;
            }
            else
            {
                Image image = c.GetComponentInChildren<Image>();
                image.enabled = true;
            }
        });
    }   
    public void ChangeText( int index)
    {
        textMeshProUGUIs.ForEach(t =>
        {
            //�洢Ĭ��������ʽ
            
            int tindex = textMeshProUGUIs.IndexOf(t);
            if(tindex == index)
            {
                //�ı�������ɫ�Լ�������ʽ��С
                
                t.fontStyle = FontStyles.Bold;
                t.fontSize = 22;
                t.color = new(49 / 255f, 94 / 255f, 205 / 255f, 1f);
            }
            else
            {
                t.fontStyle = FontStyles.Normal;
                t.fontSize = 19;
                t.color = new(153 / 255f , 153 / 255f , 153 / 255f , 1f);
            }
        });
    }

   

    //�������ݵ���ʾ����
    public void ChangeContentActive(int index)
    {
        foreach (var item in ContentList)
        {
            int Sindex = ContentList.IndexOf(item);
            //Debug.Log(Sindex);
            if (Sindex == index)
            {
                item.SetActive(true);  
                BgImages[Sindex].enabled = true;
            }
            else
            {
                item.SetActive(false);
                BgImages[Sindex].enabled = false;
            }
        }
    }
}
