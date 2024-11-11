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
        // 启动服务器监听协程
       
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
                // 有客户端尝试连接
                var clientTask = tcpListener.AcceptTcpClientAsync(); // 异步接受客户端连接
                while (!clientTask.IsCompleted) // 等待连接完成
                    yield return null;

                TcpClient tcpClient = clientTask.Result;

                // 打断Idle
                //IdleCtrl.BreakAndReset();
                // 处理客户端请求               
                StartCoroutine(HandleClient(tcpClient));              
            }
            yield return null; // 避免阻塞
        }
    }

    IEnumerator HandleClient(TcpClient tcpClient)
    {
        NetworkStream stream = tcpClient.GetStream();
        byte[] buffer = new byte[1024];
        var readTask = stream.ReadAsync(buffer, 0, buffer.Length); // 异步读取数据
        while (!readTask.IsCompleted) // 等待读取完成
            yield return null;

        string request = Encoding.UTF8.GetString(buffer, 0, readTask.Result);
        Debug.Log("Received: " + request);
        if(request != "break")
        {
        tcpMsgHandler.OnMsg(request);
        }

        // 发送响应
/*        string response = "Success";
        byte[] responseData = Encoding.UTF8.GetBytes(response);
        var writeTask = stream.WriteAsync(responseData, 0, responseData.Length); // 异步发送数据
        while (!writeTask.IsCompleted) // 等待发送完成
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
