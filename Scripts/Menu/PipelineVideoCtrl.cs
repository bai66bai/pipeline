using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PipelineVideoCtrl : MonoBehaviour
{
    public TCPClient client;
    public VideoPlayer videoPlayer; // ��Ƶ���������
    private bool isPlaying = false; // ��ǰ����״̬
    private GameObject pause;

    void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }
        pause = GameObject.Find("Pause");
        videoPlayer.loopPointReached += (vp) =>
        {
            videoPlayer.time = 0;
            PauseBtn(true);
            isPlaying = false;
        };
        // ȷ����Ƶһ��ʼ����ͣ��
        videoPlayer.Pause();
        isPlaying = false;

    }
    /// <summary>
    /// ������Ƶ��ͣ����
    /// </summary>
    public void TogglePlayPause()
    {
        if (isPlaying)
        {
            client.SendMessage($"touchScreen:togglePlay");
            videoPlayer.Pause();
        }
        else
        {
            client.SendMessage($"touchScreen:togglePlay");
            videoPlayer.Play();
        }
        isPlaying = !isPlaying;

        PauseBtn(!isPlaying);
    }

    //������ͣͼ����ʾ
    private void PauseBtn(bool state)
    {
        pause.SetActive(state);
    }

    public void CtrlStopVideo()
    {
        videoPlayer.Pause();
    }
}