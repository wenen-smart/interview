using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class WolfGoFollowTrigger : IStateTrigger<WolfController>
{
    public WolfGoFollowTrigger(WolfController entity, object machine) : base(entity, machine)
    {
    }

    public override void Init()
    {
        
    }
    public override bool HandleTrigger()
    {
        return base.HandleTrigger()&&entity.IsHaveFollowTarget();
    }
}

