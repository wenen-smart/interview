using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ACTShiftAttackTrigger : IStateTrigger<PlayerController>
{
    public ACTShiftAttackTrigger(PlayerController entity, object machine) : base(entity, machine)
    {
    }

    public override void Init()
    {
       
    }
}

