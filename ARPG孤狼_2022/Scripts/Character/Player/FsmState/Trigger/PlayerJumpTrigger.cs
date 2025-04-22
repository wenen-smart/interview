using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerJumpTrigger : IStateTrigger<PlayerController>
{
    public PlayerJumpTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger() && entity.JumpTrigger();
    }

    public override void Init()
    {
      
    }
}

