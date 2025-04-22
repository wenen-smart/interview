using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ACTIsStabAttackTrigger : IStateTrigger<PlayerController>
{
    public ACTIsStabAttackTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger() && entity.IsStabAttackTrigger();
    }

    public override void Init()
    {
       
    }
}

