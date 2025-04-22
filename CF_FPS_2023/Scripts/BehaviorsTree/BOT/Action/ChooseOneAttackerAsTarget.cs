using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Bot")]
public class ChooseOneAttackerAsTarget:Action
{
    RobotController bot;
    public SharedTransform ToMeAttacker;
    public SharedFloat canViewDis;
    public float bodyCenterHeight = 1.6f;
    public override void OnAwake()
    {
        base.OnAwake();
        bot = Owner.GetComponent<ActorComponent>().GetActorComponent<RobotController>();
    }

    public override TaskStatus OnUpdate()
    {
        Transform newTarget = null;
        Transform caster = bot.justDamageInfo.caster.transform;
        if (bot.behaviorTreeTarget==null)
        {
            //当前没有在追击人。
            if (IsSatisfy(caster))
            {
                newTarget = caster;
            }
        }
        else
        {
            //如果当前正在追击人
            if (IsSatisfy(caster))
            {
                var curToMe = bot.GetPathDistance(bot.behaviorTreeTarget.position,SearchNearbyMethod.StraightLine);
                var casterToMe =  bot.GetPathDistance(caster.position,SearchNearbyMethod.StraightLine);
                if (curToMe>casterToMe)
                {
                    //更换目标
                    newTarget = caster;
                }
            }

        }
        if (newTarget)
        {
            ToMeAttacker.Value = newTarget;
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;//
    }
    public bool IsSatisfy(Transform enemy)
    {
        var offset = enemy.position - transform.position;
        var dir = offset.normalized;
        var dis = offset.magnitude;
        Vector3 start = transform.position + Vector3.up * bodyCenterHeight;
        Vector3 end = enemy.position + Vector3.up * bodyCenterHeight;
        if (dis<=canViewDis.Value)
        {
            int hitScansMask = ~(/*1 << LayerMask.NameToLayer("FullBody") |*/ (1 << 2));
            if (Physics.Linecast(start, end, out RaycastHit hitinfo, hitScansMask, QueryTriggerInteraction.Ignore))
            {
                //追击
                ActorComponent actorComponent = hitinfo.collider.GetComponent<ActorComponent>();
                if (actorComponent)
                {
                    RoleController roleController = actorComponent.GetActorComponent<RoleController>();
                    if (enemy == roleController.transform)
                    {
                        //追击
                        return true;
                    }
                }
            }
        }
        return false;
    }
}

