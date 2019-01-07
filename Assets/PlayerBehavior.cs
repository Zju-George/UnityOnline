using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    public virtual void OnAttack() { }
    /// <summary>
    /// 受到伤害的虚函数，子类放实现，是具体的委托的函数实例
    /// </summary>
    /// <param name="source">发布者</param>
    /// <param name="e">参数是一个int，表示伤害数值</param>
    public virtual void OnDamaged(object source, DamageEventArgs e) { }

    //声明2事件，OnAttackFinished()，OnDamaged(float damage)[OnDamage black]
    public event EventHandler<DamageEventArgs> CauseDamage;
    //实际raise event的函数
    protected virtual void OnCauseDamage(int damageVal)
    {
        //? 具体的数值就在这里设置
        CauseDamage?.Invoke(this, new DamageEventArgs() { DamageValue = damageVal });
    }

    public void RemoveEventHandler(object p_Object,string eventname)
    {
        if(p_Object==null)
        {
            Debug.LogWarning("没有对应的eventname");
            return;
        }
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static;
        EventInfo _event = p_Object.GetType().GetEvent(eventname, bindingFlags);
        if (_event == null)
        {
            Debug.LogError("没有对应的eventname");
        }
        else
        {
            FieldInfo _FieldValue = _event.DeclaringType.GetField(eventname, bindingFlags);
            if (_FieldValue != null)
            {
                _FieldValue.SetValue(p_Object, null);
            }
        }
    }
}