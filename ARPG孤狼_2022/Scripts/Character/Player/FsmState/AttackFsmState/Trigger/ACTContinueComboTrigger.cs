using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ACTContinueComboTrigger : IStateTrigger<PlayerController>
{
    public ACTContinueComboTrigger(PlayerController entity, object machine) : base(entity, machine)
    {

    }

    public override bool HandleTrigger()
    {

        return base.HandleTrigger() && entity.ComboAttackTrigger();
    }

    public override void Init()
    {
       
    }
    public override bool SpecialExtral_ilCondition()
    {
        return entity.stateManager.nextAttackVaildSign;
    }
}

