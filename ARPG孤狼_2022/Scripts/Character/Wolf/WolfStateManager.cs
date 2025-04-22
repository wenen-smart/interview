using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class WolfStateManager : ActorStateManager
{
    public override void Init()
    {
        base.Init();
        isAttack = false;
        isHit = false;
        isDie = false;
    }
    public override void Update()
    {
        base.Update();
        isAttack = characterAnim.stateInfo.IsTag(AnimatorParameter.Attack.ToString());
        isHit = characterAnim.stateInfo.IsTag("Hit");
        isDie = characterAnim.stateInfo.IsTag("Death");
    }
}

