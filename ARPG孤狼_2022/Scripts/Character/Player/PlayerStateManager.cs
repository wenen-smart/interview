using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


  public class PlayerStateManager:ActorStateManager
    {

    [HideInInspector]
     public PlayerAnim newPlayerAnim;
    public bool isDrawBow{ get => CheckState(ActorStateFlag.isDrawBow.ToString()); set => SetState(ActorStateFlag.isDrawBow.ToString(), value); }
    public bool AllowBowStay { get => CheckState(ActorStateFlag.AllowBowStay.ToString()); set => SetState(ActorStateFlag.AllowBowStay.ToString(), value); }

    public bool allowClimbLadder{ get => CheckState(ActorStateFlag.allowClimbLadder.ToString()); set => SetState(ActorStateFlag.allowClimbLadder.ToString(), value); }
    
    public  override void Awake()
    {
        newPlayerAnim = characterAnim as PlayerAnim;
        base.Awake();
    }
    public override void Init()
    {
        base.Init();
        IsGround = false;
        isJump = false;
        isFall = false;
        isInTransition = false;
        isShimmy = false;
        nextAttackVaildSign = false;
        waitNextAttackInput = false;
        allowAttack = false;
        isAttack = false;
        isDenfense = false;
        allowDenfense = false;
        canReadInput = false;
        isReBound = false;
        inDefenseLayer = false;
        isHit = false;
        isDie = false;
        shakeBefore = false;
        shakeAfter = false;
        canSaveInputInMotion = false;
        superBody = false;
        isBattle = false;
        airAttack = false;
        nextAnimIsAttackAnim = false;
        isDrawBow = false;
        AllowBowStay = false;
        allowClimbLadder = false;
    }
    public override void Update()
    {
        IsGround= moveComponent.IsInGrounded;
        isJump = characterAnim.stateInfo.IsTag("Jump")|| characterAnim.stateInfo.IsTag("LockStateJump");
        isFall = characterAnim.stateInfo.IsTag("Fall");
        isInTransition = characterAnim.anim.IsInTransition(0);
        airAttack = characterAnim.stateInfo.IsTag("JumpAttack") || characterAnim.stateInfo.IsTag("AirAttack");
        //isControl = (!isJump && !isFall);
        allowHangCheck = isJump&&!IsGround;
        isShimmy = characterAnim.stateInfo.IsTag("Shimmy");
        isHit = characterAnim.stateInfo.IsTag("Hit");
        isDie = characterAnim.stateInfo.IsTag("Die");
        allowAttack =((IsGround|| airAttack) && !isJump&&!isFall&&isShimmy==false);
        isAttack = IsAttackAnim(characterAnim.stateInfo);
        nextAnimIsAttackAnim = isInTransition&&IsAttackAnim(characterAnim.nextStateInfo);
        allowDenfense = IsGround&&isFall==false && characterAnim.stateInfo.IsTag("Attack") == false&& isHit==false&& isAttack==false;
        inDefenseLayer = characterAnim.anim.GetLayerWeight(1) >0;
        isDenfense = newPlayerAnim.defenseStateInfo.IsTag("Defense")&& inDefenseLayer;
        isReBound = newPlayerAnim.defenseStateInfo.IsName("Parry")&& inDefenseLayer;
        isDrawBow = characterFacade.weaponManager.GetWeaponType()==WeaponType.Bow&&characterAnim.anim.GetLayerWeight(3)>0;
        AllowBowStay = characterFacade.roleController.moveMode != ActorMoveMode.ClosetWall&&characterFacade.roleController.moveMode != ActorMoveMode.Shimmy && characterFacade.roleController.isController;
        allowClimbLadder=IsGround&&isShimmy==false;
        base.Update();
    }
    public override bool GetDefendState()
    {
        return isDenfense;//后面要去计算防御是否成功，抵挡掉了多少伤害
    }
    public bool IsAttackAnim(AnimatorStateInfo stateInfo)
    {
        return stateInfo.IsTag("Attack")||stateInfo.IsTag("JumpAttack")||stateInfo.IsTag("AirAttack");
    }
}

