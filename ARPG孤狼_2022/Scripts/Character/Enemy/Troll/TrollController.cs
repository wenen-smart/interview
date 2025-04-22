using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrollController : EnemyController
{
    public FsmStateMachine<TrollState, TrollTransitionType, TrollController> trollFsmMachine;
    public FsmStateMachine<TrollAttackStateType, TrollAttackTransitionType, TrollController> attackFsmMachine;
    public override void Init()
    {
        base.Init();
        trollFsmMachine = new FsmStateMachine<TrollState, TrollTransitionType, TrollController>("Troll", this, TrollFSMConfig.Instance.FsmStateTypeData);

        //attackFsmMachine = new FsmStateMachine<TrollAttackStateType, TrollAttackTransitionType, TrollController>("TrollACT", this, TrollAttackFSMConfig.Instance.FsmStateTypeData);//TODO Config Data
    }
    public void LateUpdate()
    {
        trollFsmMachine.Tick();
    }
    public int RandomIdleBlend()
    {
        return UnityEngine.Random.Range(1,3);
    }
}
