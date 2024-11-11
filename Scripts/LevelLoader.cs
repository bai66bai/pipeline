using System;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    public Animator animator;

    public float transitionTime = 1f;

    public Action onLoadLevel;

    private TCPClient client;

    private void Start()
    {
        client = GetComponent<TCPClient>();
    }


    public void LoadNewScene(string sceneName, bool shouldSend = true)
    {
        StartCoroutine(LoadLevel(sceneName, shouldSend));
    }
    public void LoadNewScene(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName, true));
    }

    public void GoBack()
    {
        LoadNewScene(LevelStore.LastSceneName);
    }

    IEnumerator LoadLevel(string sceneName, bool shouldSend)
    {
        if(shouldSend) //&& sceneName != "MenuScene"
            client.SendMessage($"loadScene:{sceneName}");

        // 播放动画
        animator.SetTrigger("StartTrigger");

        // 等待动画播放完成
        yield return new WaitForSeconds(transitionTime);

        LevelStore.LastSceneName = SceneManager.GetActiveScene().name;

        // 切换场景
        SceneManager.LoadScene(sceneName);
    }
}
