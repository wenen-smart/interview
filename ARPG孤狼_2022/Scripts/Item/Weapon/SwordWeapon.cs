using Buff.Base;
using Buff.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SwordWeapon:IWeapon
    {

    
    public Animator anim;
   
    public override void Awake()
    {
        //calObjMotionVec.CloseAndRenew();
        //weaponManager.endSkillFunc += () => { return weaponManager.attackedTarget;};
        base.Awake();
    }
    public void OnDrawGizmos()
    {
        foreach (var item in checkPointList)
        {
            Gizmos.DrawWireSphere(item.checkPoint.position,item.radius);
        }
    }

    public override bool DoDamage(IDamageable damageable,Vector3 attackPoint,Vector3 _attackDir=default(Vector3))
    {
        if (damageable&&lastAttackTarget.Contains(damageable)==false)
        {
            lastAttackTarget.Add(damageable);
            Vector3 attackDir = _attackDir;
            if (attackDir==Vector3.zero)
            {
                attackDir = GetAttackDir();
            }
            MyDebug.DebugWireSphere(new Vector3[] { transform.position+attackDir * 10 }, 0.5f, Color.yellow, 5);
            MyDebug.DebugLine(transform.position,  transform.position+attackDir * 10, Color.blue, 5);
            damageable.Hit(baseDamage, attackDir,weaponManager.facade.transform,weaponManager.facade.skillManager.currentSkillEntity.skillData,attackPoint);
            return true;
        }
        return false;
    }
    public override Vector3 GetAttackDir()
    {
      return calObjMotionVec?.CalculateCurrentMotionVec()??Vector3.zero;
    }
    public override void EnableAttack()
    {
        base.EnableAttack();
        //calObjMotionVec.enabled = true;
    }
    public override void DisableAttack()
    {
        base.DisableAttack();
        //calObjMotionVec.CloseAndRenew();
    }

    public void OnTriggerEnter(Collider other)
    {
  
    }
    public int defenseValue=3;
    public override void TriggerEnemyDefense(Collider other)
    {
            Debug.Log(other.gameObject.transform.parent.parent.name);
            ActorStateManager stateManager = other.GetComponentInParent<ActorStateManager>();
            //ActorStateManager enemyStateManager = other.GetComponentInParent<ActorStateManager>();
            var enemyWeapon = GetComponentInParent<IWeapon>();
            if (enemyWeapon != null)
            {
                Debug.Log("enemyWeapon != null ");
                if (enemyWeapon.enableAttack)
                {
                    Debug.Log("enableAttack ");
                    if (stateManager.isDenfense && stateManager.isReBound == false&&stateManager.inDefenseLayer)
                    {
                        Debug.Log("defense hit ");
                   
                    //消耗防御能力 

                    //防御能力 无了 //进入OnDamage
                   
                    defenseValue--;
                    if (defenseValue<=0)
                    {
                        DoDamage(stateManager.GetComponent<IDamageable>(),Vector3.zero);
                        stateManager.characterAnim.SetLayerWeight(1, 0, true);
                        stateManager.GetComponent<RoleController>().OnHit(2, 0, 1);
                    }
                    else
                    {
                        stateManager.GetComponent<RoleController>().OnHit(1, 0, 0);
                    }
                        //stateManager.playerAnim.SetInt();
                    }
                    else if (stateManager.isReBound == true)
                    {
                    var currentRole = GetComponentInParent<RoleController>();
                    if (currentRole.GetComponent<ActorStateManager>().isHit==false)
                    {
                        currentRole.OnHit(100, 0, 0);
                        Debug.Log("reoubou1");
                    }
                   
                }
                }
            }
    }
    
    public override void ComboAttack()
    {
        Debug.Log("Combo Attack");
        RoleController entity = weaponManager.facade.roleController;
        bool attack = false;
        //if (entity.stateManager.nextAttackVaildSign)
        //{
        //    //if (playerFacade.weaponManager.attackedTarget)
        //    //{
        //    //    EndSkillTimeLineManager.Instance.SetEndSkillTimeLine(this, playerFacade.weaponManager.attackedTarget.GetComponentInParent<RoleController>(),EndKillTimeLineConfig.Instance.specialTimeLineConfigs[0].playerableAssets[0]);
        //    //    stateManager.nextAttackVaildSign = false;
        //    //    return;
        //    //}
        //    //playerAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
        //    //playerAnim.SetTrigger(AnimatorParameter.Attack.ToString(), true);

        //    attack = true;
        //}
        //else
        //{
        if (entity.anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") == false)
        {
            commonAttackID = 0;
            entity.stateManager.characterAnim.SetInt(AnimatorParameter.AttackAction.ToString(), commonAttackID);
            entity.stateManager.characterAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
            SkillSystem.Instance.AttackSkillHandler(entity, commonAttackID);

            //playerAnim.SetTrigger(AnimatorParameter.Attack.ToString(), true);
            attack = true;
        }
        else
        {
            SkillSystem.Instance.AttackSkillHandler(entity, commonAttackID + 1);
        }
        //}
    }

    public override void AirComboAttack()
    {
        Debug.Log("Air Combo Attack");
        RoleController entity = weaponManager.facade.roleController;
        bool attack = false;
        //if (entity.stateManager.nextAttackVaildSign)
        //{
        //    //if (playerFacade.weaponManager.attackedTarget)
        //    //{
        //    //    EndSkillTimeLineManager.Instance.SetEndSkillTimeLine(this, playerFacade.weaponManager.attackedTarget.GetComponentInParent<RoleController>(),EndKillTimeLineConfig.Instance.specialTimeLineConfigs[0].playerableAssets[0]);
        //    //    stateManager.nextAttackVaildSign = false;
        //    //    return;
        //    //}
        //    //playerAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
        //    //playerAnim.SetTrigger(AnimatorParameter.Attack.ToString(), true);

        //    attack = true;
        //}
        //else
        //{
        if (entity.anim.GetCurrentAnimatorStateInfo(0).IsTag("AirAttack") == false)
        {
            commonAttackID = 7;
        }
        var trackTarget = SkillSystem.Instance.Track(weaponManager.facade.roleController,commonAttackID + 1);
        if (trackTarget!=null&&trackTarget.Length>0)
        {
            MotionModifyBuff motionModifyBuff = TrackDamageableTarget(trackTarget[0]);
            GameTimeScaleCtrl.Instance.SetAroundVelocityMult(trackTarget[0].GetActorComponent<MoveComponent>(),0.2f);
            motionModifyBuff.updateAfterMovement = () => {
                GameTimeScaleCtrl.Instance.SetTimeSacle(0.5f,1000,1);
                GameTimeScaleCtrl.Instance.SetAroundVelocityMult(new MoveComponentSpeedDotween(entity.moveComponent,0f,1,GameTimeScaleCtrl.CalculateTimeDurationInGame(1000)),new MoveComponentSpeedDotween(trackTarget[0].GetActorComponent<MoveComponent>(),0f,1,GameTimeScaleCtrl.CalculateTimeDurationInGame(1000)));
                  weaponManager.facade.playerStateManager.waitNextAttackInput = true; GameRoot.Instance.AddTimeTask(GameTimeScaleCtrl.CalculateTimeDurationInGame(1000), () => { weaponManager.facade.playerStateManager.waitNextAttackInput = false; }, PETime.PETimeUnit.MillSeconds, 1);
            };
        }
        else
        {
            SkillSystem.Instance.AttackSkillHandler(entity, commonAttackID + 1);
        }
    }
    public bool TargetIsInTrackRange()
    {
        SkillEntity checkTrackRangeEntity = GetEntityInTrackRange();
        return checkTrackRangeEntity.IsHaveTarget();
    }
    public bool TargetIsInAttackRange()
    {
        SkillEntity checkRangeEntity = GetEntityInAttackRange();
        return checkRangeEntity.IsHaveTarget();
    }
    public SkillEntity GetEntityInTrackRange()
    {
        SkillEntity checkTrackRangeEntity = SkillSystem.Instance.SkillHandler(weaponManager.facade.roleController, 12, true);
        return checkTrackRangeEntity;
    }
    public SkillEntity GetEntityInAttackRange()
    {
        SkillEntity checkRangeEntity = SkillSystem.Instance.SkillHandler(weaponManager.facade.roleController, 11, true);
        return checkRangeEntity;
    }
    public override void ShiftComboAttack()
    {
        Debug.Log("Combo Attack");
        RoleController entity = weaponManager.facade.roleController;
        bool attack = false;
        //if (entity.stateManager.nextAttackVaildSign)
        //{
        //    //if (playerFacade.weaponManager.attackedTarget)
        //    //{
        //    //    EndSkillTimeLineManager.Instance.SetEndSkillTimeLine(this, playerFacade.weaponManager.attackedTarget.GetComponentInParent<RoleController>(),EndKillTimeLineConfig.Instance.specialTimeLineConfigs[0].playerableAssets[0]);
        //    //    stateManager.nextAttackVaildSign = false;
        //    //    return;
        //    //}
        //    //playerAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
        //    //playerAnim.SetTrigger(AnimatorParameter.Attack.ToString(), true);
            
        //    attack = true;
        //}
        //else
        //{
            if (entity.anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") == false)
            {
                commonAttackID = 5;
                entity.stateManager.characterAnim.SetInt(AnimatorParameter.AttackAction.ToString(), commonAttackID);
                entity.stateManager.characterAnim.ResetTrigger(AnimatorParameter.Attack.ToString());
                SkillSystem.Instance.AttackSkillHandler(entity, commonAttackID);

                //playerAnim.SetTrigger(AnimatorParameter.Attack.ToString(), true);
                attack = true;
            }
            else
        {
            SkillSystem.Instance.AttackSkillHandler(entity, commonAttackID + 1);
        }
        //}
    }
    public int tid;
    public override void Equip(bool immediately)
    {
        if (tid != 0)
        {
            GameRoot.Instance.DelTimeTask(tid);
        }
        base.Equip(immediately);
        if (immediately==false)
        {
            if (weaponManager==null)
            {
                weaponManager = GetComponentInParent<WeaponManager>();
            }
            weaponManager.facade.playerStateManager.characterAnim.SetLayerWeight(2, 1, false);

            weaponManager.facade.playerStateManager.characterAnim.SetInt(AnimatorParameter.EquipType.ToString(), 1);
            weaponManager.facade.playerStateManager.characterAnim.SetInt(AnimatorParameter.Equipment.ToString(), 1);
            tid=GameRoot.Instance.AddTimeTask(2600, () => { weaponManager.facade.playerStateManager.characterAnim.SetLayerWeight(2, 0, false); });
        }
        else
        {
            weaponManager.facade.playerStateManager.characterAnim.SetLayerWeight(2, 0, false);
            weaponEquipAndTake?.Invoke();
        }
        ///can appearing bug
    }
    public override void UnEquip(bool immediately)
    {
        if (tid != 0)
        {
            GameRoot.Instance.DelTimeTask(tid);
        }
        if (immediately==false)
        {
            weaponManager.facade.playerStateManager.characterAnim.SetLayerWeight(2, 1, false);
            weaponManager.facade.playerStateManager.characterAnim.SetInt(AnimatorParameter.EquipType.ToString(), 1);
            weaponManager.facade.playerStateManager.characterAnim.SetInt(AnimatorParameter.Equipment.ToString(), 2);
            tid=GameRoot.Instance.AddTimeTask(2600, () => { weaponManager.facade.playerStateManager.characterAnim.SetLayerWeight(2, 0, false);base.UnEquip(immediately); });
        }
        else
        {
            base.UnEquip(immediately);
            weaponManager.facade.playerStateManager.characterAnim.SetLayerWeight(2, 0, false);
            weaponUnEquipAndUnTake?.Invoke();
        }
        ///can appearing bug
    }
    public override void OnDefense()
    {
        base.OnDefense();
        Equip(true);
    }
}
[Serializable]
public class WeaponCheckPoint
{
    public Transform checkPoint;
    public float radius;
}

