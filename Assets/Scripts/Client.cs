using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{

    public string clientName;


    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    private List<GameClient> players = new List<GameClient>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public bool ConnectToServer(string host,int port)
    {
        if (socketReady)
            return false;
        try
        {
            //socket = new TcpClient(host, port);
            //stream = socket.GetStream();
            //writer = new StreamWriter(stream);
            //reader = new StreamReader(stream);

            socket = new TcpClient();
            socket.BeginConnect(host, port, new AsyncCallback(ConnectCallBack), socket);
            //socketReady = true;
        }
        catch(Exception e)
        {
            Debug.Log("Socket error" + e.Message);
        }

        return socketReady;
    }

    private void ConnectCallBack(IAsyncResult ar)
    {
        TcpClient t = (TcpClient)ar.AsyncState;
        try
        {
            if(t.Connected)//异步连接成功的标志
            {
                t.EndConnect(ar);
                stream = socket.GetStream();
                writer = new StreamWriter(stream);
                reader = new StreamReader(stream);
                socketReady = true;        
            }
            else
            {
                Debug.Log("client not connected");//we can set the delay 
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void Update()
    {
        if(socketReady)
        {
            if(stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if(data!=null)
                {
                    OnIncomingData(data);
                }
            }
        }
    }
    public void Send(string data)
    {
        if (!socketReady)
            return;

        writer.WriteLine(data);
        writer.Flush();
    }

    private void OnIncomingData(string data)
    {
        Debug.Log("Client has received a message: " + data);
        string[] aData = data.Split('|');

        switch(aData[0])
        {
            case "SWHO":
            for(int i=1;i<aData.Length-1;i++)
                {
                    UserConnected(aData[i]);//why it's not a a host
                }
                Send("CWHO|" + clientName);
                break;
            case "SCNN":
                UserConnected(aData[1]);
                break;
            case "SNUM":
                int num;
                int.TryParse(aData[1], out num);
                _GameManager.Instance.PlayerIndex = num;
                if(num==2)
                    _GameManager.Instance.PlayerIndex = num;
                break;
            case "SRUN":
                if (clientName == aData[1])
                {
                    Debug.Log("你自己刚走了一步，那不要再走了");
                    _GameManager.Instance.isMyturn = false;
                }
                else
                    _GameManager.Instance.ItsYourTurn();      
                break;
        }
    }

    private void UserConnected(string name)
    {
        GameClient c = new GameClient
        {
            name = name
        };
        players.Add(c);
        Debug.Log("playerNum" + players.Count);
        if (players.Count == 2)
            _GameManager.Instance.StartGame();
    }

    void CloseSocket()
    {
        if (!socketReady)
            return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }
}

public class GameClient
{
    public string name;
    public bool isHost;
}
