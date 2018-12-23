using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;


public class Client : MonoBehaviour
{
    public class TcpClientWithTimeout
    {
        protected string _hostname;
        protected int _port;
        protected int _timeout_milliseconds;
        protected TcpClient connection;
        protected bool connected;
        protected Exception exception;

        public TcpClientWithTimeout(string hostname, int port, int timeout_milliseconds)
        {
            _hostname = hostname;
            _port = port;
            _timeout_milliseconds = timeout_milliseconds;
        }
        public TcpClient Connect()
        {
            // kick off the thread that tries to connect
            connected = false;
            exception = null;
            Thread thread = new Thread(new ThreadStart(BeginConnect));
            thread.IsBackground = true; // 作为后台线程处理
                                        // 不会占用机器太长的时间
            thread.Start();

            // 等待如下的时间
            thread.Join(_timeout_milliseconds);

            if (connected == true)
            {
                // 如果成功就返回TcpClient对象
                thread.Abort();
                return connection;
            }
            if (exception != null)
            {
                // 如果失败就抛出错误
                thread.Abort();
                throw exception;
            }
            else
            {
                // 同样地抛出错误
                thread.Abort();
                string message = string.Format("TcpClient connection to {0}:{1} timed out",
                  _hostname, _port);
                throw new TimeoutException(message);
            }
        }
        protected void BeginConnect()
        {
            try
            {
                connection = new TcpClient(_hostname, _port);
                // 标记成功，返回调用者
                connected = true;
            }
            catch (Exception ex)
            {
                // 标记失败
                exception = ex;
            }
        }
    }
    public string clientName;
    string id;
    
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    private List<GameClient> players = new List<GameClient>();


    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        InvokeRepeating("Destroy", 0.1f, 1f);
    }


    public bool ConnectToServer(string host, int port)
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
        catch (Exception e)
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
            if (t.Connected)//异步连接成功的标志
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
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void Update()
    {
        //Debug.Log(socket.Connected + clientName);
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
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

        switch (aData[0])
        {
            case "SWHO":
                id = aData[1];
                Send("CWHO|" + id + "|" + clientName);
                break;
            case "SNUM":
                int num;
                int.TryParse(aData[1], out num);
                _GameManager.Instance.myOnlineCount = num;
                break;
            case "SChoose":
                if(clientName==aData[2])
                {
                    _GameManager.Instance.side = aData[1];
                }
                else
                {
                    if (aData[1] == "White")
                        _GameManager.Instance.side = "Black";
                    else
                        _GameManager.Instance.side = "White";
                }
                _GameManager.Instance.OnChooseSide();
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
    public void CloseSocket()
    {
        if (!socketReady)
            return;
        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
        Debug.Log("被正确关闭了");
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    private void OnDisable()
    {
        CloseSocket();
    }

    public void Destroy()
    {
        if(!socketReady)
            GameObject.Destroy(gameObject);
    }
    private void OnDestroy()
    {
        CloseSocket();
    }
}

public class GameClient
{
    public string name;
    public bool isHost;
}
