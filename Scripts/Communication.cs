using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Communication : MonoBehaviour
{
    private static int num = 0;

    public void AddOne()
    {
        GlobalUdpClient.SendData(num.ToString());
        Debug.Log(num.ToString());
        num++;
        
    }
}
