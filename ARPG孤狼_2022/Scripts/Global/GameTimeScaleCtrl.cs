using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameTimeScaleCtrl:InstanceMono<GameTimeScaleCtrl>
{
    int tid = -1;
    /// <summary>
    /// first int is InstanceID  second int is tid
    /// </summary>
    public Dictionary<int,int> moveComponentTidDic = new Dictionary<int,int>();

    public static float GetTimeScale()
    {
        return Time.timeScale;
    }
    public static float CalculateTimeDurationInGame(float lifeTimeDuration)
    {
        return lifeTimeDuration * GetTimeScale();
    }
    public void SetTimeSacle(float startScale,float delayms,float endScale,Action finishAction=null)
    {
        Time.timeScale = startScale;
        GameRoot.Instance.DelTimeTask(tid);
        GameRoot.Instance.AddTimeTask(delayms,()=> { Time.timeScale = endScale;finishAction?.Invoke(); });
        //PETime 时间计算不是基于Time.deltaTime 所以TimeScale不会影响任务计时
    }
    public void SetAroundVelocityMult(MoveComponent moveComponent,float multSpeed)
    {
        moveComponent.SetAllDirSpeedMult(multSpeed);
    }
    public void SetAroundVelocityMult(MoveComponent moveComponent,float multSpeed,float delayms,float endSpeed,Action finishAction=null)
    {
        if (moveComponentTidDic.ContainsKey(moveComponent.GetInstanceID()))
        {
            int tid = moveComponentTidDic[moveComponent.GetInstanceID()];
            GameRoot.Instance.DelTimeTask(tid);
        }
        SetAroundVelocityMult(moveComponent,multSpeed);
       int newTid = GameRoot.Instance.AddTimeTask(delayms,()=> {SetAroundVelocityMult(moveComponent,endSpeed);finishAction?.Invoke(); });
        if (moveComponentTidDic.ContainsKey(moveComponent.GetInstanceID()))
        {
            moveComponentTidDic[moveComponent.GetInstanceID()] = newTid;
        }
        else
        {
            moveComponentTidDic.Add(moveComponent.GetInstanceID(),newTid);
        }
    }
    public void SetAroundVelocityMult(params MoveComponentSpeedDotween[] args)
    {
        if (args==null)
        {
            return;
        }
        foreach (var item in args)
        {
            SetAroundVelocityMult(item.moveComponent,item.startSpeedMult,item.delayMs,item.endSpeedMult,item.finishAction);
        }
    }
}

public class MoveComponentSpeedDotween
{
    public MoveComponent moveComponent;
    public float startSpeedMult;
    public float endSpeedMult;
    public float delayMs;
    public Action finishAction;

    public MoveComponentSpeedDotween(MoveComponent moveComponent, float startSpeedMult, float endSpeedMult, float delayMs)
    {
        this.moveComponent = moveComponent;
        this.startSpeedMult = startSpeedMult;
        this.endSpeedMult = endSpeedMult;
        this.delayMs = delayMs;
    }
}