using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MoveToPosition_MotionClip:MotionModifyClip
    {
    public override void OnTickClip(float deltaTime)
    {
        base.OnTickClip(deltaTime);
        if (tickedCount == 0)
        {
            targetDis = _goalPos - buff.AffectedPerson.GetActorComponent<MoveComponent>().POSITION;
        }
        tickedCount++;
        currentTime += deltaTime;
        //var time = targetDis / data.motionSpeed;
        switch (data.variableType)
        {
            case MotionVariableType.Time:
                if (Mathf.Abs(data.targetTime - currentTime) < deltaTime)
                {
                    OnFinish();
                }
                break;
            case MotionVariableType.Speed:
                currentPos = Vector3.MoveTowards(currentPos, _goalPos, data.motionSpeed*GetAllDirSpeedMult()* Time.deltaTime);
                if (Mathf.Abs(targetDis.magnitude/data.motionSpeed - currentTime) < deltaTime)
                {
                    OnFinish();
                }
                break;
            default:
                break;
        }
    }

    
}

