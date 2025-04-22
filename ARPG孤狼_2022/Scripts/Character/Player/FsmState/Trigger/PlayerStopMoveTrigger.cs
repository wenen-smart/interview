using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerStopMoveTrigger : IStateTrigger<PlayerController>
{
    public PlayerStopMoveTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
        
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger() && entity.StopMoveTrigger();
    }

    public override void Init()
    {
       
    }
}

