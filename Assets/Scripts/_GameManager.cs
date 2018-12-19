using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Net.Sockets;

public class _GameManager : MonoBehaviour {
    public static _GameManager Instance { set; get; }

    public GameObject serverPrefab;
    public GameObject clientPrefab;
    public GameObject PlayerNumber;
    public InputField IpInput;
    public InputField PortInput;
    public int PlayerIndex;//need to be synchronized
    private GameObject Button=null;

    public bool isMyturn;
    private void Start()
    {
        Instance = this;
        PlayerIndex = 0;
        DontDestroyOnLoad(gameObject);
        
    }
    public void OnClickConnect()
    {
        string hostAddress = "192.168.0.107";//192.168.0.107，115.195.87.182
        int port=6321;
        if (IpInput.text != "")
        {
            hostAddress = IpInput.text;
        }
        if(PortInput.text!="")
        {
            int.TryParse(PortInput.text, out port);
        }

            try
        {
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = "client";            
            c.ConnectToServer(hostAddress, port);
            
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void OnClickHost()
    {
        try
        {
            TcpClient c = new Client.TcpClientWithTimeout("192.168.0.107", 6321, 10).Connect();
            if(c!=null)
            {
                c.Close();
                //弹出一个窗口告诉他，已经有server了，问你是否要加入他
                Debug.Log("已经有server了");
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            Server s = Instantiate(serverPrefab).GetComponent<Server>();
            s.Init();

            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = "host";
            c.ConnectToServer("192.168.0.107", 6321);
        }
    }

    private void Update()
    {
        if(PlayerNumber!=null)
            PlayerNumber.GetComponent<Text>().text = "PlayerNumber: " + PlayerIndex.ToString();
        //Debug.Log(isMyturn);
        if (Button == null&&GameObject.Find("Button") != null)
        {
            Debug.Log("find button");
            Button = GameObject.Find("Button");
            Button.GetComponent<Button>().onClick.AddListener(RunMyTurn);
        }

    }

    public void StartGame()
    {
        //SceneManager.LoadScene("Game");
        
        if (GameObject.FindObjectOfType<Server>())
            isMyturn = true;
        else
            isMyturn = false;
    }

    public void RunMyTurn()
    {
            if (isMyturn)
            {
                Client c = GameObject.FindObjectOfType<Client>();
                c.Send("CRUN|"+c.clientName);
                Debug.Log("c.name :" + c.clientName);
                isMyturn = !isMyturn;
                GameObject.Find("ShowTurn").GetComponent<Text>().text = "you end up your turn";
            }
            else
            {
                GameObject.Find("ShowTurn").GetComponent<Text>().text = "it is not your turn";
                return;
            }
    }
    public void ItsYourTurn()
    {
        this.isMyturn = true;
        if (GameObject.Find("ShowTurn"))
            GameObject.Find("ShowTurn").GetComponent<Text>().text = "it is your turn now!";
    }
}
