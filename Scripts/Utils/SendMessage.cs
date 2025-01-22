using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMessage : MonoBehaviour
{
    public TCPClient client;

    public void DisableScreen()
    {
        client.SendTTorMessage($"centerScreen:close");
    }
}
