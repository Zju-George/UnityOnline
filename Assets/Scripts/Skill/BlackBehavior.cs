using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBehavior : PlayerBehavior
{
    private Animator animator;
    //ps is blood partical
    public ParticleSystem ps;
    
    public void BloodPartical(object send,DamageEventArgs e)
    {
        ps.Play();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        Attack();    
        //? 目前现在这里注册事件的委托
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (player.name == "White")
            {
                CauseDamage += player.GetComponent<WhiteBehavior>().OnDamaged;
            }
        }
        CauseDamage+=BloodPartical;
        //? 注销事件的委托
        RemoveEventHandler((object)GetComponent("WhiteBehavior"),"CauseDamage");
    }
    public void GetDamage()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Damage"))
            animator.Play("Damage");
        else
        {
            animator.Play(animator.GetNextAnimatorStateInfo(0).fullPathHash, 0);
            //BlackAnimator.Update(0);
            animator.CrossFadeInFixedTime("Damage", 0.25f);
            animator.SetTrigger("Damage");
        }

        //? 产生白色的info text和黑色的info text，其旋转是不一样的
        
    }
    public void Attack()
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
        //? 发布制造伤害的事件 设置目标就是注册事件 [先注册目标，再发布制造伤害]
        OnCauseDamage(5);
        
        yield return new WaitForSeconds(22 / 30.0f);
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
