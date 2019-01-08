using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.IO;

//? 待解决： 让client 自己输入姓名时需要保证不重复，因为我们后面信息同步不是根据id而是根据 姓名，所以需要向server求证姓名是否被用过

public class Server : MonoBehaviour
{

    public string BlackBuffer = "";
    public string WhiteBuffer = "";

    List<ServerClient> clients;
    List<ServerClient> disconnectList;
    List<ServerClient> WhiteList;

    private int port = 6321;
    private TcpListener server;
    private bool serverStarted;
    private bool isGetChooseMessage = false; //? 目前就2个人，所以一方选了就直接设置为 true 。实际应该是选一方的人满，其他人就不能选择了
    private bool isTurnMessageSend = false;
    private float turnTime = 10.5f;

    private void OnApplicationQuit()
    {
        server.Stop();
    }
    private void Awake()
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();
        WhiteList = new List<ServerClient>();
    }
    
    private void Update()
    {
        if (!serverStarted)
            return;
        if(clients.Count==0)
        {
            SynchronizePlayer();
            return;
        }
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
            SynchronizePlayer();
        }

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
        BroadCast("SNum|" + OnlineNumber.ToString(), clients);//同步在线人数
        _GameManager.Instance.myOnlineCount = OnlineNumber;
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
            case "CWho":
                foreach(ServerClient sc in clients)
                {
                    if (sc.id == aData[1])
                    {
                        sc.clientName = aData[2];
                        Debug.Log("设置sc.clientName: " + aData[2]);
                    }
                }
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
                    _GameManager.Instance.OnChooseSide();   
                }     
                break;
            case "CTurn":
                OnHandleTurn(data);
                break;
        }
    }

    private void OnHandleTurn(string data)
    {
        string afterProcess = "&";
        string[] aData = data.Split('|');
        string side = aData[2];
        string type = aData[3];
        if (type == "Attack")
        {
            int damage = UnityEngine.Random.Range(1, 20);
            //! &轮数+攻击方+攻击+被攻击方+数值            
            afterProcess += aData[1] + "|" + aData[2] + "|" + aData[3] + "|" + aData[4] + "|" + damage.ToString();
        }
        if (type == "Defend")
        {
            //nothing happen right now
        }
        if (side == "White")
        {
            WhiteBuffer += afterProcess;     
        }
        if (side == "Black")
        {
            BlackBuffer += afterProcess;
        }
        CheckBuffer();
    }

    private void CheckBuffer()
    {
        if (WhiteBuffer != "" && BlackBuffer != "")
        {
            //整合消息(默认白色行动在前）
            string TurnMessage = "STurn" + "|";
            TurnMessage += WhiteBuffer;
            TurnMessage += BlackBuffer;
            Debug.Log(TurnMessage);
            //发送消息(把buffer置于null)
            BroadCast(TurnMessage, clients);
            isTurnMessageSend = true;
            WhiteBuffer = "";
            BlackBuffer = "";
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

        BroadCast("SWho|" + UniqueId, sc);//clients[clients.Count -1]可以替换为sc
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

    public void UpdateTurn()
    {
        string s = "SUpdate" + "|";
        BroadCast(s,clients);
        Invoke("ForceTurnMessage", turnTime);
    }
    public void ForceTurnMessage()
    {
        if(!isTurnMessageSend)
        {
            if (WhiteBuffer == "")
            {
                Debug.Log("WhiteBuffer为空");
                WhiteBuffer = "&-1|White|Attack|Black|1";
            }
            if (BlackBuffer == "")
            {
                Debug.Log("BlackBuffer为空");
                BlackBuffer = "&-1|Black|Attack|White|1";
            }
            CheckBuffer();
        }
        isTurnMessageSend = false;
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