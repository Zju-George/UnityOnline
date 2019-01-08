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
    private GameObject PlayerNumber;
    private InputField IpInput=null;
    private InputField PortInput=null;

    public int myOnlineCount;//need to be synchronized
    public string side = null;

    public bool duringPing=false;
    bool isStartGame = false;

    private void Start()
    {
        Screen.SetResolution(1920, 1080, false);
        Instance = this;
        myOnlineCount = 0;
        DontDestroyOnLoad(gameObject);
    }
    public void OnClickConnect()
    {
        string hostAddress = "192.168.0.107";//192.168.0.107，115.195.87.182
        int port=6321;
        IpInput = GameObject.Find("Canvas").transform.Find("IpInput").GetComponent<InputField>();
        PortInput = GameObject.Find("Canvas").transform.Find("PortInput").GetComponent<InputField>();
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
            {
                GameObject.Find("Canvas").transform.Find("Modal Dialog").gameObject.SetActive(true);
                return;
            }
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = "client"+UnityEngine.Random.Range(0,100).ToString();//暂时先用随机数区分            
            c.ConnectToServer(hostAddress, port);
            
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void OnClickHost()
    {
        if (GameObject.FindObjectOfType<Server>())
            return;
        duringPing = true;
        Invoke("SetPingFalse", 0.2f);
        try
        {
            TcpClient c = new Client.TcpClientWithTimeout("192.168.0.107", 6321, 10).Connect();//111.230.56.102,192.168.0.107
            if (c!=null)
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
        }
    }
    public void SetPingFalse()
    {
        duringPing = false;
    }
    private void Update()
    {
        PlayerNumber = GameObject.Find("Canvas").transform.Find("PlayerNumberText").gameObject;
        PlayerNumber.GetComponent<Text>().text = "PlayerNumber: " + myOnlineCount.ToString();
        //Debug.Log(isMyturn);

        if (myOnlineCount == 2 && !duringPing && !isStartGame)
        {
            if(!GameObject.FindObjectOfType<Server>())
                StartChoose();//client 进入选单
        }
    }

    public void StartChoose()
    {
        isStartGame = true;
        //choose if I am white or black
        GameObject.Find("Canvas").transform.Find("Modal Dialog").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Modal Dialog").Find("ConfirmButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Modal Dialog").Find("ChooseSide").gameObject.SetActive(true);
        //SceneManager.LoadScene("Game");
    }
    public void OnChooseSide()
    {
        GameObject.Find("Canvas").transform.Find("Modal Dialog").Find("ChooseSide").gameObject.SetActive(false);
        StartCoroutine(CountDown());
    }
    IEnumerator CountDown()
    {
        GameObject.Find("Canvas").transform.Find("Modal Dialog").Find("CountDownText").gameObject.SetActive(true);
        Text countdown = GameObject.Find("Canvas").transform.Find("Modal Dialog").Find("CountDownText").GetComponent<Text>();
        countdown.text = "You get " + side + " side!";
        yield return new WaitForSeconds(1f);
        countdown.text = "Game will Start in...";
        yield return new WaitForSeconds(0.5f);
        countdown.fontSize = 200;
        countdown.text = "3";
        yield return new WaitForSeconds(1f);
        countdown.text = "2";
        yield return new WaitForSeconds(1f);
        countdown.text = "1";
        yield return new WaitForSeconds(1f);
        if (!GameObject.FindObjectOfType<Server>())
            SceneManager.LoadScene("Game");
        else
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("Game");
        }
    }

}
