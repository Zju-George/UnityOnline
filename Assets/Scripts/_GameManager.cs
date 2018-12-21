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
    public int myOnlineCount;//need to be synchronized
    private GameObject Button=null;

    public bool isMyturn;
    private bool duringPing=false;
    bool isStartGame = false;

    private void Start()
    {
        Instance = this;
        myOnlineCount = 0;
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
            if (GameObject.FindObjectOfType<Client>())
                return;
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
        duringPing = true;
        Invoke("setPingFalse", 1.0f);
        try
        {
            TcpClient c = new Client.TcpClientWithTimeout("192.168.0.107", 6321, 10).Connect();
            if(c!=null)
            {
                c.Close();
                //弹出一个窗口告诉他，已经有server了
                GameObject.Find("Canvas").transform.Find("Modal Dialog").gameObject.SetActive(true);
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
    public void setPingFalse()
    {
        duringPing = false;
    }
    private void Update()
    {
        
        if(PlayerNumber!=null)
            PlayerNumber.GetComponent<Text>().text = "PlayerNumber: " + myOnlineCount.ToString();
        //Debug.Log(isMyturn);
        if (Button == null&&GameObject.Find("Button") != null)
        {
            Debug.Log("find button");
            Button = GameObject.Find("Button");
            Button.GetComponent<Button>().onClick.AddListener(RunMyTurn);
        }
        if (myOnlineCount == 2 && !duringPing && !isStartGame)
        {
            myStartGame();
        }
    }

    public void myStartGame()
    {
        isStartGame = true;
        //choose if I am white or black 
        GameObject.Find("Canvas").transform.Find("Modal Dialog").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Modal Dialog").Find("ConfirmButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Modal Dialog").Find("ChooseSide").gameObject.SetActive(true);
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
