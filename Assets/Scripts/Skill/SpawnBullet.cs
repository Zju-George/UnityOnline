using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBullet : MonoBehaviour
{

    public GameObject firePoint;
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
    // Update is called once per frame
    public void SpawnVFX()
    {
        
        if(firePoint!=null)
        {
            GameObject vfx = Instantiate(effectToSpawn, firePoint.transform.position, Quaternion.identity);
        }

    }
}
