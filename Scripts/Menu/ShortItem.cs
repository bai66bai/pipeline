using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShortItem : MonoBehaviour
{
  public LevelLoader LevelLoader;
    private TextMeshProUGUI textName;

    private void Start()
    {
        textName = transform.Find("TextName").gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void LoadDetailScene()
    {
        DetailStore.ActiveDetailText = textName.text;
        LevelLoader.LoadNewScene("DetailScene");
    }
}
