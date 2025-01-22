using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AllTcpHandler : TCPMsgHandler
{
    public SceneIdle sceneIdle;

    public GameObject ScreenMark;
    public override void HandleMsg(string msg)
    {
        string[] splitedMsg = msg.Split(":");
        switch (splitedMsg[0])
        {
            case "stop":
                if (SceneManager.GetActiveScene().name != "ProductScene")
                {
                    sceneIdle.enabled = false;
                }
                ScreenMark.SetActive(true);
                break;
            case "start":
                ScreenMark.SetActive(false);
                if (SceneManager.GetActiveScene().name != "ProductScene")
                {
                    sceneIdle.enabled = true;
                }
                break;
        }
    }

}

