using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneControl : MonoBehaviour {
    public GameObject WhiteCamera;
    public GameObject BlackCamera;

	// Use this for initialization
	void Start () {
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
            Debug.Log("server Camera");
	}

}
