using System.Net.Sockets;
using System.Text;
using UnityEngine;

public static class GlobalUdpClient
{
    private static string RemoteIpStatic = "192.168.3.219";
    private static readonly int RemotePortStatic = 8051;
    private static readonly UdpClient client = new();

    /// <summary>
    /// UDP客户端发送消息
    /// </summary>
    /// <param name="message">消息字符串</param>
    public static void SendData(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        client.Send(data, data.Length, RemoteIpStatic, RemotePortStatic);
        Debug.Log("Data sent to " + RemoteIpStatic + ":" + RemotePortStatic);
    }
}