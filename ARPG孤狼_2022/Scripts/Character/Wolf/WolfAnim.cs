using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class WolfAnim : CharacterAnim
{
    public override void HitEnter()
    {
        anim.SetInteger(AnimatorParameter.HitAction.ToString(), 0);
    }

    public override void DieEnter()
    {
        anim.SetInteger(AnimatorParameter.HitAction.ToString(), 0);
    }
}

