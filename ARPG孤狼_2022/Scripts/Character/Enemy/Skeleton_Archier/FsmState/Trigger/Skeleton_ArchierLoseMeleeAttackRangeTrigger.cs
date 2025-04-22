using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Skeleton_ArchierLoseMeleeAttackRangeTrigger : IStateTrigger<Skeleton_Archier_Controller>
{
    public Skeleton_ArchierLoseMeleeAttackRangeTrigger(Skeleton_Archier_Controller entity, object machine) : base(entity, machine)
    {
    }
    public override bool HandleTrigger()
    {
        return base.HandleTrigger()&&entity.TargetIsInAttackRange()==false;
    }

    public override void Init()
    {
        
    }
}

