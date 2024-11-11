using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CtrlDirection : MonoBehaviour
{
    private Image[] Lefts;
    private Image[] Rights;
    public List<Image> ContentList;
    public TCPClient client;
    public List<GameObject> Btns;

    private void Start()
    {
        Lefts = Btns[0].GetComponentsInChildren<Image>();
        Rights = Btns[1].GetComponentsInChildren<Image>();
    }

    public void ClickLeftBtn()
    {
        client.SendMessage($"changeThesis:left-{TTORStore.IP}");
        ContentList[0].enabled = true;
        ContentList[1].enabled = false;
       
        Lefts[1].enabled = false;
        Lefts[2].enabled = true;
        Rights[1].enabled = true;
        Rights[2].enabled = false;
    }



    public void ClickRightBtn()
    {
        client.SendMessage($"changeThesis:right-{TTORStore.IP}");
        ContentList[0].enabled = false;
        ContentList[1].enabled = true;
        Lefts[1].enabled = true;
        Lefts[2].enabled = false;
        Rights[1].enabled = false;
        Rights[2].enabled = true;
    }
}
