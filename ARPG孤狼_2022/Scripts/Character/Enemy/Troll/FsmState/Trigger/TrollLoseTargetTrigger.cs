using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TrollLoseTargetTrigger : IStateTrigger<TrollController>
{
    public TrollLoseTargetTrigger(TrollController entity, object machine) : base(entity, machine)
    {
    }

    public override void Init()
    {
        
    }
    public override bool HandleTrigger()
    {
        return base.HandleTrigger()&&entity.TargetIsInFollowRange()==false;
    }
}