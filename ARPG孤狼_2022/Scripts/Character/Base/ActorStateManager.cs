using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum ActorStateFlag
{
    IsGround,
    isJump,
    isFall,
    allowHangCheck,
    isInTransition,
    isShimmy,
    nextAttackVaildSign,
    waitNextAttackInput,
    allowAttack,
    isAttack,
    isDenfense,
    allowDenfense,
    canReadInput,
    isReBound,
    inDefenseLayer,
    isHit,
    isDie,
    shakeBefore,
    shakeAfter,
    canSaveInputInMotion,
    superBody,
    isBattle,
    airAttack,
    nextAnimIsAttackAnim,
    IsRootState,
    GetDefendState,
    isDrawBow,
    AllowBowStay,
    isSleep,
    allowClimbLadder,
}

public class ActorStateManager: IActorMonoManager
{
    [HideInInspector]
    public ActorPhyiscal actorPhyiscal;
    [HideInInspector]
    public CharacterAnim characterAnim;
    [HideInInspector]
    public CharacterFacade characterFacade;
    public bool nextAttackVaildSign{ get => CheckState(ActorStateFlag.nextAttackVaildSign.ToString()); set => SetState(ActorStateFlag.nextAttackVaildSign.ToString(), value); }
    public bool waitNextAttackInput { get { return nextAttackVaildSign; } set { nextAttackVaildSign = value; } }//退出攻击动画 在连击动画外中等待下次对应连击输入  这里为了统一 最终操作的是NextAttackVaildSign标识符  所以要注意执行完设置为false。
    public bool isJump { get => CheckState(ActorStateFlag.isJump.ToString()); set => SetState(ActorStateFlag.isJump.ToString(), value); }
    public bool isFall{ get => CheckState(ActorStateFlag.isFall.ToString()); set => SetState(ActorStateFlag.isFall.ToString(), value); }

    public bool allowHangCheck{ get => CheckState(ActorStateFlag.allowHangCheck.ToString()); set => SetState(ActorStateFlag.allowHangCheck.ToString(), value); }
    public bool isInTransition{ get => CheckState(ActorStateFlag.isInTransition.ToString()); set => SetState(ActorStateFlag.isInTransition.ToString(), value); }
    public bool isShimmy{ get => CheckState(ActorStateFlag.isShimmy.ToString()); set => SetState(ActorStateFlag.isShimmy.ToString(), value); }//isShimmy==Hang
    public bool allowAttack{ get => CheckState(ActorStateFlag.allowAttack.ToString()); set => SetState(ActorStateFlag.allowAttack.ToString(), value); }
    public bool isAttack{ get => CheckState(ActorStateFlag.isAttack.ToString()); set => SetState(ActorStateFlag.isAttack.ToString(), value); }

    public bool isDenfense{ get => CheckState(ActorStateFlag.isDenfense.ToString()); set => SetState(ActorStateFlag.isDenfense.ToString(), value); }
    public bool allowDenfense{ get => CheckState(ActorStateFlag.allowDenfense.ToString()); set => SetState(ActorStateFlag.allowDenfense.ToString(), value); }
    public bool canReadInput{ get => CheckState(ActorStateFlag.canReadInput.ToString()); set => SetState(ActorStateFlag.canReadInput.ToString(), value); }
    public bool isReBound{ get => CheckState(ActorStateFlag.isReBound.ToString()); set => SetState(ActorStateFlag.isReBound.ToString(), value); }
    public bool inDefenseLayer{ get => CheckState(ActorStateFlag.inDefenseLayer.ToString()); set => SetState(ActorStateFlag.inDefenseLayer.ToString(), value); }
    public bool isHit{ get => CheckState(ActorStateFlag.isHit.ToString()); set => SetState(ActorStateFlag.isHit.ToString(), value); }
    public bool isDie{ get => CheckState(ActorStateFlag.isDie.ToString()); set => SetState(ActorStateFlag.isDie.ToString(), value); }

    public bool shakeBefore{ get => CheckState(ActorStateFlag.shakeBefore.ToString()); set => SetState(ActorStateFlag.shakeBefore.ToString(), value); }//前摇
    public bool shakeAfter{ get => CheckState(ActorStateFlag.shakeAfter.ToString()); set => SetState(ActorStateFlag.shakeAfter.ToString(), value); }//后摇
    public bool canSaveInputInMotion { get => CheckState(ActorStateFlag.canSaveInputInMotion.ToString()); set => SetState(ActorStateFlag.canSaveInputInMotion.ToString(), value); } //能在动画中暂存输入，接收控制，但不在此刻执行动作
    public bool superBody{ get => CheckState(ActorStateFlag.superBody.ToString()); set => SetState(ActorStateFlag.superBody.ToString(), value); }
    public bool isBattle{ get => CheckState(ActorStateFlag.isBattle.ToString()); set => SetState(ActorStateFlag.isBattle.ToString(), value); }
    public bool airAttack{ get => CheckState(ActorStateFlag.airAttack.ToString()); set => SetState(ActorStateFlag.airAttack.ToString(), value); }
    public bool nextAnimIsAttackAnim{ get => CheckState(ActorStateFlag.nextAnimIsAttackAnim.ToString()); set => SetState(ActorStateFlag.nextAnimIsAttackAnim.ToString(), value); }
    public virtual bool GetDefendState() { return false; }

