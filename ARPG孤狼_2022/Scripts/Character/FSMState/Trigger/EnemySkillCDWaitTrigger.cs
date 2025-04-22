using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class EnemySkillCDWaitTrigger<EntityT> : IStateTrigger<EntityT> where EntityT:EnemyController
{

    public EnemySkillCDWaitTrigger(EntityT entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger();
    }

    public override void Init()
    {
        
    }
}

