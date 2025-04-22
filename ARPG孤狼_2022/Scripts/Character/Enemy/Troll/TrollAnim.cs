using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrollAnim : CharacterAnim
{
    public override void DieEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void HitEnter()
    {
        throw new System.NotImplementedException();
    }
    public override void OnAnimatorMove()
    {
        base.OnAnimatorMove();
    }
    public override void SwitchEnableAttack(int booleanInt)
    {
        base.SwitchEnableAttack(booleanInt);
        bool enableAttack = booleanInt == 1 ? true : false;

        if (enableAttack)
        {
            characterFacade.weaponManager.currentWeapon.EnableAttack();
            DeploySkill();
        }
        else
        {
            characterFacade.weaponManager.currentWeapon.DisableAttack();
        }
    }
}
