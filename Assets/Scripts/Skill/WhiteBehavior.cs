using System;
using System.Collections.Generic;
using UnityEngine;


public class WhiteBehavior : PlayerBehavior
{
    public GameObject firePoint;
    public Transform destination;// 为了指定射击的方向
    public List<GameObject> vfx = new List<GameObject>();

    private GameObject effectToSpawn;
    private bool firstShoot = false;
    private bool secondShoot = false;


    void Start()
    {
        //? 设置Speed
        Speed = 532;
        // 将Speed 转化为三位数的string 输出
        if (Speed > 999)
            Debug.LogError("Speed 超过了999");
        int hundredDig = Speed / 100;
        Debug.Log("百位数是： " + hundredDig);
        int tenDig = (Speed%100) / 10;
        Debug.Log("十位数是： " + tenDig);

        DamageTextPrefab = GameObject.Find("Prefabs").transform.Find("DamageText").gameObject;
        animator = GetComponent<Animator>();
        effectToSpawn = vfx[0];
    }

    private void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("die_back_rest")&& animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5)
        {
            animator.SetFloat("Speed", -1.0f);
            animator.CrossFade("idle_gunMiddle_ar",0.2f);
            return;
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("idle_gunMiddle_ar"))
        {
            animator.SetFloat("Speed", 1.0f);
        }
            
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("shoot_single_ar"))
        {
            
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime>0.3&&!firstShoot)
            {
                firstShoot = true;
                SpawnBullet(1,Damages[0]);
            }
            if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.4&&!secondShoot)
            {
                secondShoot = true;
                SpawnBullet(2,Damages[1]);
            }
        }
        else
        {
            firstShoot = false;
            secondShoot = false;
        }
    }

    public void SpawnBullet(int time,int damage)
    {
        GameObject bullet = Instantiate(effectToSpawn, firePoint.transform.position, Quaternion.identity);
        // 子弹的初始值
        bullet.GetComponent<BulletMove>().Sender = gameObject;
        bullet.GetComponent<BulletMove>().damage = damage;
        bullet.GetComponent<BulletMove>().time = time;

        Vector3 direction = destination.position - firePoint.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        bullet.transform.localRotation = rotation;
    }
    public override void OnDamaged(object source, DamageEventArgs e)
    {
        animator.SetFloat("Speed", 1.5f);
        animator.SetTrigger("Damage");
        //? 产生白色的info text和黑色的info text，其旋转是不一样的
        GameObject blackText = Instantiate(DamageTextPrefab, transform.position+1.5f*Vector3.up, Quaternion.identity);
        blackText.SetActive(true);
        blackText.GetComponent<Transform>().localEulerAngles = new Vector3(0, 180, 0);
        blackText.layer = 10;
        blackText.GetComponent<TextMesh>().text = e.DamageValue.ToString();
        GameObject whiteText = Instantiate(DamageTextPrefab, transform.position + 1.5f *Vector3.up+0.05f * Vector3.right, Quaternion.identity);
        whiteText.SetActive(true);
        whiteText.layer = 9;
        whiteText.GetComponent<TextMesh>().text = e.DamageValue.ToString();

        Hp -= e.DamageValue;
    }
    public override void OnAttack()
    {
        Debug.Log("Begin Attack..");
        animator.SetTrigger("Shoot");//? 在update里通过枪口动画发射子弹真正攻击
    }
}
