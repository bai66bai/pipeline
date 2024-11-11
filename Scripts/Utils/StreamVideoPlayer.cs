using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class StreamVideoPlayer : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public string VideoName;

    private void Start()
    {
        if(VideoPlayer != null)
        {
            Debug.Log(VideoName);
            VideoFile(VideoName,  VideoPlayer);
        }
    }

    public void VideoFile(string videoName , VideoPlayer videoPlayer)
    {
        StartCoroutine(ReadFile(videoName,  videoPlayer));
    }


    IEnumerator ReadFile(string videoName, VideoPlayer videoPlayer)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, videoName);

        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        { if(videoPlayer != null)
            {
                videoPlayer.url = filePath;
                VideoPlayer.Prepare();
            }
            
        }
        else
        {
            Debug.LogError("º”‘ÿ ß∞‹£∫ " + www.error);
        }
    }
    private void OnDestroy()
    {
        VideoPlayer.targetTexture.Release();
    }
}