    public bool IsRootState
    {
        get { return characterFacade.buffContainer.IsContainBuff(4); }
        set
        {
            if (value)
            {
                characterFacade.buffContainer.DOBuff(4, this.actorSystem);
            }
            else
            {
                characterFacade.buffContainer.PushBuff(4);
            }
        }
    }

    public bool IsGround
    {
        get => CheckState(ActorStateFlag.IsGround.ToString()); 
        set
        {
            if (StateFlagDic.ContainsKey(ActorStateFlag.IsGround.ToString())==false)
            {
                SetState(ActorStateFlag.IsGround.ToString(), value);
                GroundStateChanagedHandler?.Invoke(value);
            }
            else
            {
                if (IsGround != value)
                {
                    SetState(ActorStateFlag.IsGround.ToString(), value);
                    GroundStateChanagedHandler?.Invoke(value);
                }
            }
            
        }
    }
    [HideInInspector]public MoveComponent moveComponent;
    public Action<bool> GroundStateChanagedHandler;

    public Dictionary<string, bool> StateFlagDic = new Dictionary<string, bool>();
    [HideInInspector]
    public AnimationClip clipthatPreInputEnable;
    public override void Awake()
    {
        base.Awake();
        
    }
    public override void Init()
    {
        base.Init();
        characterAnim = GetComponentInChildren<CharacterAnim>();
        characterFacade = GetActorComponent<CharacterFacade>();
    }
    public void ShakeBefore(bool start)
    {
        if (start)
        {
            shakeBefore = true;
            shakeAfter = false;
        }
        else
        {
            shakeBefore = false;
        }
    }
    public void ShakeAfter(bool start)
    {
        if (start)
        {
            shakeBefore = false;
            shakeAfter = true;
        }
        else
        {
           
            shakeAfter = false;
        }
       
    }
    public void SuperBody(bool enable)
    {
        superBody = enable;
        SwitchReceiverInput(enable);
    }
    public void SwitchReceiverInput(bool input)
    {
        canSaveInputInMotion = input;
        if (input)
        {
            AnimatorClipInfo[] animationClips = characterAnim.anim.GetCurrentAnimatorClipInfo(0);
            if (animationClips!=null&&animationClips.Length>0)
            {
                clipthatPreInputEnable =animationClips[0].clip;
            }
        }
    }

public virtual void Update()
    {
        CheckInPreInputAnimClipIsPlay();
    }
    public bool CheckState(string stateName)
    {
        bool state;
        if (StateFlagDic.TryGetValue(stateName,out state))
        {
            return state;
        }
        MyDebug.DebugWarning("没有对应的状态标识，请检查stateName:"+stateName);
        SetState(stateName,false);
        return false;
    }
    public void SetState(string stateName,bool state)
    {
        if (StateFlagDic.ContainsKey(stateName))
        {
            StateFlagDic[stateName] = state;
        }
        else
        {
            StateFlagDic.Add(stateName,state);
        }
    }
    public bool CheckAnimationStateByName(string stateName)
    {
        return characterAnim.stateInfo.IsName(stateName);
    } 
    public bool CheckAnimationStateByTag(string stateTag)
    {
        return characterAnim.stateInfo.IsTag(stateTag);
    }
    public void CheckInPreInputAnimClipIsPlay()
    {
        if (clipthatPreInputEnable != null)
        {
            AnimatorClipInfo[] animationClips = characterAnim.anim.GetCurrentAnimatorClipInfo(0);
            bool isPlay = false;
            if (animationClips != null && animationClips.Length > 0)
            {
                foreach (var item in animationClips)
                {
                    if (item.clip == clipthatPreInputEnable)
                    {
                        isPlay = true;
                        break;
                    }
                }
            }
            if (isPlay == false)
            {
                clipthatPreInputEnable = null;//清理脏数据
            }
        }
    }
}
/// <summary>
/// 后期如果有性能问题 可以改成当Get时候再去更新状态
/// </summary>
public class ActorStateHandler
{
    public Func<bool> stateUpdateHandler;
    public bool GetState()
    {
        if (stateUpdateHandler == null)
        {
            MyDebug.DebugError("严重错误 更新状态标识符的委托未赋值");
            return false;
        }
        return stateUpdateHandler.Invoke();
    }
}

