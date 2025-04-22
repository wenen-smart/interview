using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ClonelyEnemyFacade:CharacterFacade
    {
    private float blendAirValueSpeed = 20;
    public override void Init()
    {
        base.Init();
        playerStateManager.GroundStateChanagedHandler += (ground) => { blendAirValueSpeed = (ground ? 100 : 20); };
    }
    public void DefendEnter()
    {
        //暂时不处理  放到状态机控制了
        //roleController.ClearIncreaseVelo();
        //roleController.ClearMoveSpeed();
        //weaponManager.StartDefense();
    }
    public void DefendExit()
    {
        //暂时不处理  放到状态机控制了
        //roleController.EnableCtrl();
        //weaponManager.CanelDefense();
    }
    public void ReboundEnter()
    {
        roleController.DisableCtrl();
        roleController.ClearMoveSpeed();
        roleController.ClearIncreaseVelo();

        characterAnim.SetInt(AnimatorParameter.DefenseAction.ToString(), 0);

    }
    public void ReboundExit()
    {
        playerStateManager.isReBound = false;
        roleController.EnableCtrl();

        Debug.Log(playerStateManager.isDenfense);
    }
    public void OpenController()
    {
        roleController.EnableCtrl();
    }
    public void LoseAllControllerAndSpeed()
    {
        roleController.DisableCtrl();
        roleController.ClearMoveSpeed();
        roleController.ClearIncreaseVelo();
        characterAnim.SetFloat(AnimatorParameter.Forward.ToString(), 0);
        characterAnim.SetFloat(AnimatorParameter.VeloDir.ToString(), 0);
    }
    public void DefenseHitEnter()
    {
        Debug.Log("DefenseHitEnter ");
        characterAnim.SetInt(AnimatorParameter.HitAction.ToString(), 0);
        characterAnim.SetInt(AnimatorParameter.DefenseAction.ToString(), 0);
    }


    public void ReBoundedEnter()
    {
        characterAnim.SetInt(AnimatorParameter.HitAction.ToString(), 0);
    }
    public void LateUpdate()
    {
        CheckGround();
    }
    public void CheckGround()
    {
        characterAnim.SetBool(AnimatorParameter.isGround.ToString(), playerStateManager.IsGround);
        roleController.LerpAnimationFloatVlaue(AnimatorParameter.AirValue.ToString(),playerStateManager.IsGround==true?0:10,blendAirValueSpeed*Time.deltaTime);
    }
}

