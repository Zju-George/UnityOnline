using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehave : MonoBehaviour
{
    public Transform canvas;
    public GameObject player;
    public GameObject HpBarPrefab;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas").transform;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int strlength = gameObject.name.Length;
        //Debug.Log(strlength);
        string name = gameObject.name.Substring(0, strlength - 6);
        //Debug.Log(name);
        foreach(var player in players)
        {
            if (name == player.name)
                this.player = player;
        }
        SpawnUI();
    }

    private void SpawnUI()
    {
        GameObject hpbar = Instantiate(HpBarPrefab, canvas);
        hpbar.GetComponent<HpBarBehave>().player = this.player;
        hpbar.GetComponent<HpBarBehave>().cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
