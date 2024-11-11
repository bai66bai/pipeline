using TMPro;
using UnityEngine;

public class MenuItem : MonoBehaviour
{
    public GameObject detailObj;
    public string Text { get => textMeshPro.text; }



    //private Image image;
    private TextMeshProUGUI textMeshPro;

    private static readonly Color inactivateColor = new(153f / 255f, 153f / 255f, 153f / 255f, 1f);

    void Awake()
    {
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ActivateItem()
    {
        textMeshPro.color = Color.white;
        detailObj.SetActive(true);

    }

    public void InactivateItem()
    {
        textMeshPro.color = inactivateColor;
        detailObj.SetActive(false);
    }

}
