using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;

public class PlayerFacade:CharacterFacade
    {
    [HideInInspector]
  public  PlayerController playerController;
   
    [HideInInspector]
    public PlayerHangSystem playerHangSystem;
    private static PlayerFacade instance;

    public static PlayerFacade Instance
    {

        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerFacade>();
            }
            return instance;
        }
        set
        {
            instance = value;
        }

    }
    public override void Awake()
    {
        Instance = this;
        base.Awake();
    }
    public override void Init()
    {
        base.Init();
        playerController = roleController as PlayerController;
        playerHangSystem = GetComponent<PlayerHangSystem>();
        playerHangSystem.Init();
        BattleManager.Instance.LoadAllActorInto();//暂时测试
        
    }

    #region Animator ReturnInvoke
    #region runStop
    public void RunStopEnter()
    {
        playerController.DisableCtrl();
        Debug.Log("RunStopEnter" + Time.time);
    }
    public void RunStopExit()
    {
        playerController.EnableCtrl();
        Debug.Log("RunStopExit" + Time.time);
    }

    public void RunStopUpdate(AnimatorUpdateArgSaver argSaver)
    {
        playerController.CheckingQuickTurn();
    }
    #endregion
        #region jump
    public void Jump1Enter()
    {
        playerStateManager.IsGround = false;
        playerController.DisableCtrl();
        playerController.SetLastMoveSpeedMultToCurrent(PlayerableConfig.Instance.jump_惯性_Mult);
        Debug.Log("Jump1Enter" + Time.time);
    }
    
    public void Jump1Exit()
    {
        characterAnim.anim.SetInteger(AnimatorParameter.JumpAction.ToString(), 0);
        playerController.EnableCtrl();
        //Debug.Log("Jump1Exit" + Time.time);
        SetFall();
    }
    public void Jump1MotionCurveUpdate(AnimatorUpdateArgSaver saver, List<AnimationCurve> curve)
    {
        var y = curve[0].Evaluate(saver.stateInfo.normalizedTime);
        playerController.IncreaseVelo += new Vector3(0, y, 0);
    }
    public void Jump2MotionCurveUpdate(AnimatorUpdateArgSaver saver, List<AnimationCurve> curve)
    {
        var y = curve[0].Evaluate(saver.stateInfo.normalizedTime);
        y =y * InputSystem.Instance.jumpValue;
        playerController.IncreaseVelo += new Vector3(0, y, 0);
    }
    public void Jump2Enter()
    {
        playerStateManager.IsGround = false;
        playerController.DisableCtrl();

        playerController.SetLastMoveSpeedMultToCurrent(PlayerableConfig.Instance.jump_惯性_Mult);
        Debug.Log("Jump2Enter" + Time.time);
    }
    public void Jump2Exit()
    {
        characterAnim.anim.SetInteger(AnimatorParameter.JumpAction.ToString(), 0);
        //playerController.EnableCtrl();
        SetFall();
        Debug.Log("Jump2Exit" + Time.time);
    }
    #endregion
        #region fall
  
    public void FallEnter()
    {
        playerController.DisableCtrl();

        playerController.SetLastMoveSpeedMultToCurrent(PlayerableConfig.Instance.fall_惯性_Mult);
        //Debug.Log("FallEnter" + Time.time);
    }
    public void FallExit()
    {
        characterAnim.anim.SetInteger(AnimatorParameter.JumpAction.ToString(), 0);
        playerController.EnableCtrl();
        playerController.EnableRotate(false);
        Debug.Log("playerController isCtrlRotate=" + playerController.isCtrlRotate);
        Debug.Log("FallExit" + Time.time);
    }
    public void FallLanding1Enter()
    {
        Debug.Log("FallLanding1Enter();" + Time.time);
        playerController.DisableCtrl();
        playerController.ClearMoveSpeed();
    }
    public void FallLanding1Exit()
    {
        playerController.EnableCtrl();
    }
    public void FallUpdate(AnimatorUpdateArgSaver saver, List<AnimationCurve> curve)
    {
        var y = curve[0].Evaluate(saver.stateInfo.normalizedTime);
        playerController.IncreaseVelo += new Vector3(0, y, 0);
        //if (saver.stateInfo.normalizedTime>0.2f)
        //{
        //    playerController.EnableRotate(true);
        //}
        var loseCtrlMoveSpeed = playerController.loseCtrlMoveSpeed;
        //bool isGround = actorPhyiscal.CastGround(PlayerableConfig.Instance.check_InGroundWhenFall_MaxDis, true);
        bool isGround= playerStateManager.IsGround;
        bool isPressMove = InputSystem.Instance.CurrentPressMoveKey();
        if (loseCtrlMoveSpeed >= (/*PlayerableConfig.Instance.quickMaxFallSpeed+*/PlayerableConfig.Instance.commonFallSpeed) / 2 && isPressMove)
        {
            if (isGround)
            {
                characterAnim.anim.SetInteger(AnimatorParameter.JumpAction.ToString(), 5);
                playerController.ClearIncreaseVelo();
            }
        }
        else
        {
            if (isGround)
            {
                characterAnim.anim.SetInteger(AnimatorParameter.JumpAction.ToString(), 4);
                playerController.ClearIncreaseVelo();
            }

        }
    }
    public void FallLanding2Enter()
    {
        playerController.DisableCtrl();
        playerController.SetLastMoveSpeedMultToCurrent(PlayerableConfig.Instance.fall2Landing_惯性_Mult);
    }
    public void FallLanding2Exit()
    {
        playerController.EnableCtrl();
    }
    #endregion
    #region closetWall
    public void ClosetWall_OnStandToCover_Enter()
    {
        playerController.DisableCtrl();
        playerController.ClearIncreaseVelo();
        playerController.ClearMoveSpeed();
        playerController.ExChangeMoveMode(ActorMoveMode.ClosetWall);
    }
    public void ClosetWall_OnStandToCover_Exit()
    {
        playerController.EnableCtrl();
        //characterAnim.SetInt(AnimatorParameter.CoverAction.ToString(),2);
    }
    public void ClosetWall_OnCoverToStand_Enter()
    {
        playerController.CtrlEnableGravity(false);
        playerController.SetCharacterColliderEnable(false);
        playerController.ExChangeMoveMode(ActorMoveMode.Common);
        playerController.EnableCtrl();
        playerController.DisableCtrl(false);
        characterAnim.SetInt(AnimatorParameter.CoverAction.ToString(), 0);
    }

    public void ClosetWall_OnCoverToStand_Exit()
    {
        playerController.EnableCtrl();
        playerController.CtrlEnableGravity(true);
        playerController.SetCharacterColliderEnable(true);

    }
    
    public void ClosetWall_Locomotion_Enter()
    {
        playerController.DisableCtrl();
        playerController.EnableCtrl(false);
    }
    public void Closetwall_Locomotion_Exit()
    {
        CameraMgr.Instance.CloseVirtualAim();
       
    }

    #endregion

    public void Rush0Update(AnimatorUpdateArgSaver saver)
    {
        if (InputSystem.Instance.shiftEvent.pressAxis>0.75f)
        {
            characterAnim.SetTrigger(AnimatorParameter.Rush.ToString());
        }
    }

    public override void SetColliderHeightWhenJump()
    {
        base.SetColliderHeightWhenJump();
        playerController.SetCharacterHeightWhenJump();
    }
    public void LocomotionEnter()
    {
        characterAnim.SetInt(AnimatorParameter.JumpAction.ToString(), 0);
        SetMirrorParameter(false);
    }
    public void AnimatorMonoUpdate(AnimatorUpdateArgSaver argSaver)
    {

        string methodName = argSaver.methodName;
        if (string.IsNullOrEmpty(methodName) || string.IsNullOrWhiteSpace(methodName))
        {
            Debug.LogWarning("-----动画机想要调用的Update方法名为空------");
            return;
        }
        AnimatorTransArgType argType = argSaver.argType;
        string arg = argSaver.arg;
        MethodInfo methodInfo = this.GetType().GetMethod(methodName);
        switch (argType)
        {
            case AnimatorTransArgType.Float:
                float argFloat;
                if (float.TryParse(arg, out argFloat))
                {
                    methodInfo.Invoke(this, new object[] { argSaver, argFloat });
                }

                break;
            case AnimatorTransArgType.Int:
                int argInt;
                if (int.TryParse(arg, out argInt))
                {
                    methodInfo.Invoke(this, new object[] { argSaver, argInt });
                }

                break;
            case AnimatorTransArgType.Object:
                methodInfo.Invoke(this, new object[] { argSaver, arg });
                break;
            case AnimatorTransArgType.Bool:
                bool argBool;
                if (bool.TryParse(arg, out argBool))
                {
                    methodInfo.Invoke(this, new object[] { argSaver, argBool });
                }
                break;
            case AnimatorTransArgType.String:
                methodInfo.Invoke(this, new object[] { argSaver, arg });
                break;
            case AnimatorTransArgType.AnimatorCurve:
                string fieldNameStrs = (string)arg;
                string[] fieldNames = fieldNameStrs.Split('|');
                List<AnimationCurve> animationCurves = new List<AnimationCurve>();
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    if (string.IsNullOrEmpty(fieldNames[i]) || string.IsNullOrWhiteSpace(fieldNames[i]))
                    {
                        Debug.LogWarning("-----动画机想要调用的Update方法名为空------");
                        continue;
                    }
                    Type type = PlayerAnimatorConfig.Instance.GetType();
                    FieldInfo fieldInfo = type.GetField(fieldNames[i]);
                    object curveObj = fieldInfo.GetValue(PlayerAnimatorConfig.Instance);
                    AnimationCurve animationCurve = curveObj as AnimationCurve;
                    animationCurves.Add(animationCurve);
                }

                methodInfo.Invoke(this, new object[] { argSaver, animationCurves });
                break;
            default:
                methodInfo.Invoke(this, new object[] { argSaver });
                break;
        }
    }
    #endregion
    public void SetFall()
    {
        if (playerStateManager.IsGround == false)
        {
            characterAnim.anim.SetInteger(AnimatorParameter.JumpAction.ToString(), 3);
        }
    }

    public void HandleHangUpWall()
    {
        playerController.ExChangeMoveMode(ActorMoveMode.Shimmy);
    }
    public void HangWallEnter()
    {
        characterAnim.SetInt(AnimatorParameter.HangAction.ToString(), 0);
        playerController.DisableCtrl();
        playerController.ClearIncreaseVelo();
        playerController.ClearMoveSpeed();
        playerController.SetCharacterColliderEnable(false);
        playerController.CtrlEnableGravity(false);
        playerController.ExChangeMoveMode(ActorMoveMode.Shimmy);
        characterAnim.StartAllHandIK();
    }
    public void HangWallAnimUpdate(AnimatorUpdateArgSaver animatorUpdateArgSaver)
    {
        float speed = PlayerableConfig.Instance.hangUpToBlendSpeedCurve.Evaluate(animatorUpdateArgSaver.stateInfo.normalizedTime);
        if (playerHangSystem.bodyPoint != Vector3.zero)
        {
            playerController.LerpToTargetOnUpdate(playerHangSystem.bodyPoint,speed , false);
        }
    }
    public void HangWallExit()
    {
        playerController.ExChangeMoveMode(ActorMoveMode.Common);
    }
    public void HandlerHangDown()
    {
        playerController.ExChangeMoveMode(ActorMoveMode.Common);
        characterAnim.SetInt(AnimatorParameter.HangAction.ToString(),-1);
        playerController.SetCharacterPosition(transform.position,Vector3.up*0.02f,1);
       
    }
    public void HandleHangToClimb()
    {
        characterAnim.SetInt(AnimatorParameter.HangAction.ToString(),2);
    }
    public void HangToClimEnter()
    { 
          characterAnim.SetInt(AnimatorParameter.HangAction.ToString(), 0);
    playerController.SetCharacterColliderEnable(false);
        playerController.CtrlEnableGravity(false);
        playerController.ExChangeMoveMode(ActorMoveMode.Common);
        GameRoot.Instance.LeaveHandlerMainEvent(EventCode.StandToDownHang);
    }
    public void HangToClimbExit()
    {
        //var targetPos = characterAnim.anim.bodyPosition;
        //targetPos.y = characterAnim.anim.GetIKPosition(AvatarIKGoal.LeftFoot).y;
        //transform.position = targetPos;
        playerController.EnableCtrl();
        playerController.SetCharacterColliderEnable(true);
        playerController.CtrlEnableGravity(true);
        playerHangSystem.hangUpPlatfromFinish?.Invoke();
        playerHangSystem.hangUpPlatfromFinish = null;
    }
    public void HangShimmyEnter()
    {
        playerHangSystem.SignHandleDown();
        playerHangSystem.SingHandleToCllimb();
        playerController.DisableCtrl();
        playerController.ClearIncreaseVelo();
        playerController.ClearMoveSpeed();
        playerController.SetCharacterColliderEnable(false);
        playerController.CtrlEnableGravity(false);
        playerController.ExChangeMoveMode(ActorMoveMode.Shimmy);
        characterAnim.StartAllHandIK();
    }
    public void HangShiwmmyExit()
    {
        playerController.ExChangeMoveMode(ActorMoveMode.Common);
    }
    public void CanelShimmyEnter()
    {
        GetComponent<PlayerHangSystem>().ResetDefault();
        playerController.ExChangeMoveMode(ActorMoveMode.Common);
    }
    public void CanelShimmyUpdate(AnimatorUpdateArgSaver saver, List<AnimationCurve> curves)
    {
        var y = curves[0].Evaluate(saver.stateInfo.normalizedTime);
        playerController.IncreaseVelo += new Vector3(0, y, 0);
    }
    public void CanelShimmyExit()
    {
        characterAnim.SetInt(AnimatorParameter.HangAction.ToString(), 0);
        playerController.DisableCtrl();
        playerController.EnableCtrl();
        playerController.SetCharacterColliderEnable(true);
        playerController.CtrlEnableGravity(true);
        GetComponent<PlayerHangSystem>().ResetDefault();
    }
    public void HandleStandToHang()
    {
        characterAnim.SetInt(AnimatorParameter.HangAction.ToString(),3);
        
    }
    public void StandToHangEnter()
    {
        //characterAnim.SetInt(AnimatorParameter.HangAction.ToString(), 0);
        playerController.DisableCtrl();
        playerController.SetCharacterColliderEnable(false);
        playerController.CtrlEnableGravity(false);
    }
    public void StandToHangUpdate(AnimatorUpdateArgSaver saver)
    {
        
        AnimatorStateInfo stateInfo = saver.animator.GetCurrentAnimatorStateInfo(0);
        var positionLerpPct=1f-Mathf.Exp((Mathf.Log(1f-0.99f)/0.29f)*Time.deltaTime);
      
        transform.position = Vector3.Lerp(transform.position,playerHangSystem.bodyPoint, positionLerpPct);
        if (stateInfo.normalizedTime >= 0.9)
        {
            playerController.deltaRotation = Quaternion.identity;
            var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / 0.1f) * Time.deltaTime);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles,Quaternion.LookRotation(-playerHangSystem.pointNormal).eulerAngles, rotationLerpPct);
        }

    }


    public void StandToHangExit()
    {
        transform.position = playerHangSystem.bodyPoint;
        playerController.EnableCtrl();
        playerController.SetCharacterColliderEnable(true);
        playerController.CtrlEnableGravity(true);
    }
    public void LockStateJumpEnter()
    {
        characterAnim.SetInt(AnimatorParameter.JumpAction.ToString(),0);
        playerController.ClearIncreaseVelo();
        playerController.ClearMoveSpeed();
        playerController.DisableCtrl();
        var forward = characterAnim.anim.GetFloat(AnimatorParameter.Forward.ToString());
        var hor = characterAnim.anim.GetFloat(AnimatorParameter.TurnAngle.ToString());
        if (forward != 0)
        {
            forward = Mathf.Abs(forward) / forward;
        }
        if (hor != 0)
        {
            hor = Mathf.Abs(hor) / hor;
        }

        playerController.targetBlend = forward;
        characterAnim.SetFloat(AnimatorParameter.Forward.ToString(), forward);
        characterAnim.SetFloat(AnimatorParameter.TurnAngle.ToString(), hor);
        playerStateManager.superBody = true;
    }
    public void LockStateJumpExit()
    {
        playerController.EnableCtrl();
        playerStateManager.superBody = false;
    }
    public void LockStateLocomotionEnter()
    {
        playerController.EnableCtrl(false);
    }
    
    public void SetCommonAttack(AnimatorUpdateArgSaver saver,int id)
    {
        weaponManager.currentWeapon.commonAttackID = id;
        Debug.Log("playerAttackID:"+id);
        characterAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
        characterAnim.SetInt(AnimatorParameter.AttackAction.ToString(),id);
    }
    public void DefendEnter()
    {
        //状态机那边去处理了
        //playerController.ClearIncreaseVelo();
        //playerController.ClearMoveSpeed();
        //playerController.EnableCtrl(false);
        //playerController.EnableRotate(false);
        //weaponManager.StartDefense();
    }
    public void DefendExit()
    {
        //状态机那边去处理了
        //playerController.EnableCtrl();
        //weaponManager.CanelDefense();
    }
    public void ReboundEnter()
    {
        playerController.DisableCtrl();
        playerController.ClearMoveSpeed();
        playerController.ClearIncreaseVelo();
        weaponManager.CanelDefense();
        characterAnim.SetInt(AnimatorParameter.DefenseAction.ToString(),0);
      
    }
    public void ReboundExit()
    {
        playerStateManager.isReBound = false;
        playerController.EnableCtrl();
        weaponManager.CanelDefense();
        Debug.Log(playerStateManager.isDenfense);
    }
    public void OpenController()
    {
        playerController.EnableCtrl(false);
    }
    public void LoseAllControllerAndSpeed()
    {
        playerController.DisableCtrl();
        playerController.ClearMoveSpeed();
        playerController.ClearIncreaseVelo();
        characterAnim.SetFloat(AnimatorParameter.Forward.ToString(), 0);
        characterAnim.SetFloat(AnimatorParameter.VeloDir.ToString(), 0);
    }
    public void ClearInputDir()
    {
        playerController.lastInputDir = Vector2.zero;
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
    public void ForwardRollEnter()
    {
        roleController.ClearAllMotionData(true);
        playerStateManager.SuperBody(true);
    }
    public void ForwardRollExit()
    {
        roleController.EnableCtrl();
        playerStateManager.SuperBody(false);
    }

  public void AirComboAttackEnter()
    {
        characterAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
    }
    public void JumpAttack02Enter()
    {
        characterAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
    }
    public void LockModeDodgeEnter()
    {
        characterAnim.SetInt(AnimatorParameter.JumpAction.ToString(),0);
    }
    public override List<AnimatorParameterDataPacker> ReplaceAnimatorBeforePackData()
    {
        List<AnimatorParameterDataPacker> packers = new List<AnimatorParameterDataPacker>();
        packers.Add(new AnimatorParameterDataPacker(AnimatorParameter.LockTarget.ToString(), AnimatorControllerParameterType.Bool, characterAnim.anim));
        return packers;
    }
    /// <summary>
    /// 更换状态机后 去修正数据。
    /// </summary>
    public override void ReplaceAnimatorAfterReviseData(List<AnimatorParameterDataPacker> packers)
    {
        if (packers != null && packers.Count > 0)
        {
            foreach (var arg in packers)
            {
                switch (arg.parameterName)
                {
                    case "LockTarget":
                        characterAnim.SetBool(AnimatorParameter.LockTarget.ToString(), arg.boolValue);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public override void OpenRightArrowPoseRig()
    {
        //开启右手持箭矢姿势 就要关闭右手持武器姿势
        weaponManager.weaponRigList[0].weight = 0;
        weaponManager.weaponRigList[2].weight = 1;
    }
    public override void CloseRightArrowPoseRig()
    {
        weaponManager.weaponRigList[2].weight = 0;
    }
    public  void FromLadderClimbToPlatformEnter()
    {
        characterAnim.CloseAllFeetIK();
        characterAnim.CloseAllHandIK();
    }
    public void FromLadderClimbToPlatformExit()
    {
        playerController.EnableCtrl(true);
        playerController.ExChangeMoveMode(ActorMoveMode.Common);
        playerController.CtrlEnableGravity(true);
        playerController.SetCharacterColliderEnable(true);
        playerHangSystem.hangUpPlatfromFinish?.Invoke();
    }
    public override HpBarEntity ApplyHpBarEntity()
    {
        BasePanel basepanel = UIManager.Instance.TryGetAExistPanel(UIPanelIdentity.GameMain);
        if (basepanel)
        {
            NGUI_GameMainPopup gameMain = basepanel as NGUI_GameMainPopup;
            //gameMain.playerHPBarEntity.UpdateHpBar(this.roleController.damageable);
            return gameMain.playerHPBarEntity;
        }
        return null;
    }
}

