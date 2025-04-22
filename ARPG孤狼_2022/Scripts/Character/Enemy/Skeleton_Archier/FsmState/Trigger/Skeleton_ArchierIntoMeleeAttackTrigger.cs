using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Skeleton_ArchierIntoMeleeAttackTrigger : IStateTrigger<Skeleton_Archier_Controller>
{
    public Skeleton_ArchierIntoMeleeAttackTrigger(Skeleton_Archier_Controller entity, object machine) : base(entity, machine)
    {
    }

    public override void Init()
    {
        
    }
    public override bool HandleTrigger()
    {
        return base.HandleTrigger()&&entity.TargetIsInAttackRange()&&entity.IsSeeTarget();
    }
}

