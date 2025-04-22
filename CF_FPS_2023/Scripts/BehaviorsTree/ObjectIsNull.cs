using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[TaskCategory("Objects")]
public class ObjectIsNull : Conditional
{
    public SharedVariable _object;
    public override TaskStatus OnUpdate()
    {
        return _object.GetValue()==null?TaskStatus.Success:TaskStatus.Failure;
    }
}
