using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    public List<Button> btns = new List<Button>();
    public GameObject verticalLayoutGroup;

    Text t=null;
    IEnumerator coroutine;

    private void OnEnable()
    {
        foreach (Button btn in btns)
        {
            btn.interactable = true;
        }
        verticalLayoutGroup.SetActive(true);
        coroutine = StartCountDown();
        t = GetComponent<Text>();
        StartCoroutine(coroutine);
        //Invoke("SwitchHangOn", 3);
    }
    public void SwitchHangOn()
    {
        StopCoroutine(coroutine);
        t.text = "Wait...";
    }
    IEnumerator StartCountDown()
    {
        //Text t = GetComponent<Text>();
        t.text = "10";
        for (int i = 9; i >= 0; i--)
        {
            yield return new WaitForSeconds(1f);
            t.text = i.ToString();
            if (i == 0)
            {
                TurnTimeOut();
                yield break;
            }
        }

    }
    public void SetInactive()
    {
        verticalLayoutGroup.SetActive(false);
    }
    public void OnPressAttack()
    {
        SwitchHangOn();
        foreach(Button btn in btns)
        {
            btn.interactable = false;
        }
        verticalLayoutGroup.GetComponent<Animator>().SetTrigger("IsChoosed");
        Invoke("SetInactive", 2);
        //? 调用client 里的一个函数给server 发攻击的消息
        if (GameObject.FindObjectOfType<Client>() == null)
        {
            Debug.Log("没有client！！！");
        }
        else
        {
            Client c = GameObject.FindObjectOfType<Client>();
            string oppositeSide;
            if (_GameManager.Instance.side == "White")
                oppositeSide = "Black";
            else
                oppositeSide = "White";
            c.Send("CTurn|" + GameSceneControl.TurnNumber.ToString() + "|"
                + _GameManager.Instance.side + "|" + "Attack|" + oppositeSide);
        }
    }
    public void OnPressDefend()
    {
        SwitchHangOn();
        foreach (Button btn in btns)
        {
            btn.interactable = false;
        }
        verticalLayoutGroup.GetComponent<Animator>().SetTrigger("IsChoosed");
        Invoke("SetInactive", 2);
        //? 调用client 里的一个函数给server 发防守的消息
    }
    void TurnTimeOut()
    {
        //switch(Random.Range(0,2))
        //{
        //    case 0:
        //        OnPressAttack();
        //        break;
        //    case 1:
        //        OnPressDefend();
        //        break;
        //}
        OnPressAttack();
    }

}
