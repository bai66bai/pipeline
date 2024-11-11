using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class IpAddress : MonoBehaviour
{
    void Start()
    {
        string localIP = GetLocalIPAddress();
        TTORStore.IP = localIP;
    }

    string GetLocalIPAddress()
    {
        string localIP = string.Empty;

        foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
        {
            // 过滤掉 IPv6 地址，仅保留 IPv4 地址
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }

        return localIP;
    }
}
