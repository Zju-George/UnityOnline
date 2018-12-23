using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.IO;

//? 待解决： 让client 自己输入姓名时需要保证不重复，因为我们后面信息同步不是根据id而是根据 姓名

public class Server : MonoBehaviour
{

    public int port = 6321;

    List<ServerClient> clients;
    List<ServerClient> disconnectList;
    List<ServerClient> WhiteList;

    private TcpListener server;
    private bool serverStarted;
    private bool isGetChooseMessage = false; //? 目前就2个人，所以一方选了就直接设置为 true 。实际应该是选一方的人满，其他人就不能选择了


    private void Awake()
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();
        WhiteList = new List<ServerClient>();
        //InvokeRepeating("SychronizePlayer", 0f, 0.1f);
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

        for (int i = 0; i < disconnectList.Count; i++)
        {
            //Tell our player somebody has disconnected

            clients.Remove(disconnectList[i]);
            //ServerClient sc = disconnectList[i];
            //sc.tcp.Close();//不然一直发消息
            disconnectList.RemoveAt(i);

        }

        //_GameManager.Instance.myOnlineCount = clients.Count;
        SynchronizePlayer();
    }

    /*            synchronize online player        */
    public void SynchronizePlayer()
    {
        if (!serverStarted)
            return;
        int OnlineNumber = 0;
        foreach(ServerClient sc in clients)
        {
            if (sc.clientName != null)
                OnlineNumber += 1; 
        }
        BroadCast("SNUM|" + OnlineNumber.ToString(), clients);//同步在线人数
    }
    /*                      */

    public void Init()
    {

        DontDestroyOnLoad(gameObject);
        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            serverStarted = true;
            StartListening();

            //开启同步
            InvokeRepeating("SynchronizePlayer", 1f, 1f);
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
        }
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
        //Debug.Log("Server has received a message : " + data);
        string[] aData = data.Split('|'); 

        switch (aData[0])
        {
            case "CWHO":
                foreach(ServerClient sc in clients)
                {
                    if (sc.id == aData[1])
                    {
                        sc.clientName = aData[2];
                        Debug.Log("设置sc.clientName: " + aData[2]);
                    }
                }
                break;
            case "CRUN":
                data = data.Replace('C', 'S');
                BroadCast(data, clients);
                break;
            case "CChoose":
                if (isGetChooseMessage)
                    return;
                isGetChooseMessage = true;
                string name = aData[2];
                string side = aData[1];
                if(GetServerClient(name)!=null)
                {
                    if (side == "White")
                        WhiteList.Add(GetServerClient(name));
                    Debug.Log(GetServerClient(name).clientName + " choose " + side);
                    BroadCast("SChoose|" + side + "|" + name,clients);
                }     
                break;
        }

    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);

    }

    private void AcceptTcpClient(IAsyncResult ar)
    {
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

        string UniqueId = System.Guid.NewGuid().ToString();
        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ar),UniqueId);
        clients.Add(sc);

        BroadCast("SWHO|" + UniqueId, sc);//clients[clients.Count -1]可以替换为sc
        //BroadCast("SNUM|" + clients.Count.ToString(), clients);//同步目前连着的人的总数
        SynchronizePlayer();
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

    private ServerClient GetServerClient(string name)
    {
        ServerClient result = null;
        bool isFind = false;
        foreach(ServerClient sc in clients)
        {
            if(sc.clientName==name)
            {
                isFind = true;
                result = sc;
            }
        }
        if(!isFind)
        {
            Debug.LogError("ServerClient " + name + " not EXIST!");
        }
        return result;
    }
}

public class ServerClient
{
    public string clientName=null;
    public TcpClient tcp;

    public string id;
    public ServerClient(TcpClient tcp,string id)
    {
        this.tcp = tcp;
        this.id = id;
    }
}