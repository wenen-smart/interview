using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Skeleton_ArchierFollowUpTargetTrigger : IStateTrigger<Skeleton_Archier_Controller>
{
    public Skeleton_ArchierFollowUpTargetTrigger(Skeleton_Archier_Controller entity, object machine) : base(entity, machine)
    {
    }

    public override void Init()
    {
       
    }
    public override bool HandleTrigger()
    {
        return base.HandleTrigger()&&entity.TargetIsInFollowRange()&&entity.IsSeeTarget();
    }
}

