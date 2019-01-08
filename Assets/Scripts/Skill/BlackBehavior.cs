using System;
using System.Collections;
using UnityEngine;

public class BlackBehavior : PlayerBehavior
{

    //ps is blood partical
    public ParticleSystem ps;
    
    public void BloodPartical(object send,DamageEventArgs e)
    {
        ps.Play();
    }
    void Start()
    {
        DamageTextPrefab = GameObject.Find("Prefabs").transform.Find("DamageText").gameObject; 
        animator = GetComponent<Animator>();
        
        //? 目前现在这里注册事件的委托
        //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //foreach (var player in players)
        //{
        //    if (player.name == "White")
        //    {
        //        CauseDamage += player.GetComponent<PlayerBehavior>().OnDamaged;
        //    }
        //}
        //CauseDamage+=BloodPartical;
        //? 注销事件的委托
        //RemoveEventHandler("CauseDamage");
        
    }
    public override void OnDamaged(object source, DamageEventArgs e)
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
        Hp -= e.DamageValue;
        //? 产生白色的info text和黑色的info text，其旋转是不一样的
        GameObject blackText = Instantiate(DamageTextPrefab, transform.position + 1.5f * Vector3.up, Quaternion.identity);
        blackText.SetActive(true);
        blackText.GetComponent<Transform>().localEulerAngles = new Vector3(0, 180, 0);
        blackText.layer = 10;
        blackText.GetComponent<TextMesh>().text = e.DamageValue.ToString();
        GameObject whiteText = Instantiate(DamageTextPrefab, transform.position + 1.3f * Vector3.up + 0.05f * Vector3.right, Quaternion.identity);
        whiteText.SetActive(true);
        whiteText.layer = 9;
        whiteText.GetComponent<TextMesh>().text = e.DamageValue.ToString();
    }
    public override void OnAttack(object source, EventArgs e)
    {
        StartCoroutine(RunForward()); 
    }
    IEnumerator RunForward()
    {
        animator.SetBool("Run", true);
        for (int i=0;i<50;i++)
        {
            transform.position -= 0.04f*Vector3.forward;//? Vector3.forward 改成一个向量指向攻击对象
            yield return new WaitForSeconds(0.01f);
        }
        animator.SetBool("Run", false);
        yield return new WaitForSeconds(28 / 30.0f);
        //? 发布制造伤害的事件 设置目标就是注册事件 [先注册目标，再发布制造伤害]
        this.OnCauseDamage(Damages[0]);
        
        yield return new WaitForSeconds(22 / 30.0f);
        StartCoroutine(RunBack());
    }
    IEnumerator RunBack()
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
