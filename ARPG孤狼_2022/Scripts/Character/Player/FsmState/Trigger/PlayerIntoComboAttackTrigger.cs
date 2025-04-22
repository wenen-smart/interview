using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerIntoComboAttackTrigger : IStateTrigger<PlayerController>
{
    public PlayerIntoComboAttackTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger() && entity.ComboAttackTrigger();
    }

    public override void Init()
    {
        
    }
}

