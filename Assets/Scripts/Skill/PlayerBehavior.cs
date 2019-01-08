using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DamageEventArgs:EventArgs
{
    public int DamageValue { get; set; }
}

public class PlayerBehavior : MonoBehaviour
{
    [Range(0, 100)]
    public int Hp;

    public List<GameObject> Desitinations = new List<GameObject>();
    public List<int> Damages = new List<int>();
    protected Animator animator;
    protected GameObject DamageTextPrefab;
    public virtual void OnAttack(object source,EventArgs e) { }
    /// <summary>
    /// 受到伤害的虚函数，子类放实现，是CauseDamage事件的委托的函数实例
    /// </summary>
    /// <param name="source">发布者</param>
    /// <param name="e">参数是一个int，表示伤害数值</param>
    public virtual void OnDamaged(object source, DamageEventArgs e) { }

    //声明2事件，AttackFinished ，CauseDamage
    public event EventHandler AttackFinished;
    public event EventHandler<DamageEventArgs> CauseDamage;
    //? 发布 Damage event
    public virtual void OnCauseDamage(int damageVal)//? 具体的伤害就在这里设置
    {
        CauseDamage?.Invoke(this, new DamageEventArgs() { DamageValue = damageVal });
    }
    //? 发布 AttackFinished event
    public virtual void OnAttakFinished()
    {
        AttackFinished?.Invoke(this,EventArgs.Empty);
    }
    public void RemoveEventHandler(string eventname)
    {
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static;
        EventInfo _event = this.GetType().GetEvent(eventname, bindingFlags);
        if (_event == null)
        {
            Debug.LogError("没有对应的eventname");
        }
        else
        {
            FieldInfo _FieldValue = _event.DeclaringType.GetField(eventname, bindingFlags);
            if (_FieldValue != null)
            {
                _FieldValue.SetValue(this, null);
            }
        }
    }
}