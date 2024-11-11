using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CtrlBtnImage : MonoBehaviour
{
    public List<Image> BtnImages;

    public void ChangeActive(bool active)
    {
        BtnImages[0].enabled = active;
        BtnImages[1].enabled = !active;
    }
}
