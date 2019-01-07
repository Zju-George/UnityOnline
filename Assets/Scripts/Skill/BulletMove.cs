using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float speed;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;//hit vfx
    public GameObject HitPlayer;
    // Start is called before the first frame update
    void Start()
    {
        HitPlayer = GameObject.Find("Black");
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
        Type t = Type.GetType(HitPlayer.name + "Behavior");
        HitPlayer.GetComponent(t).SendMessage("GetDamage");

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



        Destroy(gameObject);
    }
}
