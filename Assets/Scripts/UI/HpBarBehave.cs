using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarBehave : MonoBehaviour
{
    /// <summary>
    /// 先生成所有的玩家，再生成所对应的摄像机，摄像机生成一个hp bar(由Camera指定Player)
    /// </summary>
    public GameObject player;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 hpPosition = cam.WorldToScreenPoint(player.transform.Find("hp").position);
        transform.position = hpPosition;

    }
}
