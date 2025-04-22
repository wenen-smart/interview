using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerHangFallTrigger : IStateTrigger<PlayerController>
{
    public PlayerHangFallTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger() && entity.HangFallTrigger(); ;
    }

    public override void Init()
    {
        
    }
}

