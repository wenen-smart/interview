using Assets.Resolution.Scripts.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ExplosionGrenadeEntity : ThrowItemAmmoEntity
{
    public bool isIgnoreDamageToTeam { get; set; }
    protected override void Activate()
    {
        InstantiateEffect();
        int hitScansMask=~(1<<2);
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius,hitScansMask);

        foreach (var c in colliders)
        {
            var actor = c.GetComponent<ActorComponent>();

            if (actor != null)
            {
                var role = actor.GetActorComponent<RoleController>();
                if (isIgnoreDamageToTeam&&role.team==user.team)
                {
                    continue;
                }
                DamageInfo damageInfo=new DamageInfo();
                damageInfo.ammo_size = 1;
                damageInfo.caster = user;
                damageInfo.damageType = DamageTypes.Explosion;
                damageInfo.casterWeaponInfo = throwItemData;
                damageInfo._DamagedTime = Time.time;
                damageInfo.damage = CalcuateDamage(actor.transform.position);
                actor.GetActorComponent<ActorHealth>().Damage(damageInfo);
            }
        }
        TimeSystem.Instance.TimerUpdateFinish(LifeTimer);
        GameObjectFactory.Instance.PushItem(gameObject);
    }
}
