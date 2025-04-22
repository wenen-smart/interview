using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyAnim : CharacterAnim
{
    [HideInInspector]
    public EnemyController enemyController;
    public override void ActorComponentAwake()
    {
        base.ActorComponentAwake();
        enemyController=GetActorComponent<EnemyController>();
    }
    public override void HitEnter()
    {
        anim.SetInteger(AnimatorParameter.HitAction.ToString(), 0);
    }

    public override void DieEnter()
    {
        anim.SetInteger(AnimatorParameter.HitAction.ToString(), 0);
    }
    public void PlayWeaponEffect(int id)
    {
       
    }

}

