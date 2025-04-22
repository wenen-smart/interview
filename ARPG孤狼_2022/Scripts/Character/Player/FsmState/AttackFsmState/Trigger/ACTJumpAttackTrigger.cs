using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ACTJumpAttackTrigger : IStateTrigger<PlayerController>
{
    public ACTJumpAttackTrigger(PlayerController entity, object machine) : base(entity, machine)
    {

    }
    public override bool HandleTrigger()
    {
        return base.HandleTrigger() && entity.JumpAttackTrigger(); 
    }

    public override void Init()
    {
        
    }
}

