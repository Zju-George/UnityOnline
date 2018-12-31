using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour {

    private void OnEnable()
    {
        transform.SetAsLastSibling();
    }

    public void OnClick()
    {
        gameObject.SetActive(false);
    }

    public void OnClickBlack()
    {
        Client c = GameObject.FindObjectOfType<Client>();
        if (!c)
        {
            Debug.LogError("没有client");
            return;
        }
        string name = c.clientName;
        c.Send("CChoose|Black|" + name);

    }
    public void OnClickWhite()
    {
        Client c = GameObject.FindObjectOfType<Client>();
        if (!c)
        {
            Debug.LogError("没有client");
            return;
        }
        string name = c.clientName;
        c.Send("CChoose|White|" + name);

    }

}
