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
    public Transform hp;
    // Start is called before the first frame update
    void Start()
    {
        switch(player.name)
        {
            case "Black":
                hp = player.transform.Find
                    ("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head/hp");
                break;
            case "White":
                hp = player.transform.Find("hp");
                break;
        }
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 hpPosition = cam.WorldToScreenPoint(hp.position);
        transform.position = hpPosition;

    }
}
