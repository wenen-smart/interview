using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum WolfState
{
    Idle,
    Move,
    Attack
}
public enum WolfTransitionType
{
    FindTarget,
    GoFollow,
    LoseTarget
}
public enum SkeletonState
{
    Idle,
    Move, 
    MeleeAttack,
    RemoteAttack,
    Sleep,
    Patrol
}
public enum SkeletonTransitionType
{
    FollowUpTarget,
    LoseTarget,
    IntoMeleeAttack,
    IntoRemoteAttack,
    CanPatrol,
    LoseMeleeAttackRange,
}
public enum TrollState
{
    Idle,
    Move,
    Attack,
}
public enum TrollTransitionType
{
    FollowUpTarget,
    LoseTarget,
    IntoAttack,
}
public enum TrollAttackStateType
{
    ComboAttack,//普通連擊
}
public enum TrollAttackTransitionType
{
    AttackStart,
}

public enum DefaultStateType
{

}
public enum DefaultTransitionType
{

}
public enum AttackType
{
    None = 0,
    ComboAttack = 1,
    StabAttack = 2,
    ShiftAttack = 3,
    JumpAttack = 4,
    AirComboAttack,
}
public enum AttackTransitionType
{
    IsStabAttack,
    ContinueCombo,
    ShiftAttack,
    JumpAttack,
    AirContinueAttack
}

public enum PlayerStateType
{
    Idle,
    Move,
    Jump,
    Fall,
    Attack,
    Defense,
    Rebound,
    ShiftTeleport,
    Hang,
}

public enum PlayerTransitionType
{
    InputMove = 1,
    Jump = 2,
    NoGround = 3,
    JumpExit = 4,
    InGround = 5,
    IntoComboAttack = 6,
    IntoDefense = 7,
    IntoRebound = 8,
    ChanelDefense = 9,
    AttackFinish = 10,
    StopMove = 11,
    ShiftTeleport = 12,
    HangUp,
    HangFall,
    HangFinish,
    ACTAirContinueAttack = 16,//强制修正

}