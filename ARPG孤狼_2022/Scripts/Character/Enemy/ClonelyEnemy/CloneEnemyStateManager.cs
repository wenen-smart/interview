using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CloneEnemyStateManager:ActorStateManager
{
    ClonelyEnemyAnim ClonelyEnemyAnim;
    public override void Awake()
    {
        ClonelyEnemyAnim = characterAnim as ClonelyEnemyAnim;
        base.Awake();
    }
    public override void Init()
    {
        base.Init();
        IsGround= false;
    isJump = false;
    isFall= false;
    isInTransition= false;
    isShimmy= false;
    nextAttackVaildSign= false;
    waitNextAttackInput= false;
    allowAttack= false;
    isAttack= false;
    isDenfense= false;
    allowDenfense= false;
    canReadInput= false;
    isReBound= false;
    inDefenseLayer= false;
    isHit= false;
    isDie= false;
    shakeBefore= false;
    shakeAfter= false;
    canSaveInputInMotion= false;
    superBody= false;
    isBattle= false;
    airAttack= false;
    nextAnimIsAttackAnim= false;
    }

    public override void Update()
    {
        IsGround = actorPhyiscal.IsGround;
        isJump = characterAnim.stateInfo.IsTag("Jump") || characterAnim.stateInfo.IsTag("LockStateJump");
        isFall = characterAnim.stateInfo.IsTag("Fall");
        isInTransition = characterAnim.anim.IsInTransition(0);
        airAttack = characterAnim.stateInfo.IsTag("JumpAttack") || characterAnim.stateInfo.IsTag("AirAttack");
        //isControl = (!isJump && !isFall);
        allowHangCheck = isJump && !isFall && !IsGround;
        isShimmy = characterAnim.stateInfo.IsTag("Shimmy");
        isHit = characterAnim.stateInfo.IsTag("Hit");
        isDie = characterAnim.stateInfo.IsTag("Die");
        allowAttack = ((IsGround || airAttack) && !isJump && !isFall && isShimmy == false);
        isAttack = characterAnim.stateInfo.IsTag("Attack") || characterAnim.stateInfo.IsTag("JumpAttack") || characterAnim.stateInfo.IsTag("AirAttack");
        allowDenfense = IsGround && isFall == false && characterAnim.stateInfo.IsTag("Attack") == false && isHit == false && isAttack == false;
        inDefenseLayer = characterAnim.anim.GetLayerWeight(1) > 0;
        //isDenfense = ClonelyEnemyAnim.defenseStateInfo.IsTag("Defense") && inDefenseLayer;
        //isReBound = ClonelyEnemyAnim.defenseStateInfo.IsName("Parry") && inDefenseLayer;
        base.Update();
    }
}

