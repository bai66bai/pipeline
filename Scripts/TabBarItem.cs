using TMPro;
using UnityEngine;

public class TabBarItem : MonoBehaviour
{
    // 父级对象的TabBar控制器
    private TabBarCtrl tabBarCtrl;
    private TextMeshProUGUI textMeshProUGUI;

    void Start()
    {
        tabBarCtrl = transform.parent.gameObject.GetComponent<TabBarCtrl>();
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // 检测点击事件，调用父级处理点击方法
    public void ItemHandleClick()
    {
        tabBarCtrl.HandleClick(textMeshProUGUI.text);
    }
}