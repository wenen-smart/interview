using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class IEventSystem<T,TEventCode,TAction,TArg> : CommonSingleTon<T>, I_Init where T : new() where TEventCode:Enum where TAction:EventHandler<TAction,TArg> where TArg:new()
{
    public Dictionary<TEventCode, TAction> eventDic = new Dictionary<TEventCode, TAction>();
    public abstract void Init();
    public virtual void RegisterEvent(TEventCode eventCode,TAction eventAction)
    {
        TAction action = GetEventAction(eventCode);
        if (action!=null)
        {
            action = eventAction;
            Debug.Log("已经注册事件，覆盖原事件");
            eventDic[eventCode] = action;
            return;
        }
        eventDic.Add(eventCode,eventAction);
    }
    public virtual TAction GetEventAction(TEventCode eventCode)
    {
        TAction eventHandler;
        eventDic.TryGetValue(eventCode, out eventHandler);
        return eventHandler;
    }
    public abstract void RemoveEvent(TEventCode eventCode);
    public abstract void RemoveEvent(TAction eventAction);
    public abstract void ClearAllEvent();
}

