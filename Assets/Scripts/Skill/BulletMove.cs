using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    public float speed;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public Animator BlackAnimator;
    // Start is called before the first frame update
    void Start()
    {
        BlackAnimator = GameObject.Find("BlackCharactor").GetComponent<Animator>();
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
        if (!BlackAnimator.GetCurrentAnimatorStateInfo(0).IsName("Damage"))
            BlackAnimator.Play("Damage");
        else
        {
            BlackAnimator.Play(BlackAnimator.GetNextAnimatorStateInfo(0).fullPathHash, 0);
            //BlackAnimator.Update(0);
            BlackAnimator.CrossFadeInFixedTime("Damage", 0.25f);
            BlackAnimator.SetTrigger("Damage");

        }

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
