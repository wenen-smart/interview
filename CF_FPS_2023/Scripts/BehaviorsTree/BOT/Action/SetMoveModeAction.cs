using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections.Generic;

[TaskCategory("Bot")]
public class SetMoveModeAction: BehaviorDesigner.Runtime.Tasks.Action
{
    private RoleController role;
    public ControllerState controllerState;
    public override void OnAwake()
    {
        base.OnAwake();
        role = GetComponent<ActorComponent>().GetActorComponent<RoleController>();
    }
    public override void OnStart()
    {
        base.OnStart();
        role.SetMoveMode(controllerState);
    }
}
