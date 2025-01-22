using System.Net.Sockets;
using System.Text;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TCPClient : MonoBehaviour
{
    // ���÷�����IP�Ͷ˿�
    private static string serverIp = "192.168.5.4";
    public int serverPort = 8686;

    // ��װ��������ķ���
    public new void SendMessage(string message)
    {
        StartCoroutine(SendRequestCoroutine(message));
    }

    private List<string> TTORIPs = new List<string>
    {
        "192.168.5.14",
        "192.168.5.15",
        "192.168.5.16",
        "192.168.5.17"
    };

    public  void SendTTorMessage(string message)
    {
        TTORIPs.ForEach(ip =>
        {
            StartCoroutine(SendTTORRequestCoroutine(message, ip));
        });
    }

    private IEnumerator SendTTORRequestCoroutine(string message ,string ttorIp)
    {

        // ����TCP�ͻ��˲����ӵ�������
        TcpClient client = new();
        var connectTask = client.ConnectAsync(ttorIp, serverPort);

        // �ȴ��������
        while (!connectTask.IsCompleted)
        {
            yield return null;
        }


        if (connectTask.IsFaulted)
        {
            throw connectTask.Exception;
        }

        NetworkStream stream = client.GetStream();

        // ����Ϣת��Ϊ�ֽ�����
        byte[] data = Encoding.UTF8.GetBytes(message);

        // ������Ϣ
        var writeTask = stream.WriteAsync(data, 0, data.Length);

        // �ȴ��������
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
            // �ر�����
            client?.Close();
        }
    }

    private IEnumerator SendRequestCoroutine(string message)
    {

        // ����TCP�ͻ��˲����ӵ�������
        TcpClient client = new();
        var connectTask = client.ConnectAsync(serverIp, serverPort);

            // �ȴ��������
            while (!connectTask.IsCompleted)
            {
                yield return null;
            }

        
            if (connectTask.IsFaulted)
            {
                throw connectTask.Exception;
            }

            NetworkStream stream = client.GetStream();

            // ����Ϣת��Ϊ�ֽ�����
            byte[] data = Encoding.UTF8.GetBytes(message);

            // ������Ϣ
            var writeTask = stream.WriteAsync(data, 0, data.Length);

            // �ȴ��������
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
            // �ر�����
            client?.Close();
        }
    }
}
