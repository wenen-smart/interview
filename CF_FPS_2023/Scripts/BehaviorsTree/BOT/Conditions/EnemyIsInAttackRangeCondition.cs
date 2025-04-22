using Assets.Resolution.Scripts.Weapon;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[TaskCategory("Bot")]
public class EnemyIsInAttackRangeCondition:Conditional
{
    RobotController botController;
    public SharedTransform target;
    public override void OnAwake()
    {
        base.OnAwake();
        botController = Owner.GetComponent<ActorComponent>().GetActorComponent<RobotController>();
    }
    public override void OnStart()
    {
        base.OnStart();
    }
    public override TaskStatus OnUpdate()
    {
        BaseWeapon baseWeapon = botController.RuntimeInventory.currentWeapon;
        if (Vector3.Distance(target.Value.position,transform.position)<=baseWeapon.maxAttackDistance)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
