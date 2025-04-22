using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class IEnemyState<EntityT> : IState<EnemyFsmStateType, EnemyTransitionType,EntityT> where EntityT:class
{
    public EnemyFSMMachine<EntityT> enemyFsm;


    protected IEnemyState(EnemyFsmStateType stateType, EnemyFSMMachine<EntityT> enemyFsm) : base(stateType, enemyFsm)
    {
        this.enemyFsm = enemyFsm;
    }
}

