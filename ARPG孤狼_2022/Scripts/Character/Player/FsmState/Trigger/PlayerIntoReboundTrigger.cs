using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerIntoReboundTrigger : IStateTrigger<PlayerController>
{
    public PlayerIntoReboundTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger() && entity.IntoReboundTrigger();
    }

    public override void Init()
    {
       
    }
}

