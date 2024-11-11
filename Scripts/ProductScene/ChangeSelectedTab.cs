using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSelectedTab : MonoBehaviour
{
    public LevelLoader LevelLoader;
    public TCPClient client;
    public void TabSelect(int index)
    {
        SwiperStore.SelectedTab = index;
        string sceneName = index == 0 ? "SwiperOncologyScene" : "SwiperPipelineScene";
        LevelLoader.LoadNewScene("SwiperScene", false);
        client.SendMessage($"loadScene:{sceneName}");
    }
}
