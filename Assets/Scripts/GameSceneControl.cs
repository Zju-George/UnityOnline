using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneControl : MonoBehaviour {
    public GameObject WhiteCamera;
    public GameObject BlackCamera;
    public static int TurnNumber;
    public int EventToRegCount = 0;
    public int EventToRegNumber = 1;
    public string[] EventRegInfos;

    void Awake () {
        
        if (_GameManager.Instance == null)
            Debug.Log("GamaManager 不存在");
        else if (_GameManager.Instance.side == "White")
        {
            WhiteCamera.SetActive(true);
            BlackCamera.SetActive(false);
        }
        else if (_GameManager.Instance.side == "Black")
        {
            WhiteCamera.SetActive(false);
            BlackCamera.SetActive(true);
        }
        else
        {
            Debug.Log("server Camera");
            WhiteCamera.SetActive(true);
            BlackCamera.SetActive(false);
            if (GameObject.FindObjectOfType<Server>() != null)
            {
                GameObject.FindObjectOfType<Server>().UpdateTurn();
            }
        }         
	}

    //? 依次去注册事件，触发事件，
    //? 最后一个Attack事件结束后发信息给server播放完毕，清空EventRegInfos和EventToRegCount和EcentToRegNumber,更新TurnNumber
    public void RegistarEvents()
    {
        if (EventToRegNumber > EventToRegCount)
            Debug.LogError("没有那么多Event需要注册");
        //? 根据EventToRegNumber 找到字符串，填充攻击对象，填充伤害数值，注册第 EventToRegNumber 的事件

    }
    public void OnHandleTurnMessage(string turndata)
    {
        if (EventToRegCount > 0)
        {
            Debug.LogError("还有Event没执行完，新的就到了");
            return;
        }
        string[] EventRegInfos = turndata.Split('&');
        EventToRegCount = EventRegInfos.Length;
        Debug.Log(EventToRegCount);
        Debug.Log(EventRegInfos[0]);
    }
}
