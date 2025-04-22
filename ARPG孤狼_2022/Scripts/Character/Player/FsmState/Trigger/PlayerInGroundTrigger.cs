using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerInGroundTrigger : IStateTrigger<PlayerController>
{
    public PlayerInGroundTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger() && entity.InGroundTrigger();
    }

    public override void Init()
    {
        
    }
}

