

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityTransform;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Bot")]
public class BotLookAtTarget: Action
{
    [UnityEngine.Tooltip("The GameObject to look at. If null the world position will be used.")]
    public SharedTransform targetLookAt;
    [UnityEngine.Tooltip("Vector specifying the upward direction")]
    public Vector3 worldUp;

    private Transform targetTransform;
    private RobotController bot;
    public override void OnAwake()
    {
        base.OnAwake();
        bot = Owner.GetComponent<ActorComponent>().GetActorComponent<RobotController>();
    }
    public override void OnStart()
    {
        targetTransform = targetLookAt.Value;
    }

    public override TaskStatus OnUpdate()
    {
        if (targetTransform == null)
        {
            Debug.LogWarning("Transform is null");
            return TaskStatus.Failure;
        }

        transform.LookAt(bot.ProjectOnSelfXZPlane(targetTransform.position),Vector3.up);

        return TaskStatus.Success;
    }

    public override void OnReset()
    {
        targetLookAt = null;
        worldUp = Vector3.up;
    }
}
