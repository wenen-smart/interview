using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyInAttackRangeTrigger<EntityT> : IStateTrigger<EntityT> where EntityT:EnemyController
{
    public EnemyInAttackRangeTrigger(EntityT entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger()&&entity.SatisfyAttackCondition();
    }

    public override void Init()
    {
        
    }
}

