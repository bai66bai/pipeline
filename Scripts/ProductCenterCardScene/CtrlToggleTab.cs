using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CtrlToggleTab : MonoBehaviour
{
    public List<GameObject> MedicalList;
    public TCPClient client;
    public List<GameObject> BtnList;

    public void ChangeTab(GameObject btn)
    {
        client.SendMessage($"tabName:{btn.name}");
        int index = BtnList.IndexOf(btn);
        ShowMedicalLIst(index);
        ChangeImage(index);
    }
   
    

    private void ChangeImage(int index)
    {
        foreach (var item in BtnList)
        {
            item.transform.GetComponent<CtrlBtnImage>().ChangeActive(BtnList.IndexOf(item) == index);
        }
    }


    //øÿ÷∆œ‘ æƒ⁄»›
    private void ShowMedicalLIst(int index)
    {
        foreach (var item in MedicalList)
        {
            item.SetActive(MedicalList.IndexOf(item) == index);
        }
    }
}
