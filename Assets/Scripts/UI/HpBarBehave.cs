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
    private Transform hppos;
    private RectTransform hp;
    void Start()
    {
        hp = transform.Find("hp").GetComponent<RectTransform>();
        switch(player.name)
        {
            case "Black":
                hppos = player.transform.Find
                    ("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Neck/Bip001 Head/hppos");
                break;
            case "White":
                hppos = player.transform.Find("hppos");      
                break;
        }
        Update();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 hpPosition = cam.WorldToScreenPoint(hppos.position);
        transform.position = hpPosition;
        hp.sizeDelta = new Vector2(player.GetComponent<PlayerBehavior>().Hp,hp.sizeDelta.y);
    }
}
