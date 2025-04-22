using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class EnemyAttackState<EntityT> : IEnemyState<EntityT> where EntityT:EnemyController
{
    public EnemyAttackState(EnemyFsmStateType stateType, EnemyFSMMachine<EntityT> enemyFsm) : base(stateType, enemyFsm)
    {
    }

    public override void Enter(EntityT go, params object[] args)
    {
        go.AttackEnter();
    }

    public override void Excute(EntityT go, params object[] args)
    {
        go.Attack();
        
    }

    public override void Exit(EntityT go, params object[] args)
    {
        base.Exit(go, args);
    }
    public override void OnUpdate(EntityT go, params object[] args)
    {
       
    }
}

