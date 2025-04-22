using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SkeletonStateManager : ActorStateManager
{
    public bool isDrawBow{ get => CheckState(ActorStateFlag.isDrawBow.ToString()); set => SetState(ActorStateFlag.isDrawBow.ToString(), value); }
    public override void Init()
    {
        base.Init();
        isJump = false;
        isFall = false;
        IsGround = false;
        isAttack = false;
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
        isDrawBow = false;
    }
    public override void Update()
    {
        base.Update();
        isJump = characterAnim.stateInfo.IsTag("Jump");
        isFall = characterAnim.stateInfo.IsTag("Fall");
        IsGround = moveComponent.IsInGrounded;
        isDrawBow = characterAnim.stateInfo.IsTag("BowAim");
        isAttack = characterAnim.stateInfo.IsTag("Attack")||isDrawBow;
        allowAttack = ((IsGround) && !isJump && !isFall);
        isHit = characterAnim.stateInfo.IsTag("Hit");
        isDie = characterAnim.stateInfo.IsName("Death");
    }
}