using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerJumpExitTrigger : IStateTrigger<PlayerController>
{
    public PlayerJumpExitTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger() && entity.JumpExitTrigger();
    } 

    public override void Init()
    {
        
    }
}

