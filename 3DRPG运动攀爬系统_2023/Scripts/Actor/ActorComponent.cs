using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ActorComponent: IActor
{
    //public ActorStateManager stateManager;
    //public CharacterAnim characterAnim;
    private ActorSystem _actorSystem;
    //[HideInInspector]
    public ActorSystem actorSystem
    {
        get
        {
            if (_actorSystem==null)
            {
                _actorSystem = GetComponent<ActorSystem>()??GetComponentInParent<ActorSystem>();
            }
            return _actorSystem;
        } 
        set
        {
            _actorSystem = value;
        }
    }
    public bool isInitialize { get; private set; } = false;
    public virtual void Awake()
    {

    }
    public virtual void Start()
    {

    }
    public  virtual void ActorComponentAwake()
    {
        
    }
    public override void Init()
    {
        isInitialize = true;
    }
    public virtual void AddActorComponent<T>() where T : ActorComponent
    {
        actorSystem.AddActorComponent<T>(gameObject.AddComponent<T>());
    }
    public static T AddActorComponent<T>(GameObject go) where T : ActorComponent
    {
        ActorSystem tp_actorSystem=go.GetComponentInParent<ActorSystem>(true);
        if (tp_actorSystem==null)
        {
            Debug.LogError($"在{go.name} 上级找不到ActorSystem：");
        }
        else
        {
           return tp_actorSystem.AddActorComponent(go.AddComponent<T>());
        }
        return null;
    }
    public virtual T GetActorComponent<T>() where T : ActorComponent
    {
        return actorSystem.GetActorComponent<T>();
    }
    public static  T GetActorComponentStatic<T>(GameObject go) where T : ActorComponent
    {
        ActorSystem tp_actorSystem=go.GetComponentInParent<ActorSystem>(true);
        if (tp_actorSystem == null)
        {
            Debug.LogError($"在{go.name} 上级找不到ActorSystem：");
            return null;
        }
        return tp_actorSystem?.GetActorComponent<T>(go);
    }
}

