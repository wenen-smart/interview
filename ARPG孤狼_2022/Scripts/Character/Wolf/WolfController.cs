using MalbersAnimations.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfController : NPCController
{
    public FsmStateMachine<WolfState, WolfTransitionType, WolfController> stateMachine;
    public WolfFSMStateConfig stateConfig;

    public override void Init()
    {
        base.Init();
        stateMachine = new FsmStateMachine<WolfState, WolfTransitionType, WolfController>("Wolf",this,stateConfig.FsmStateTypeData);
        GameRoot.Instance.AddFrameTask(5,()=> { SetFollow(PlayerFacade.Instance.roleController);});
    }
    protected override void Update()
    {
        base.Update();
        stateMachine.Tick();
    }
    public override void SetNavMeshTarget(Vector3 target)
    {
        facade.characterAnim.SetInt("State",1);
        moveComponent.SetNavMeshTarget(target);
        //    GetComponentInChildren<MalbersAnimations.Controller.AI.MAnimalAIControl>().SetDestination(target);
    }
    public override void StopMove()
    {
        facade.characterAnim.SetInt("State",0);
        LerpForwardBlend(0);
        LerpAnimationFloatVlaue(AnimatorParameter.Horizontal.ToString(), 0, 1, true);
        moveComponent.StopNavAgent();
        //GetComponentInChildren<MalbersAnimations.Controller.AI.MAnimalAIControl>().Stop();
    }
}
