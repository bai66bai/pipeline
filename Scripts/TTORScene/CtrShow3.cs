using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class CtrShow3 : MonoBehaviour
{

    public List<ScrollRect> scrollRects; 

    private void Start()
    {
        StartCoroutine(RunScroll());
    }


    private void OnEnable()
    {
        StartCoroutine(RunScroll());
    }

    IEnumerator RunScroll()
    {

        for (int i = 0; i < scrollRects.Count; i++)
        {
           TtorCtrScroll ttorCtrScroll = scrollRects[i].GetComponent<TtorCtrScroll>();
            ttorCtrScroll.PublicScroll();
            yield return new WaitForSeconds(0.1f);
        }
       
    }
}
