using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Skeleton_ArchierLoseTargetTrigger : IStateTrigger<Skeleton_Archier_Controller>
{
    public Skeleton_ArchierLoseTargetTrigger(Skeleton_Archier_Controller entity, object machine) : base(entity, machine)
    {

    }

    public override void Init()
    {
        
    }
    public override bool HandleTrigger()
    {
        return base.HandleTrigger()&&entity.TargetIsInFollowRange()==false||entity.TargetDiedOrNULL();
    }
}

