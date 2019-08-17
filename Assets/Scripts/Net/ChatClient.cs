using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class ChatClient : MonoBehaviour
{
    public string ipaddress = "192.168.174.1";
    public int port = 7799;
    private Socket clientSocket;

    public InputField MessageInput;
    public Text MessageText;

    private Thread thread;
    private byte[] data = new byte[1024];
    private string message = "";

    void ConnectToServer()
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ipaddress), port));
        Thread thread = new Thread(ReceiveMessage);
        thread.Start();
    }

    void ReceiveMessage()
    {
        while(true)
        {
            if(clientSocket.Connected == false)
            {
                break;
            }
            int length = clientSocket.Receive(data);
            message = Encoding.UTF8.GetString(data);
            print(message);
        }
    }

    void ClientSendMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        clientSocket.Send(data);
    }

    public void OnSendButtonClick()
    {
        Debug.Log("Click");
        string value = MessageInput.text;
        ClientSendMessage(value);
        MessageInput.text = "";
    }
    public void OnDestroy()
    {
        clientSocket.Shutdown(SocketShutdown.Both);
        clientSocket.Close();
    }

    void Start()
    {
        ConnectToServer();
    }

    void Update()
    {
        if(message != "" && message != null)
        {
            MessageText.text += "\n" + message;
            message = "";
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnSendButtonClick();
        }
    }
}
