using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrModelOutLine : MonoBehaviour
{
    public List<GameObject> Cells;
    public List<GameObject> Skeletons;
    public List<GameObject> Livers;
    public List<GameObject> Eyeballs;

    public void OutLineActive(GameObject m)
    {

        if(m.name == "LiverBtn")
        {
            ActiveForEach(Livers, true);
            ActiveForEach(Cells, false);
            ActiveForEach(Skeletons, false);
            ActiveForEach(Eyeballs, false);
        }else if(m.name == "CellBtn")
        {
            ActiveForEach(Cells, true);
            ActiveForEach(Skeletons, false);
            ActiveForEach(Livers, false);
            ActiveForEach(Eyeballs, false);
        }else if(m.name == "SkeletonBtn")
        {
            ActiveForEach(Skeletons, true);
            ActiveForEach(Cells, false);
            ActiveForEach(Livers, false);
            ActiveForEach(Eyeballs, false);
        }
        else
        {
            ActiveForEach(Eyeballs, true);
            ActiveForEach(Cells, false);
            ActiveForEach(Livers, false);
            ActiveForEach(Skeletons, false);
        }
    }
    /// <summary>
    /// ������
    /// </summary>
    /// <param name="list">����������б�</param>
    /// <param name="IsActive">�жϼӲ���������</param>
    public void ActiveForEach(List<GameObject> list,bool IsActive)
    {
        list.ForEach(p =>
        {
            MeshRenderer objRenderer = p.GetComponent<MeshRenderer>();
            objRenderer.material.SetFloat("_IsActive", IsActive ? 1 : 0);
        });
    }
}
