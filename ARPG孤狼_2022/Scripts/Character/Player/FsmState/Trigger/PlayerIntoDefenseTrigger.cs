using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerIntoDefenseTrigger : IStateTrigger<PlayerController>
{
    public PlayerIntoDefenseTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger() && entity.IntoDefenseTrigger();
    }

    public override void Init()
    {
        
    }
}

