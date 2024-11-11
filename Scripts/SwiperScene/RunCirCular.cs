using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunCirCular : MonoBehaviour
{
    public List<Image> images;
    public int RunNum;
    public float interval;
    public int index;

    public void LoadColor() => StartCoroutine(ChangeImageActive());

    IEnumerator ChangeImageActive()
    {
        yield return null;
        for (int i = 0; i < RunNum; i++)
        {
            images[i].enabled = true;
            yield return new WaitForSeconds(interval);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < RunNum; i++)
        {
            images[i].enabled = true;
        }
    }
}