
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Turnthepage : MonoBehaviour
{
   public GameObject TabBar;

    public List<GameObject> Btns;

    private TouchSwiper touchSwiper;

    private GameObject ActiveSwiper;

    private Image[] Lefts;
    private Image[] Rights;
    private void Start()
    {

        //获取tabbar上的脚本暴露出来的对象
        ActiveSwiper = TabBar.GetComponent<TabBarCtrl>().ActiveSwiper;
        touchSwiper = ActiveSwiper.GetComponentInChildren<TouchSwiper>();
        Lefts = Btns[0].GetComponentsInChildren<Image>();
        Rights = Btns[1].GetComponentsInChildren<Image>();
        ChangeBtnColor(0f);
    }

    private void OnEnable()
    {
        ActiveSwiper = TabBar.GetComponent<TabBarCtrl>().ActiveSwiper;
        touchSwiper = ActiveSwiper.GetComponentInChildren<TouchSwiper>();
        Lefts = Btns[0].GetComponentsInChildren<Image>();
        Rights = Btns[1].GetComponentsInChildren<Image>();
        ChangeBtnColor(0f);
    }

    public void ChangeActive()
    {
        ActiveSwiper = TabBar.GetComponent<TabBarCtrl>().ActiveSwiper;
        touchSwiper = ActiveSwiper.GetComponentInChildren<TouchSwiper>();
        float normalizedPosition = touchSwiper.scrollRect.normalizedPosition.x;
        ChangeBtnColor(normalizedPosition);
    }



    public void Left()
    {
        touchSwiper.SwipeToLeft();
    }

    public void Right()
    {
        touchSwiper.SwipeToRight();
    }

    //改变箭头按钮颜色
    public void ChangeBtnColor(float index)
    {
       //判断内容有几页
      int num =  touchSwiper.ContentTransform.childCount / 6; 
        if(Mathf.Abs(index - 0) < 0.05 && num == 1)
        {
            Lefts[0].enabled = true;
            Lefts[1].enabled = false;
            Rights[0].enabled = true;
            Rights[1].enabled = false;
        }
        else if (Mathf.Abs(index - 0)<0.05)
        {
            Lefts[0].enabled = true;
            Lefts[1].enabled = false;
            Rights[0].enabled = false;
            Rights[1].enabled = true;
        }else if(Mathf.Abs(index - 1) < 0.05) 
        {
            Lefts[0].enabled = false;
            Lefts[1].enabled = true;
            Rights[0].enabled = true;
            Rights[1].enabled = false;
        }
        else if(index > 0 || index < 1)
        {
            Lefts[0].enabled = false;
            Lefts[1].enabled = true;
            Rights[0].enabled = false;
            Rights[1].enabled = true;
        }
    }


}
