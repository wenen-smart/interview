using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerAttackFinishTrigger : IStateTrigger<PlayerController>
{
    public PlayerAttackFinishTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger() && entity.IsAttackFinishTrigger();
    }

    public override void Init()
    {
       
    }
}

