using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.IO;



public class Server : MonoBehaviour
{

    public int port = 6321;

    List<ServerClient> clients;
    List<ServerClient> disconnectList;

    private TcpListener server;
    private bool serverStarted;
    private void Awake()
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();
    }
    public void Init()
    {

        DontDestroyOnLoad(gameObject);
        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            serverStarted = true;
            StartListening();
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
        }
    }
    private void Update()
    {
        if (!serverStarted)
            return;


        foreach (ServerClient c in clients)
        {
            //Is the client still connected?
            if (!IsConnected(c.tcp))
            {
                try
                {
                    c.tcp.Close();
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
                disconnectList.Add(c);
                continue;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if (data != null)
                        OnInComingData(c, data);
                }
            }
        }

        if (disconnectList.Count > 0)
        {
            BroadCast("SNUM|" + clients.Count.ToString(), clients);//同步在线人数

        }
        for (int i = 0; i < disconnectList.Count; i++)
        {
            //Tell our player somebody has disconnected

            clients.Remove(disconnectList[i]);
            //ServerClient sc = disconnectList[i];
            //sc.tcp.Close();//不然一直发消息
            disconnectList.RemoveAt(i);

        }

        _GameManager.Instance.PlayerIndex = clients.Count;

    }
    //Server Write s
    private void BroadCast(string data, List<ServerClient> c1)
    {
        foreach (ServerClient sc in c1)
        {
            if (!IsConnected(sc.tcp))
                return;
            try
            {
                StreamWriter writer = new StreamWriter(sc.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {
                Debug.LogError("Write error: " + e.Message);
            }
        }
    }
    private void BroadCast(string data, ServerClient c)
    {
        List<ServerClient> sc = new List<ServerClient> { c };
        BroadCast(data, sc);
    }
    //Server Read
    private void OnInComingData(ServerClient c, string data)
    {
        Debug.Log("Server has received a message : " + data);
        string[] aData = data.Split('|');

        switch (aData[0])
        {
            case "CWHO":
                c.clientName = aData[1];
                BroadCast("SCNN|" + c.clientName, clients);//Server Broadcast to clients that who has joined the game;
                break;
            case "CRUN":
                data = data.Replace('C', 'S');
                BroadCast(data, clients);
                break;
        }

    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);

    }

    private void AcceptTcpClient(IAsyncResult ar)
    {
        string allUsers = "";
        foreach (ServerClient i in clients)
        {
            allUsers += i.clientName + '|';
        }

        TcpListener listener = (TcpListener)ar.AsyncState;

        if (listener == server)
        {
            //Debug.Log("this listener is the server");
            //Debug.Log("now player's index is " + _GameManager.Instance.PlayerIndex);
        }
        else
        {
            Debug.Log("this Listener is not the server");
        }

        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ar));
        clients.Add(sc);

        BroadCast("SWHO|" + allUsers, clients[clients.Count - 1]);
        BroadCast("SNUM|" + clients.Count.ToString(), clients);//同步目前连着的人的总数
        StartListening();
    }

    private bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }

                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }
}

public class ServerClient
{
    public string clientName;
    public TcpClient tcp;

    public ServerClient(TcpClient tcp)
    {
        this.tcp = tcp;
    }
}