using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyIdleState<EntityT> : IEnemyState<EntityT> where EntityT : EnemyController
{


    public EnemyIdleState(EnemyFsmStateType stateType, EnemyFSMMachine<EntityT> enemyFsm) : base(stateType, enemyFsm)
    {

    }


    public override void Enter(EntityT go, params object[] args)
    {
        base.Enter(go);
        Debug.Log("Enter Idle");
        go.OpenAttackTime();
    }

    public override void Excute(EntityT go, params object[] args)
    {
        
    }

    public override void Exit(EntityT go, params object[] args)
    {
        base.Exit(go, args);
        Debug.Log("Exit Idle");
    }



    public override void OnUpdate(EntityT go, params object[] args)
    {
        go.Idle();
    }

}

