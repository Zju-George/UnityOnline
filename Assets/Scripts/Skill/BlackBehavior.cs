using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBehavior : MonoBehaviour
{
    private Animator animator;
    public new ParticleSystem particleSystem;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Attack();       
    }

    void Attack()
    {
        StartCoroutine(RunForward()); 
    }
    IEnumerator RunForward()
    {
        animator.SetBool("Run", true);
        for (int i=0;i<50;i++)
        {
            transform.position -= 0.04f*Vector3.forward;
            yield return new WaitForSeconds(0.01f);
        }
        animator.SetBool("Run", false);
        yield return new WaitForSeconds(28 / 30.0f);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (player.name == "WhiteCharactor")
                player.GetComponent<WhiteBehaviour>().GetDamage();
        }
        yield return new WaitForSeconds(2 / 30.0f);
        particleSystem.Play();
        //? 这个以后要可以设置目标
        yield return new WaitForSeconds(20 / 30.0f);
        StartCoroutine(GotBack());
    }
    IEnumerator GotBack()
    {
        for (int i=0;i<30;i++)
        {
            transform.position += 0.25f / 40 * Vector3.up;
            transform.position += 2.0f/40 * Vector3.forward;
            yield return new WaitForSeconds(0.01f);
        }
        for(int i=0;i<10;i++)
        {
            transform.position -= 0.75f / 40 * Vector3.up;
            transform.position += 2.0f / 40 * Vector3.forward;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
