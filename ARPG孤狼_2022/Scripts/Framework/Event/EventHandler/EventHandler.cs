using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public delegate void AEvent<T>(T arg1);
public delegate void AEvent();
public interface IEventHandler<T, TArgs> where TArgs : new() where T : IEventHandler<T, TArgs>
{
    bool AddListener(AEvent<TArgs> _event);
    void RemoveListener(AEvent<TArgs> buffEvent);
    bool AddListener(IEventHandler<T, TArgs> v1);
    void RemoveListener(IEventHandler<T, TArgs> v1);
    void RemoveAllEvent();
    void Invoke(TArgs args);
    List<IEventHandler<T, TArgs>> GetEventHandlers();
    public bool HaveListener { get; }
    public int GetListenerCount { get; }
}

public class EventHandler<T,TArgs>: IEventHandler<T, TArgs> where TArgs:new() where T: EventHandler<T, TArgs>,IEventHandler<T, TArgs>
{
    private List<AEvent<TArgs>> Events = new List<AEvent<TArgs>>();
    private List<IEventHandler<T, TArgs>> EventHandlers = new List<IEventHandler<T, TArgs>>();
    public EventHandler(AEvent<TArgs> _event)
    {
        AddListener(_event);
    }
    public static T operator +(EventHandler<T, TArgs> v1, AEvent<TArgs> v2)
    {
        v1.AddListener(v2);
        return v1 as T;
    }
    public static T operator +(EventHandler<T, TArgs> v1, EventHandler<T, TArgs> v2)
    {
        v1.AddListener(v2);
        return v1 as T;
    }
    public static T operator -(EventHandler<T, TArgs> v1 , AEvent<TArgs> v2)
    {
        v1.RemoveListener(v2);
        return v1 as T;
    }
    public static T operator -(EventHandler<T, TArgs> v1, EventHandler<T, TArgs> v2)
    {
        v1.RemoveListener(v2);
        return v1 as T;
    }
    public virtual bool AddListener(AEvent<TArgs> _event)
    {
        if (Events.Contains(_event))
        {
            Debug.LogWarning("重复添加，已过滤，请注意代码隐患");
            return false;
        }
        Events.Add(_event);
        return true;
    }
    public virtual void RemoveListener(AEvent<TArgs> buffEvent)
    {
        if (Events.Contains(buffEvent))
        {
            Events.Remove(buffEvent);
        }
    }
    public virtual bool AddListener(IEventHandler<T, TArgs> v1)
    {
        if (EventHandlers.Contains(v1))
        {
            Debug.LogWarning("重复添加，已过滤，请注意代码隐患");
            return false;
        }
        if (v1.GetEventHandlers().Contains(this))
        {
            Debug.LogError("两个EventHandler将要发生互相引用，会发生事件的无限循环，此操作无效");
            return false;
        }
        EventHandlers.Add(v1);
        return true;
    }
    public virtual void RemoveListener(IEventHandler<T, TArgs> v1)
    {
        if (EventHandlers.Contains(v1))
        {
            EventHandlers.Remove(v1);
        }
    }
    public virtual void RemoveAllEvent()
    {
        Events.Clear();
        EventHandlers.Clear();
    }
    public virtual void Invoke(TArgs args)
    {
        foreach (var buffEvent in Events)
        {
            buffEvent.Invoke(args);
        }
        foreach (var eventHandler in EventHandlers)
        {
            eventHandler.Invoke(args);
        }
    }

    public List<IEventHandler<T, TArgs>> GetEventHandlers() 
    {
        return EventHandlers;
    }

    public bool HaveListener { get { return GetListenerCount > 0; } }
    public int GetListenerCount { get { return Events.Count; } }
}
public class EventHandler<TArgs> where TArgs : new() 
{
    private List<AEvent<TArgs>> Events = new List<AEvent<TArgs>>();
    public EventHandler(AEvent<TArgs> _event)
    {
        AddListener(_event);
    }
    public static EventHandler<TArgs> operator +(EventHandler<TArgs> v1, AEvent<TArgs> v2)
    {
        v1.AddListener(v2);
        return v1;
    }
    public static EventHandler<TArgs> operator -(EventHandler<TArgs> v1, AEvent<TArgs> v2)
    {
        v1.RemoveListener(v2);
        return v1;
    }
    public virtual void AddListener(AEvent<TArgs> _event)
    {
        if (Events.Contains(_event))
        {
            Debug.LogWarning("重复添加，已过滤，请注意代码隐患");
            return;
        }
        Events.Add(_event);
    }
    public virtual void RemoveListener(AEvent<TArgs> buffEvent)
    {
        if (Events.Contains(buffEvent))
        {
            Events.Remove(buffEvent);
        }
    }
    public virtual void RemoveAllEvent()
    {
        Events.Clear();
    }
    public virtual void Invoke(TArgs args)
    {
        foreach (var buffEvent in Events)
        {
            buffEvent.Invoke(args);
        }
    }
    public bool HaveListener { get { return GetListenerCount > 0; } }
    public int GetListenerCount { get { return Events.Count; } }
}
