using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerInputMoveTrigger : IStateTrigger<PlayerController>
{
    public PlayerInputMoveTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger() && entity.InputMoveTrigger();
    }

    public override void Init()
    {
       
    }
}

