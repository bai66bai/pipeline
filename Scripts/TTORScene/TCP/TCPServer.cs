using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TCPServer : MonoBehaviour
{
    public static TCPServer Instance { get; private set; }

    public int port = 8686;
    private static TcpListener tcpListener;
    private bool isServerRunning = true;
    private TCPMsgHandler tcpMsgHandler;

    void Start()
    {
        // ��������������Э��
       
        tcpMsgHandler = GetComponent<TCPMsgHandler>();
        StartCoroutine(ServerSetup());
    }

     IEnumerator ServerSetup()
    {
        if (tcpListener == null)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            Debug.Log($"Server started on port {port}.");
        }

        while (isServerRunning)
        {
            if (tcpListener.Pending())
            {
                // �пͻ��˳�������
                var clientTask = tcpListener.AcceptTcpClientAsync(); // �첽���ܿͻ�������
                while (!clientTask.IsCompleted) // �ȴ��������
                    yield return null;

                TcpClient tcpClient = clientTask.Result;

                // ���Idle
                //IdleCtrl.BreakAndReset();
                // ����ͻ�������               
                StartCoroutine(HandleClient(tcpClient));              
            }
            yield return null; // ��������
        }
    }

    IEnumerator HandleClient(TcpClient tcpClient)
    {
        NetworkStream stream = tcpClient.GetStream();
        byte[] buffer = new byte[1024];
        var readTask = stream.ReadAsync(buffer, 0, buffer.Length); // �첽��ȡ����
        while (!readTask.IsCompleted) // �ȴ���ȡ���
            yield return null;

        string request = Encoding.UTF8.GetString(buffer, 0, readTask.Result);
        Debug.Log("Received: " + request);
        if(request != "break")
        {
        tcpMsgHandler.OnMsg(request);
        }

        // ������Ӧ
/*        string response = "Success";
        byte[] responseData = Encoding.UTF8.GetBytes(response);
        var writeTask = stream.WriteAsync(responseData, 0, responseData.Length); // �첽��������
        while (!writeTask.IsCompleted) // �ȴ��������
            yield return null;*/

        stream.Close();
        tcpClient.Close();
    }

    void OnApplicationQuit()
    {
        isServerRunning = false;
        tcpListener.Stop();
        Debug.Log("Server shutdown.");
    }
}
