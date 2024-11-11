using System.Net.Sockets;
using System.Text;
using System;
using UnityEngine;
using System.Collections;

public class TCPClient : MonoBehaviour
{
    // 配置服务器IP和端口
    private static string serverIp = "192.168.3.18";
    public int serverPort = 8686;

    // 封装发送请求的方法
    public new void SendMessage(string message)
    {
        StartCoroutine(SendRequestCoroutine(message));
    }

    private IEnumerator SendRequestCoroutine(string message)
    {

        // 创建TCP客户端并连接到服务器
        TcpClient client = new();
        var connectTask = client.ConnectAsync(serverIp, serverPort);

            // 等待连接完成
            while (!connectTask.IsCompleted)
            {
                yield return null;
            }

        
            if (connectTask.IsFaulted)
            {
                throw connectTask.Exception;
            }

            NetworkStream stream = client.GetStream();

            // 将消息转换为字节数组
            byte[] data = Encoding.UTF8.GetBytes(message);

            // 发送消息
            var writeTask = stream.WriteAsync(data, 0, data.Length);

            // 等待发送完成
            while (!writeTask.IsCompleted)
            {
                yield return null;
            }
        try
        {
            if (writeTask.IsFaulted)
            {
                throw writeTask.Exception;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception: " + e.Message);
        }
        finally
        {
            // 关闭连接
            client?.Close();
        }
    }
}
