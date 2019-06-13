using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSceneControl : MonoBehaviour {
    public GameObject WhiteCamera;
    public GameObject BlackCamera;
    public static int TurnNumber;
    public int EventToRegCount = 0;
    public int EventToRegNumber = 1;
    public string[] EventRegInfos;
#if SERVER
    
#endif
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
            WhiteCamera.SetActive(false);
            BlackCamera.SetActive(false);
            if (GameObject.FindObjectOfType<Server>() != null)
            {
                GameObject.FindObjectOfType<Server>().UpdateTurn();
            }
        }         
	}
    public void OnHandleTurnMessage(string turndata)
    {
        if (EventToRegCount > 0)
        {
            Debug.LogError("还有Event没执行完，新的就到了");
            return;
        }
        EventRegInfos = turndata.Split('&');
        EventToRegCount = EventRegInfos.Length;
        Debug.Log("FirstRegistar");
        
        //开始注册
        if (EventToRegCount>=1)
        {
        FirstRegistar();
        }
        else
        {
            //? 所有人都是被动防守的状态, Need to be handled

        }
        GameObject.Find("Canvas").transform.Find("TurnCountDown").gameObject.SetActive(false);
    }
    //? 依次去注册事件，触发事件，
    //? 最后一个Attack事件结束后发信息给server播放完毕，清空EventRegInfos,重置EventToRegCount为0，重置EcentToRegNumber为1,更新TurnNumber
    //? 根据EventToRegNumber 找到字符串，填充攻击对象，填充伤害数值，注册第 EventToRegNumber 的事件
    public void RegistarEvent(object source, EventArgs e)
    {
        if (EventToRegNumber + 1 > EventToRegCount)
        {
            Debug.LogError("没有那么多Event需要注册");
            return;
        }
        string eventRegInfo = EventRegInfos[EventToRegNumber - 1];
        string[] infos = eventRegInfo.Split('|');
        PlayerBehavior LastsenderPlayer = GameObject.Find(infos[1]).GetComponent<PlayerBehavior>();
        LastsenderPlayer.RemoveEventHandler("AttackFinished");
        LastsenderPlayer.RemoveEventHandler("CauseDamage");
        LastsenderPlayer.Desitinations.Clear();
        LastsenderPlayer.Damages.Clear();

        EventToRegNumber += 1;

        eventRegInfo = EventRegInfos[EventToRegNumber - 1];
        infos = eventRegInfo.Split('|');
        PlayerBehavior senderPlayer = GameObject.Find(infos[1]).GetComponent<PlayerBehavior>();
        if (infos[2] == "Attack")
        {
            //Attack 就只有一个攻击目标,设置一个攻击目标
            senderPlayer.Desitinations = new List<GameObject> { GameObject.Find(infos[3]) };
            //设置攻击伤害(内联变量声明)
            int.TryParse(infos[4], out int damage);
            senderPlayer.Damages = new List<int> { damage };
            if (senderPlayer.gameObject.name == "White")
            {
                int.TryParse(infos[5], out int damage2);
                senderPlayer.Damages.Add(damage2);
            }
            senderPlayer.CauseDamage += senderPlayer.Desitinations[0].GetComponent<PlayerBehavior>().OnDamaged;
        }
        if(EventToRegNumber<EventToRegCount)
        {
            senderPlayer.AttackFinished += RegistarEvent;
            Debug.Log("AttackFinished注册了下一轮注册");
            senderPlayer.OnAttack();
        }
        else if(EventToRegNumber==EventToRegCount)
        {
            Debug.Log("事件注册完了");
            senderPlayer.AttackFinished += LastRegistarHandler;
            senderPlayer.OnAttack();
        }
        else
        {
            Debug.LogError("注册越界了,重大错误，可能是善后做的不对");
        }
    }
    
    private void FirstRegistar()
    {
        //不需要清理上一个人的event
        if(EventToRegNumber!=1)
        {
            Debug.LogError("不是第一个注册的");
            return;
        }
        string eventRegInfo = EventRegInfos[EventToRegNumber-1];
        string[] infos = eventRegInfo.Split('|');
        PlayerBehavior senderPlayer = GameObject.Find(infos[1]).GetComponent<PlayerBehavior>();
        if(infos[2]=="Attack")
        {
            //Attack 就只有一个攻击目标,设置一个攻击目标
            senderPlayer.Desitinations = new List<GameObject> { GameObject.Find(infos[3]) };
            //设置攻击伤害(内联变量声明)
            int.TryParse(infos[4], out int damage);
            senderPlayer.Damages = new List<int> { damage };
            if(senderPlayer.gameObject.name=="White")
            {
                int.TryParse(infos[5], out int damage2);
                senderPlayer.Damages.Add(damage2);
            }
            senderPlayer.CauseDamage += senderPlayer.Desitinations[0].GetComponent<PlayerBehavior>().OnDamaged;
        }
        if (EventToRegCount == 1)
        {
            Debug.Log("只有一个事件需要注册，这个同时又是lastRegistar");
            senderPlayer.AttackFinished += LastRegistarHandler;
        }
        else
        {
            senderPlayer.AttackFinished += RegistarEvent;
            Debug.Log("AttackFinished注册了下一轮注册");
        }
        senderPlayer.OnAttack();
        
    }
    //? 善后
    public void LastRegistarHandler(object source, EventArgs e)
    {
        Debug.Log("开始善后..");
        string eventRegInfo = EventRegInfos[EventToRegCount - 1];
        string[] infos = eventRegInfo.Split('|');
        PlayerBehavior EndsenderPlayer = GameObject.Find(infos[1]).GetComponent<PlayerBehavior>();
        EndsenderPlayer.RemoveEventHandler("AttackFinished");
        EndsenderPlayer.RemoveEventHandler("CauseDamage");
        EndsenderPlayer.Desitinations.Clear();
        EndsenderPlayer.Damages.Clear();

        EventRegInfos = null;
        EventToRegCount = 0;
        EventToRegNumber = 1;

        FindObjectOfType<Client>().Send("CFinish|" + TurnNumber.ToString() + "|" + _GameManager.Instance.side);
        TurnNumber += 1;
    }
}
