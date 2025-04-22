using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ActorSystem : ActorComponent
{
    //[Header("角色配置信息ID")]
    //public int roleDefinitionID;
    //public RoleObjectDefinition roleDefinition;
    //public RoleData roleData=new RoleData();
    private List<ActorComponent> _actorComponentList=new List<ActorComponent>();
    public List<ActorComponent> actorComponentList{
        get
        {
            if (_actorComponentList==null)
            {
                _actorComponentList = gameObject.GetComponentsInChildren<ActorComponent>(true).ToList();
            }
            return _actorComponentList;
        } 
        set
        {
            _actorComponentList = value;
        }
    }
    private Dictionary<GameObject, List<IActor>> selfRegisterActorComponents = new Dictionary<GameObject, List<IActor>>();//No LifeTime Call
    public override void Awake()
    {
//#if UNITY_EDITOR
//        if (GameRoot.GameState_.IsSelectThisEnumInMult(GameState.NONE) == true)
//        {
//            return;
//        }
//#endif
        ActorComponentAwake();
    }
    public override void ActorComponentAwake()
    {
        //roleDefinition = RoleObjectDefinition.GetRoleDefinition(roleDefinitionID);
        actorComponentList=gameObject.GetComponentsInChildren<ActorComponent>(true).ToList();
        actorSystem = this;
        foreach (var actorComponent in actorComponentList)
        {
            if (this!=actorComponent)
            {
                actorComponent.ActorComponentAwake();
                actorComponent.actorSystem = this;
            }
        }
    }
    public T AddActorComponent<T>(T actorComponent) where T : ActorComponent
    {
        if (actorComponentList.Contains(actorComponent) == false)
        {
            actorComponentList.Add(actorComponent);
            actorComponent.ActorComponentAwake();
            actorComponent.actorSystem = this;
        }
        else
        {
            Debug.LogWarning($"ActorComponent:{actorComponent.GetType()} Have Exist");
        }
        return null;
    }
    public void RemoveActorComponent<T>(T actorComponent,bool immediately=false) where T : ActorComponent
    {
        if (actorComponentList.Contains(actorComponent))
        {
            actorComponent.OnDestory();
            actorComponentList.Remove(actorComponent);
            if (immediately == false)
            {
                Destroy(actorComponent);
            }
            else
            {
                DestroyImmediate(actorComponent);
            }
        }
    }

    public  override T GetActorComponent<T>() 
    {
        foreach (var item in actorComponentList)
        {
            if (item is T)
            {
                return item as T;
            }
        }
        if (selfRegisterActorComponents.Count > 0)
        {
            foreach (var pair in selfRegisterActorComponents)
            {
                List<IActor> actors = pair.Value;
                foreach (var item in actors)
                {
                    if (item is T)
                    {
                        return item as T;
                    }
                }
            }
        }
        return null;
    }
    public  T GetActorComponent<T>(GameObject go) where T : MonoBehaviour,IActor
    {
        foreach (var item in actorComponentList)
        {
            if (item is T&&item.gameObject== go)
            {
                return item as T;
            }
        }
        if (selfRegisterActorComponents.ContainsKey(go))
        {
            List<IActor> actors = selfRegisterActorComponents[go];
            foreach (var item in actors)
            {
                if (item is T)
                {
                    return item as T;
                }
            }
        }
        return null;
    }
    public void RegisterActorObject(GameObject go,IActor iactor)
    {
        if (iactor is ActorComponent)
        {
            return;
        }
        List<IActor> actors = null;
        if (selfRegisterActorComponents.ContainsKey(go))
        {
            actors = selfRegisterActorComponents[go];
        }
        if (actors==null)
        {
            actors = new List<IActor>();
            selfRegisterActorComponents.Add(go,actors);
        }
        else if (actors.Contains(iactor))
        {
            return;
        }
        actors.Add(iactor);
        iactor.actorSystem = this;
    }
}

