using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEditor;

[TaskCategory("Bot")]
public class EnemyIsInViewCondition : Conditional
{
    RobotController botController;
    public SharedFloat checkViewAngle;
    public SharedFloat checkViewDis;
    //public SharedVector3 recordTargetPos;
    public SharedTransform sharedTarget;
    public float bodyCenterHeight;

    public override void OnAwake()
    {
        base.OnAwake();
        botController = Owner.GetComponent<ActorComponent>().GetActorComponent<RobotController>();
    }
    public override TaskStatus OnUpdate()
    {
        var enemyCamp = (botController.unit == UnitCamp.A ? UnitCamp.B : UnitCamp.A);
        List<RoleController> enemys = MapManager.Instance.FindActors(enemyCamp.ToString());
        Transform target = null;
        if (enemys != null)
        {
            for (int i = 0; i < enemys.Count; i++)
            {
                var enemy = enemys[i];
                if (enemy.actorHealth.isDeath)
                {
                    continue;
                }
                var lookEnemyVec = enemy.transform.position - transform.position;
                float angle = Vector3.Angle(lookEnemyVec, transform.forward);
                if (angle <= checkViewAngle.Value && lookEnemyVec.magnitude <= checkViewDis.Value)
                {
                    //在检测范围内
                    //检测是否可见
                    int hitScansMask = ~(/*1 << LayerMask.NameToLayer("FullBody") |*/ (1 << 2));
                    Vector3 enemyEyePosition = enemy.transform.position;
                    enemyEyePosition.y = enemy.EyePosition.y;
                    Vector3 start = botController.EyePosition;
                    Vector3 end = enemyEyePosition;
                    Vector3 dir = (end - start).normalized;
                    //从碰撞体外发送射线，防止检测自身。不过经过研究从碰撞体内部发送射线 不会检测碰撞本身。
                    //不过要注意如果发送的射线的位置有碰撞体而且碰撞体会局部位移的的话，要注意了。比如骨骼上的碰撞体。
                    //start += dir * (botController.characterRadius+0.01f);
                    //DebugTool.DrawLine(start, end, Color.yellow, 5);
                    if (Physics.Linecast(start,end,out RaycastHit hitinfo, hitScansMask, QueryTriggerInteraction.Ignore))
                    {
                        //追击
                        ActorComponent actorComponent = hitinfo.collider.GetComponent<ActorComponent>();
                        if (actorComponent)
                        {
                            RoleController roleController = actorComponent.GetActorComponent<RoleController>();
                            if (enemy == roleController)
                            {
                                //追击
                                target = enemy.transform;
                                //DebugTool.DrawLine(start, target.position, Color.green, 5);
                            }
                        }
                    }
                }
            }
        }
        
        sharedTarget.Value = target;
        //Owner.SetVariable("target",sharedTarget);
        if (target!=null)
        {
            botController.GetTeamForEditor().FindTarget(botController,target.position);
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
    #if UNITY_EDITOR
     public override void OnDrawGizmos()
    {
        if (Selection.activeTransform==Owner.transform)
        {
            Handles.color = new Color(0,1,0,0.05f);
            Handles.DrawSolidArc(Owner.transform.position,Owner.transform.up,Owner.transform.forward,-checkViewAngle.Value,checkViewDis.Value);
            Handles.DrawSolidArc(Owner.transform.position,Owner.transform.up,Owner.transform.forward,checkViewAngle.Value,checkViewDis.Value);
        }
        
    }
    #endif
}
