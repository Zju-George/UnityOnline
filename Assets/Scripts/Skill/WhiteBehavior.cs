using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBehavior : PlayerBehavior
{
    public GameObject firePoint;
    public Transform destination;// 射击的终点
    public List<GameObject> vfx = new List<GameObject>();

    private GameObject effectToSpawn;
    private Animator anim;
    private bool firstShoot = false;
    private bool secondShoot = false;
    // Start is called before the first frame update
    void Start()
    {
        effectToSpawn = vfx[0];
        anim = GetComponent<Animator>();
        //anim.SetTrigger("Shoot");
    }

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("die_back_rest")&& anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5)
        {
            anim.SetFloat("Speed", -1.0f);
            return;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("shoot_single_ar"))
        {
            if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime>0.3&&!firstShoot)
            {
                firstShoot = true;
                SpawnVFX();     
            }
            if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.4&&!secondShoot)
            {
                secondShoot = true;
                SpawnVFX();
            }
        }
        else
        {
            firstShoot = false;
            secondShoot = false;
        }

    }

    internal void OnDamaged(BlackBehavior blackBehavior, object e)
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    public void SpawnVFX()
    {
        GameObject vfx = Instantiate(effectToSpawn, firePoint.transform.position, Quaternion.identity);
        Vector3 direction = destination.position - firePoint.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        vfx.transform.localRotation = rotation;
    }
    public override void OnDamaged(object source, DamageEventArgs e)
    {
        anim.SetFloat("Speed", 1.5f);
        anim.SetTrigger("Damage");
    }
}
