using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public enum EnemyFsmStateType
{
    Idle,
    Patrol,
    Tracking,
    Attack,
    Dodging,
    RunAway
}

public enum EnemyTransitionType
{
    Move,
    InAttackRange,
    SkillCDWait
}
[Serializable]
public class EnemyFSMMachine<EntityT> : FsmStateMachine<EnemyFsmStateType, EnemyTransitionType, EntityT> where EntityT:class
{
    
    public EnemyFSMMachine(string masterName, EntityT entity, FsmStateData<EnemyFsmStateType, EnemyTransitionType>[] fsmStaterDatas) : base(masterName,entity, fsmStaterDatas)
    {

    }
}

