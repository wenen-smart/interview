using Buff.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RayFire;

public abstract class IWeapon:ItemObject
    {
    public CalObjMotionVec calObjMotionVec;
    public List<WeaponCheckPoint> checkPointList;
    public LayerMask canAttackLayers;
    public int baseDamage;
    [HideInInspector]
    public bool enableAttack;
    [HideInInspector]
    public List<IDamageable> lastAttackTarget=new List<IDamageable>();
    [HideInInspector]
    public WeaponManager weaponManager;
    public GameObject unEquipGo;
    public Action<IWeapon> weaponEquip;//指装备武器这一代码行为，与执行动画同时执行 不需要配置动画Event 
    public Action<IWeapon> weaponUnEquip;//指卸下武器这一代码行为，与执行动画同时执行  不需要配置动画Event
    public Action weaponUnEquipAndUnTake;//实际动画 收武器的那一刻触发的委托 需要配置动画Event
    public Action weaponEquipAndTake;//实际动画 拿武器 拔出来的那一刻触发的委托 需要配置动画Event
    [HideInInspector]
    public MeshRenderer[] meshRenderer;
    public WeaponDataConfig weaponData;
    [HideInInspector]
    public int commonAttackID = 0;
    public override void Awake()
    {
        base.Awake();
        meshRenderer = GetComponentsInChildren<MeshRenderer>();
    }
    public virtual void EnableAttack()
    {
        enableAttack = true;
        Debug.Log("EnableAttack");
        //weaponManager.attackedTarget = null;
    }
    public virtual void DisableAttack()
    {
        enableAttack = false;
        lastAttackTarget.Clear();
    }

    public virtual void OpenAttackCollider()
    {

    }

    public virtual void CloseAttackCollider()
    {

    }

    public virtual void SaveTarget(IDamageable damageable)
    {
        weaponManager.diedTarget = null;
        weaponManager.attackedTarget = damageable;
        if (damageable.IsDie)
        {
            weaponManager.diedTarget = damageable;
        }
        weaponManager.soonAttackedTarget = null;
        weaponManager.soonDiedTarget = null;
    }
    public abstract Vector3 GetAttackDir();
    //public abstract void DoDamage();
    public abstract bool DoDamage(IDamageable damageable,Vector3 attackPoint,Vector3 _attackDir=default(Vector3));
    public virtual void TriggerEnemyDefense(Collider other)
    {

    }
    //public abstract void Attack<T>(T entity) where T:RoleController;


    public virtual void ComboAttack()
    {

    }
    public virtual void AirComboAttack()
    {

    }

    public virtual void StabAttack()
    { 
    }
    public virtual void ShiftComboAttack()
    {

    }
    /// <summary>
    /// 伴随装备武器动画开始  不知道何时显示武器 所以不激活物体
    /// </summary>
    /// <param name="immediately"></param>
    public virtual void Equip(bool immediately)
    {
        weaponEquip?.Invoke(this);
    }
    /// <summary>
    /// 伴随着卸下武器结束 所以可以调用隐藏武器
    /// </summary>
    /// <param name="immediately"></param>
    public virtual void UnEquip(bool immediately)
    {
        weaponManager.facade.playerStateManager.characterAnim.SetInt(AnimatorParameter.Equipment.ToString(), 0);
        weaponUnEquip?.Invoke(this);
        SetActive(false);
    }
    public virtual void SetActive(bool enable)
    {
        gameObject.SetActive(enable);
    }
    public virtual void ShowRenderer()
    {
        foreach (var item in meshRenderer)
        {
            item.enabled = true;
        }
    }
    public virtual void HideRenderer()
    {

        foreach (var item in meshRenderer)
        {
            item.enabled = false;
        }
    }

    public virtual void EquipWeaponInRendererView()
    {
        ShowRenderer();
        Transform effect = transform.Find("Effect");
        if (effect)
        {
            foreach (Transform item in effect)
            {
                item.gameObject.SetActive(true);
            }
        }
        if (unEquipGo)
        {
            unEquipGo?.SetActive(false);
        }
        
    }
    public virtual void UnEquipWeaponInRendererView()
    {
        HideRenderer();
        Transform effect = transform.Find("Effect");
        if (effect)
        {
            foreach (Transform item in effect)
            {
                item.gameObject.SetActive(false);
            }
        }
        if (unEquipGo)
        {
            unEquipGo?.SetActive(true);
        }
    }
    public virtual void OnDefense()
    {

    }
    public MotionModifyBuff TrackDamageableTarget(IDamageable target)
    {
       return weaponManager.facade.roleController?.TrackDamageableTarget(target);
    }
    public void Update()
    {
        
    }
    public bool CheckAttackTrigger(SkillEntity skillEntity,Action deploySkill)
    {
        bool attackVaild = false;
        //检测开关
        if (enableAttack && weaponManager.triggerCheck)
        {
            //检查点  -》属于一改武器的多个检查点，应该集中处理
            List<AttackCheckPointResult> attackResult = new List<AttackCheckPointResult>();
            foreach (var item in checkPointList)
            {
                Collider[] attacked = Physics.OverlapSphere(item.checkPoint.position, item.radius);
                AttackCheckPointResult attackCheckPointResult = new AttackCheckPointResult();
                attackCheckPointResult.attackPoint = item.checkPoint.position;
                attackCheckPointResult.attackedList = attacked;
                attackResult.Add(attackCheckPointResult);
            }
            attackVaild=AttackTriggerObject(attackResult,skillEntity,deploySkill);
        }
        return attackVaild;
    }

    public bool AttackTriggerObject(List<AttackCheckPointResult> attackResults,SkillEntity skillEntity,Action deploySkill)
    {
        bool attackVaild = false;
        //检测到对象
        if (attackResults != null && attackResults.Count > 0)
        {
            foreach (var attackResult in attackResults)
            {

                Collider[] attackedList = attackResult.attackedList;
                foreach (var atc in attackedList)
                {
                    //是否属于攻击对象
                    if ((1 << atc.gameObject.layer & (canAttackLayers.value)) != 0)
                    {
                        //是否有生命系统
                        IDamageable damageable = atc.transform.GetComponentInParent<IDamageable>();
                        if (damageable && damageable.IsDie == false&&BattleManager.Instance.IsBelongSameUnitCamp(skillEntity.owner.GetActorComponent<IDamageable>(),damageable)==false)
                        {
                            bool isCanDamaged = false;
                            isCanDamaged = (skillEntity.DamagedTimeIsOver(damageable));
                            if (isCanDamaged)
                            {
                                if (skillEntity.IsCanAttackGroup() == false)
                                {
                                    if (skillEntity.CurrentIsHaveDamageAnyObject())
                                    {
                                        return false;
                                    }
                                }
                                skillEntity.RecordDamaged(damageable);//记录伤害时间
                                attackVaild = AttackEnemy(damageable, deploySkill,attackResult.attackPoint);
                                BattleManager.Instance.AttackFeetBack(skillEntity,damageable);
                            }
                        }
                        else
                        {
                            AttackCanFragmentItem(atc,attackResult.attackPoint);
                        }
                    }
                }
            }
        }
        return attackVaild;
    }

    public bool AttackEnemy(IDamageable damageable,Action deploySkill,Vector3 attackPoint)
    {
        bool attackVaild = false;
        if (damageable)
        {
            var stateManager = damageable.GetComponentInParent<ActorStateManager>();
            
            //防御状态
            if (stateManager != null && (stateManager.isDenfense || stateManager.isReBound))
            {

            }
            else
            {
                weaponManager.soonAttackedTarget = damageable;
                //非防御状态
                if (damageable.hp <= baseDamage)
                {
                    weaponManager.soonDiedTarget = damageable;
                    //damageable.Die();
                    //After Need to Die
                    if (damageable.characterFacade.playerStateManager != null && damageable.characterFacade.playerStateManager.superBody == false)
                    {
                        deploySkill?.Invoke();
                        DoDamage(damageable,attackPoint);
                    }
                }
                else
                {
                    if (damageable.characterFacade.playerStateManager != null && damageable.characterFacade.playerStateManager.superBody == false)
                    {
                        deploySkill?.Invoke();
                        DoDamage(damageable,attackPoint);
                    }
                }
                SaveTarget(damageable);
                attackVaild=true;
            }
        }
        return attackVaild;
    }


    public static bool AttackCanFragmentItem(Collider atc,Vector3 point,float basedamage=50)
    {
        RaggedBox raggedBox =atc.GetComponent<RaggedBox>();
        if (raggedBox!=null)
        {
            RayfireRigid rayfireRigid = atc.GetComponent<RayfireRigid>();
            if (rayfireRigid != null)
            {
                float damage = UnityEngine.Random.Range(1, 3) * basedamage;
                float previewhp = rayfireRigid.damage.maxDamage - (rayfireRigid.damage.currentDamage + damage);
                if (previewhp <= 0)
                {
                    rayfireRigid.physics.mass = 1;
                    atc.GetComponent<Rigidbody>().mass = 1;
                    raggedBox.DropItem();
                }
                rayfireRigid.ApplyDamage(damage, point, 1);

                return true;
            }
        }
        return false;
    }
    public void AttackNoEmemy()
    {

    }
    public virtual IWeaponConsumable FillConsumables()
    {
        return null;
    }
    public virtual void UnEquipConsumable()
    {

    }
}
public class AttackCheckPointResult
{
    public Vector3 attackPoint;
    public Collider[] attackedList;
}

