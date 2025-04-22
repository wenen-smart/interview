using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrollStateManager : ActorStateManager
{
    public override void Init()
    {
        base.Init();
        isJump = false;
        isFall = false;
        IsGround = false;
        isAttack =false;
        allowAttack = false;
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
    }
    public override void Update()
    {
        base.Update();
        isJump = characterAnim.stateInfo.IsTag("Jump");
        isFall = characterAnim.stateInfo.IsTag("Fall");
        IsGround = moveComponent.IsInGrounded;
        isAttack = characterAnim.stateInfo.IsTag("Attack");
        allowAttack=((IsGround) && !isJump&&!isFall);
        isHit = characterAnim.stateInfo.IsTag("Hit");
        isDie = characterAnim.stateInfo.IsName("Death");
    }
}
