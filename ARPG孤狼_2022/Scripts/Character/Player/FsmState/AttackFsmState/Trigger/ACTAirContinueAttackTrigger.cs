using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ACTAirContinueAttackTrigger : IStateTrigger<PlayerController>
{
    public ACTAirContinueAttackTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
    }

    public override bool HandleTrigger()
    {
        return base.HandleTrigger()&& entity.AirComboAttackTrigger(); 
    }

    public override void Init()
    {
        
    }
    public override bool SpecialExtral_ilCondition()
    {
        return entity.stateManager.waitNextAttackInput;
    }
}

