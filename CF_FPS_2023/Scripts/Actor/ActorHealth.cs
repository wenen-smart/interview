using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorHealth : Hittable,IActor
{
    public int MaxHp;
    public int currentHp;
    public Action<DamageInfo> OnDie;
    public Action<DamageInfo> OnDamage;
    public bool isDeath { get; private set; }
    private ActorSystem _actorSystem;
    public ActorSystem actorSystem
    {
        get
        {
            if (_actorSystem == null)
            {
                _actorSystem = GetComponent<ActorSystem>() ?? GetComponentInParent<ActorSystem>();
            }
            return _actorSystem;
        }
        set
        {
            _actorSystem = value;
        }
    }
    public Action<int> OnHPChangeHandler;
    public void Init()
    {
        actorSystem.RegisterActorObject(gameObject,this);
        isDeath = false;
        SetHp(MaxHp);
    }
    public void SetHp(int hp)
    {
        int old = currentHp;
        currentHp = Mathf.Clamp(hp,0,MaxHp);
        if (currentHp != old)
        {
            OnHPChangeHandler?.Invoke(currentHp);
        }
    }
    public override void Damage(DamageInfo damageInfo)
    {
        base.Damage(damageInfo);
        var targetHp = currentHp - damageInfo.damage;
        SetHp(targetHp);
        if (targetHp>0)
        {
            OnDamage?.Invoke(damageInfo);
        }
        else
        {
            OnDie?.Invoke(damageInfo);
            isDeath = true;
        }
    }

    public void AddActorComponent<T>() where T : ActorComponent
    {
        ActorComponent.AddActorComponent<T>(actorSystem,gameObject);
    }

    public T GetActorComponent<T>() where T : MonoBehaviour,IActor
    {
        if (this is T)
        {
            return this as T;
        }
        return actorSystem.GetActorComponent<T>();
    }
}
public struct DamageInfo
{
    public int damage;
    public RoleController caster;
    public DamageTypes damageType;
    public WeaponDataConfig casterWeaponInfo;
    public Vector3 castDir;
    public RagDollBone damagedBodyPart;
    public Vector3 hitPoint;
    public Vector3 hitNormal;
    public float ammo_size;
    public float _DamagedTime;

    public DamageInfo(int damage, RoleController caster, DamageTypes damageType, WeaponDataConfig casterWeaponInfo, Vector3 castDir, RagDollBone damagedBodyPart, Vector3 hitPoint, Vector3 hitNormal, float ammo_size, float damagedTime)
    {
        this.damage = damage;
        this.caster = caster;
        this.damageType = damageType;
        this.casterWeaponInfo = casterWeaponInfo;
        this.castDir = castDir;
        this.damagedBodyPart = damagedBodyPart;
        this.hitPoint = hitPoint;
        this.hitNormal = hitNormal;
        this.ammo_size = ammo_size;
        _DamagedTime = damagedTime;
    }
}


