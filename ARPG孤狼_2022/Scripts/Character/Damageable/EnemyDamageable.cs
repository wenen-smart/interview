using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageable : IDamageable
{
    [HideInInspector]
    public Animator anim;


    // Start is called before the first frame update
    public override void Init()
    {
        base.Init();
        anim = GetActorComponent<CharacterAnim>().anim;
    }
    public override void Die(DamageData damageData)
    {
        base.Die(damageData);
    }
    protected override void Damage(DamageData damageData)
    {
        base.Damage(damageData);

    }
}
