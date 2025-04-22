using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : CharacterAnim
{
   
    public PlayerController playerController;

    public PlayerFacade playerFacade;
    public AnimatorStateInfo defenseStateInfo;
    
    public  override void Awake()
    {
        base.Awake();
        playerController = GetComponentInParent<PlayerController>();
        playerFacade = PlayerFacade.Instance;
    }
   



    public override void OpenNextAttackVaildSign()
    {
        playerFacade.playerStateManager.nextAttackVaildSign = true;
    }
    public override void CloseNextAttackVaildSign()
    {
        playerFacade.playerStateManager.nextAttackVaildSign = false;
    }
    public override void PlayWeaponEffect(int id)
    {
        playerController.PlayWeaponEffect(id);
    }

    public override void SwitchEnableAttack(int booleanInt)
    {
        base.SwitchEnableAttack(booleanInt);
        bool enableAttack = booleanInt==1 ?true:false;
     
        if (enableAttack)
        {
            playerFacade.weaponManager.currentWeapon.EnableAttack();
            DeploySkill();
        }
        else
        {
            playerFacade.weaponManager.currentWeapon.DisableAttack();
        }
    }

    public override void HitEnter()
    {
        anim.SetInteger(AnimatorParameter.HitAction.ToString(),0);
        anim.SetFloat(AnimatorParameter.HitAction.ToString(), 0);
        anim.SetFloat(AnimatorParameter.DefenseAction.ToString(), 0);
    }

    public override void DieEnter()
    {
        anim.SetInteger(AnimatorParameter.HitAction.ToString(), 0);
    }

    public void Test()
    {
        Debug.Log(stateInfo.normalizedTime);
    }
    public override void Update()
    {
        base.Update();
        defenseStateInfo = anim.GetCurrentAnimatorStateInfo(1);
    }
   
}
