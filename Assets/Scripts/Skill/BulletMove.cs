using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float speed;
    public GameObject muzzlePrefab;
    //hit特效
    public GameObject hitPrefab;

    public int damage;
    public int time;

    public GameObject Sender;
    public GameObject Destination;

    void Start()
    {
        Destination = GameObject.Find("Black");
        var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
        muzzleVFX.transform.forward = gameObject.transform.forward;
        var psMuzzle = muzzleVFX.GetComponent<ParticleSystem>();
        if(psMuzzle!=null)
        {
            Destroy(muzzleVFX, psMuzzle.main.duration);
        }
        else
        {
            var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
            Destroy(muzzleVFX, psChild.main.duration);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(speed!=0)
            transform.position += transform.forward * (speed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        speed = 0;
        Sender.GetComponent<PlayerBehavior>().OnCauseDamage(damage);

        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;
        var hitVFX = Instantiate(hitPrefab,pos,rot);
        var psHit = hitVFX.GetComponent<ParticleSystem>();
        if (psHit != null)
        {
            Destroy(hitVFX, psHit.main.duration);
        }
        else
        {
            var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
            Destroy(hitVFX, psChild.main.duration);
        }

        if(time ==Sender.GetComponent<PlayerBehavior>().Damages.Count)
        {  
            Transform[] sons = GetComponentsInChildren<Transform>();
            foreach(var son in sons)
            {
                son.gameObject.SetActive(false);
            }
            Invoke("AttackFinish", 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void AttackFinish()
    {
        Sender.GetComponent<PlayerBehavior>().OnAttakFinished();
        Destroy(gameObject);
    }
}

