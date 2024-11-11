using UnityEngine;
using UnityEngine.SceneManagement;

public  class TCPMsgHandler : MonoBehaviour
{
    private LevelLoader levelLoader;
    private void Awake()
    {
        levelLoader = GetComponent<LevelLoader>();
    }

    public virtual void HandleMsg(string msg) { }

    public virtual void OnMsg(string msg) 
    {
        string[] splitMsg = msg.Split(":");
        //if (splitMsg[0] == "loadScene")
        // levelLoader.LoadNewScene(splitMsg[1]);        
        //else
            HandleMsg(msg);
    }
}

