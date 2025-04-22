using Buff.Base;
using Buff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BattleManager :CommonSingleTon<BattleManager>,IManager
{
    
    public Dictionary<IDamageable, HpBarEntity> hpbarEntityDics = new Dictionary<IDamageable, HpBarEntity>();
    public static string[] allUNITCampTags = Enum.GetNames(typeof(UnitCamp));
    public void Init()
    {
        Debug.Log("Init BattleManager");
        CoreEventSystem.Instance.RegisterEvent(EventCode.PickedUp, new CoreEventHandler((args) => { OnPickedUp((BuffBase)args.value[0]); }));
        
    }
    public void OnTriggerEnter(Collider other)
    {
        
    }
    public void LoadAllActorInto()
    {
        //campDic.Clear();
        //foreach (var campTag in allUNITCampTags)
        //{
        //    GameObject[] units=GameObject.FindGameObjectsWithTag(campTag);
        //    List<RoleController> roles = new List<RoleController>();
        //    foreach (var unit in units)
        //    {
        //        RoleController role = unit.GetComponent<RoleController>();
        //        if (role!=null)
        //        {
        //            roles.Add(role);
        //        }
        //        else
        //        {
        //            MyDebug.DebugError(unit.gameObject.name+":身上没有挂载RoleController");
        //        }
        //    }
        //    if (roles.Count > 0)
        //    {
        //        campDic.Add(campTag, roles);
        //    }
        //}
    }
    public List<RoleController> FilterUnits(string[] tagList)
    {
        List<RoleController> roles = new List<RoleController>();
        foreach (var tagStr in tagList)
        {
           List<RoleController> camp = MapManager.Instance.FindActors(tagStr);
            if (camp!=null)
            {
                roles.AddRange(camp);
            }
        }
        return roles;
    }
    public void OnPickedUp(BuffBase buff)
    {
        ActorSystem caster = (ActorSystem)buff.caster;
        ActorSystem affectedPerson= (ActorSystem)buff.AffectedPerson;
        Debug.Log(caster?.name+"挑起了"+affectedPerson?.name+"");
        CastForce(caster,affectedPerson,Vector3.up * 8);
        CastForce(caster,caster,Vector3.up * 8);
    }

    public void LoadScript(object args)
    {
        Debug.Log("Load Script.... BattleManager");
    }

    public void CastForce(ActorSystem caster,ActorSystem affector,Vector3 force)
    {
        CoreEventSystem.Instance.RegisterEvent(EventCode.Force, new CoreEventHandler((args) => { OnForceStart((BuffBase)args.value[0],force); }));
        BuffBase buff = affector.GetActorComponent<BuffContainer>().DOBuff(3,caster);
    }
    public void OnForceStart(BuffBase buff,Vector3 force)
    {
        ActorSystem affectedPerson = (ActorSystem)buff.AffectedPerson;
        affectedPerson.GetActorComponent<MoveComponent>().AddImpluseForce(force);
    }
    public void AttackFeetBack(SkillEntity skillEntity,IDamageable hiter)
    {
        if (skillEntity.owner.gameObject.CompareTag("Player"))
        {
            CameraMgr.Instance.GeneralImpusle();
            GameTimeScaleCtrl.Instance.SetTimeSacle(0.1f, 300, 1);
            CameraMgr.Instance.OpenPost_FangsheMohu();
        }
        else
        {
            if (hiter.gameObject.CompareTag("Player"))
            {
                CameraMgr.Instance.GeneralImpusle();
                GameTimeScaleCtrl.Instance.SetTimeSacle(0.1f, 300, 1);
                CameraMgr.Instance.OpenPost_FangsheMohu();
            }
        }
        
    }
    public HpBarEntity GetHpBarEntity(IDamageable damageable)
    {
        HpBarEntity hpBarEntity;
        hpbarEntityDics.TryGetValue(damageable, out hpBarEntity);
        return hpBarEntity;
    }
    public HpBarEntity ApplyHpBarEntity(IDamageable damageable)
    {
        HpBarEntity hpBarEntity;
        hpbarEntityDics.TryGetValue(damageable,out hpBarEntity);
        if (hpBarEntity==null)
        {
            hpBarEntity = GameObjectFactory.Instance.PopItem(PathDefine.UIItemPath+Constants.HpBarEntity).GetComponent<HpBarEntity>();

            if (hpBarEntity!=null)
            {
                hpbarEntityDics.Add(damageable,hpBarEntity);
                hpBarEntity.SetFollowHead(damageable.characterFacade.actorPhyiscal.GetCheckPoint(CheckPointType.Head));
            }
        }

       return hpBarEntity;
    }
    public void RemoveHpBarEntity(IDamageable damageable)
    {
        if (hpbarEntityDics.ContainsKey(damageable))
        {
            HpBarEntity hpBarEntity=hpbarEntityDics[damageable];
            hpBarEntity.SetFollowHead(null);
            hpbarEntityDics.Remove(damageable);
            GameObjectFactory.Instance.PushItem(hpBarEntity.gameObject);
        }
    }
    public bool IsBelongSameUnitCamp(RoleController attacker,RoleController hiter)
    {
        return attacker.MyCampTag.IsSelectThisEnumInMult(hiter.MyCampTag);
    }
    public bool IsBelongSameUnitCamp(IDamageable attacker, IDamageable hiter)
    {
        return IsBelongSameUnitCamp(attacker.GetActorComponent<RoleController>(),hiter.GetActorComponent<RoleController>());
    }
}

