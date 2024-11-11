using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTORTcpHandler : TCPMsgHandler
{

    public List<ScreenCastBtn> screenCastBtns;
    public override void HandleMsg(string msg)
    {
        string[] splitMesg = msg.Split(":");
        if (splitMesg[0] == "screenCasting")
        {
            screenCastBtns.ForEach(t =>
            {
                if (t.gameObject.activeInHierarchy && t.enabled)
                {
                    t.ChangeImage();
                    t.IsScreenCasting = true;
                }
            });
        }
    }
}
