using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerChanelDefenseTrigger : IStateTrigger<PlayerController>
{
    public PlayerChanelDefenseTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger() && entity.CanelDefenseTrigger();
    }

    public override void Init()
    {
       
    }
}

