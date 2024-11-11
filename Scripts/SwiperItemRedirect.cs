using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

public class SwiperItemRedirect : MonoBehaviour, IPointerClickHandler
{

    public LevelLoader LevelLoader;
    private TextMeshProUGUI textName;

    // Start is called before the first frame update
    void Start()
    {
        textName = transform.Find("TextName").gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void LoadDetail()
    {
        string targetItemName = textName.text
                                        .Replace("（", "")
                                        .Replace("）", "");
        MedicineDict.dict.TryGetValue(targetItemName, out string v);

        DetailStore.ActiveDetailText = v ?? targetItemName;

        LevelLoader.LoadNewScene("DetailScene");
    }

    /// <summary>
    /// 点击事件回调
    /// </summary>
    /// <param name="eventData">事件参数</param>
    public void OnPointerClick(PointerEventData eventData) => LoadDetail();
}
