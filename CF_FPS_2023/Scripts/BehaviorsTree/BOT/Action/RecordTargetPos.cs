using System;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

[TaskCategory("Bot")]
public class RecordTargetPos:BehaviorDesigner.Runtime.Tasks.Action
{
    public SharedVector3 recordTargetPos;
    public SharedTransform targetPosByTransform;
    public Vector3 targetPos;
    public override TaskStatus OnUpdate()
    {
        if (targetPosByTransform!=null&&targetPosByTransform.Value!=null)
        {
            recordTargetPos.Value = targetPosByTransform.Value.position;
        }
        else
        {
            recordTargetPos.Value = targetPos;
        }
        return TaskStatus.Success;
    }
}

