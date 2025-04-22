using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static IDamageable;

public class NPCController : RoleController
{
    [HideInInspector]
    public CharacterController characterController;
    public float viewAngle = 100;//视角
    public float attackDistance;//攻击范围
    public float remoteAttackDistance;//远程攻击范围
    public float maxFollowDis = 5;//追随范围
    [Tooltip("运动动画开启数值驱动 脚本手动控制，否则启用rootmotion"), Header("运动动画开启数值驱动")]
    public bool locomotionSpeedEnableMathDirector;
    public ActionRange actionRange;
    public bool isTogglePatrol = false;
    public NavMeshPointGroup NavPointGroup;
    public int overrideDropItem=-1;
    private RoleController followTarget;

    

    public RoleController FollowTarget { get => followTarget;protected set => followTarget = value; }

    public override void Init()
    {
        base.Init();
        isController = true;
    }
    protected virtual void Update()
    {
        if (isController)
        {
            ListenerCommmonMove();
        }


        UpdateBlend();
    }
    public void FixedUpdate()
    {
        CalcuateMove();
    }
    public virtual void ListenerCommmonMove()
    {
        if (locomotionSpeedEnableMathDirector)
        {
            if (moveSpeedMult != targetBlend)
            {
                if (facade.characterAnim.stateInfo.IsTag("Locomotion"))
                {
                    moveSpeedMult = targetBlend;
                }
            }
        }
    }
    public override void CalcuateMove()
    {
        moveComponent.CalcuateMove();
        //var velo = (characterController.velocity + DeltaPosition + increaseVelo) / 2;//TimeLine和Rootmotion也使用这种运动方式。 防止穿模。所以这块不能直接用IsController去取消移动
        ////注意要判断速度是否为0 ，因为SimpleMove不知道怎么回事。TimeLine设置完位置后， TimeLine结束后 又会归回上一个点。经过发现是这个SimpleMove的问题。一直传零的时候导致的
        //if (velo!=Vector3.zero||(velo==Vector3.zero&& characterController.velocity!=Vector3.zero))
        //{
        //    characterController.SimpleMove(velo);
        //}

        // DeltaPosition = Vector3.zero;
        //ClearIncreaseVelo();
    }
    public override void AnimatorMove(Animator animator, Vector3 velocity, Quaternion angular)
    {

        if (TimeLineManager.Instance.TimeLineIsPlay)
        {
            EndSkillTimeLineManager.Instance.director.GetGenericBinding(EndSkillTimeLineManager.Instance.bindingDic[Constants.hiterTrackName].sourceObject);
        }

    }

    public virtual void Idle()
    {

    }
    public virtual void Patrol()
    {

    }
    public virtual void Attack()
    {

    }
    public virtual void Turn()
    {

    }
    public virtual void AttackEnter()
    {

    }
    public virtual void OpenAttackTime()
    {

    }
    #region 
    public virtual bool InAttackRange()
    {
        return false;
    }
    public virtual bool AttackDeltaTimeOut()
    {
        return false;
    }
    public virtual bool SatisfyAttackCondition()
    {
        return InAttackRange() && AttackDeltaTimeOut();
    }
    #endregion
    public bool IsSeeTarget(bool mustCanArrive=true)
    {
        return IsSeeTarget(lockTarget?.transform,mustCanArrive);
    }
    private bool IsSeeTarget(Transform target,bool mustCanArrive=true)
    {
        return IsSeeTarget(target, viewAngle,mustCanArrive);
    }
    private bool IsSeeTarget(Transform target, float angle,bool mustCanArrive=true)
    {
        if (target==null||TargetDiedOrNULL(target.GetComponent<RoleController>()))
        {
            return false;
        }
        if (mustCanArrive)
        {
            NavMeshPath path = moveComponent.CalculatePath(target.position);
            if (path == null || path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid)
            {
                return false;
            }
        }
        Vector3 dir = (target.transform.position - moveComponent.POSITION).normalized;
        return (Vector3.Angle(transform.forward, Vector3.ProjectOnPlane(dir,Vector3.up)) <= angle / 2);
    }
    public bool TargetIsInFollowRange()
    {
        if (lockTarget == null)
        {
            return false;
        }
        return TargetIsInRange(maxFollowDis);
    }
    public bool TargetDiedOrNULL()
    {
        return TargetDiedOrNULL(lockTarget);
    }
    public bool TargetDiedOrNULL(RoleController target)
    {
        if (target == null)
        {
            return true;
        }
        return target.damageable.IsDie || target.gameObject.activeInHierarchy == false;
    }
    public bool TargetIsInAttackRange()
    {
        if (lockTarget == null)
        {
            return false;
        }
        return TargetIsInRange(attackDistance);
    }
    public bool TargetIsInAttackRange(Transform target)
    {
        if (target == null)
        {
            return false;
        }
        return TargetIsInRange(target, attackDistance);
    }
    public void UpdateAttackDistance(float distance)
    {
        attackDistance = distance;
    }
    public bool TargetIsInRange(float range)
    {
        return TargetIsInRange(lockTarget?.transform, range);
    }
    public bool TargetIsInRange(Transform target, float range)
    {
        if (target==null)
        {
            return false;
        }
        Vector3 dis = target.position - moveComponent.POSITION;
        return range >= (dis.magnitude);
    }
    public RoleController CheckRangeIsHaveOppose(float range)
    {
        RoleController target = null;
        List<RoleController> opposeRoles = BattleManager.Instance.FilterUnits(opposeCampTagStrList);
        if (opposeRoles.Count > 0)
        {
            if (opposeRoles.Count > 1)
            {
                opposeRoles.Sort((enemy1, enemy2) =>
            {
                int result = (GetBetweenDistance(enemy1.transform) < GetBetweenDistance(enemy2.transform)) ? 1 : -1;
                return result;
            });
            }
            for (int i = 0; i < opposeRoles.Count; i++)
            {
                if (TargetIsInRange(opposeRoles[i].transform, range))
                {
                    target = opposeRoles[i];
                }
            }
        }
        return target;
    }
    public void LoseTarget()
    {
        lockTarget = null;
    }
    public void UpdateTarget(RoleController opposeRole)
    {
        lockTarget = opposeRole;
    }
    public void TryLockAttackTarget(float range = 0)
    {
        if (damageable.attackerList.Count > 0)
        {
            bool isCanLock = true;
            if (range > 0)
            {
                if (Vector3.Distance(damageable.attackerList[0].transform.position, transform.position) > range)
                {
                    isCanLock = false;
                }
            }
            if (isCanLock)
            {
                UpdateTarget(damageable.attackerList[0]);
                FixedCharacterLookToTarget(lockTarget.transform);
            }
        }
    }
    public void UpdateTarget(IDamageable opposeRole)
    {
        lockTarget = opposeRole.GetActorComponent<RoleController>();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="angle">角度等于0的时候表示 是当前敌人所设置的viewAngle</param>
    public void UpdateTarget(int angle = 0,bool mustCanArrive=true)
    {
        if (lockTarget == null)
        {
            float viewAngle = angle == 0 ? this.viewAngle : Mathf.Clamp(Math.Abs(angle), 0, 360);

            List<RoleController> opposeRoles = BattleManager.Instance.FilterUnits(opposeCampTagStrList);
            if (opposeRoles.Count > 0)
            {
                if (opposeRoles.Count > 1)
                {
                    opposeRoles.Sort((enemy1, enemy2) =>
                {
                    int result = (GetBetweenDistance(enemy1.transform) > GetBetweenDistance(enemy2.transform)) ? 1 : -1;
                    return result;
                });
                }
                foreach (var oppose in opposeRoles)
                {
                    if (oppose)
                    {
                        if (IsSeeTarget(oppose.transform, viewAngle,mustCanArrive)&&TargetIsInRange(oppose.transform,maxFollowDis))
                        {
                            lockTarget = oppose;
                            break;
                        }
                    }
                }

            }
        }
        else 
        {
            if (IsSeeTarget(mustCanArrive)==false)
            {
                lockTarget = null;
            }
        }
    }
    public bool TargetIsInRemoteAttackRange()
    {
        if (lockTarget == null || remoteAttackDistance <= attackDistance)
        {
            return false;
        }
        return TargetIsInRange(remoteAttackDistance);
    }
    public void OnDrawGizmosSelected()
    {
        Vector3 leftPoint = transform.GetCirclePoint(-viewAngle / 2);
        Vector3 rightPoint = transform.GetCirclePoint(viewAngle / 2);
        Vector3 leftDir = (leftPoint - transform.position).normalized;
        Vector3 rightDir = (rightPoint - transform.position).normalized;
        leftPoint = transform.position + leftDir * attackDistance;
        rightPoint = transform.position + rightDir * attackDistance;
        //视角线
        Handles.color = Color.red;
        Handles.DrawLine(transform.position, leftPoint);
        Handles.DrawLine(transform.position, rightPoint);
        //攻击范围
        Handles.color = new Color(1, 0, 27.0f / 255, 70.0f / 255);
        Handles.DrawSolidArc(transform.position, transform.up, leftDir, viewAngle, attackDistance);
        //远程攻击范围
        Handles.color = new Color(247.0f / 255, 1, 0, 40.0f / 255);
        Handles.DrawSolidArc(transform.position, transform.up, leftDir, viewAngle, remoteAttackDistance);
        //追踪范围
        Handles.color = new Color(52f / 255, 1, 0, 20.0f / 255);
        Handles.DrawSolidArc(transform.position, transform.up, leftDir, viewAngle, maxFollowDis);
        //360范围
        Handles.color = Color.white;
        Handles.DrawWireArc(transform.position, transform.up, transform.forward, 360, attackDistance);
        Handles.DrawWireArc(transform.position, transform.up, transform.forward, 360, maxFollowDis);
    }
    public override Vector3 GetAimPoint(Vector3 weaponPoint, Quaternion lookForward)
    {
        Vector3 point = Vector3.zero;
        if (lockTarget != null)
        {
            point = lockTarget.centerTrans.transform.position;
        }
        else
        {
            point = weaponPoint + lookForward.eulerAngles * PlayerableConfig.Instance.bowArrowMaxRayDis;
        }
        //point += UnityEngine.Random.insideUnitSphere*0.5f;
        return point;
    }
    public void DeadAfterDropItem()
    {
        List<DropItemKindInfo> dropItemKindInfos = new List<DropItemKindInfo>();
        if (overrideDropItem != -1)
        {
            dropItemKindInfos = DropItemListContainer.Instance.GetItemKindInfo(overrideDropItem).ToList();
        }
        else
        {
            dropItemKindInfos = actorSystem.roleDefinition.RandomGetItemInRange();
        }
        if (dropItemKindInfos==null)
        {
            return;
        }
        foreach (var itemKindInfo in dropItemKindInfos)
        {
            GameObject itemGO = null;
            GameObject itemPrefab = itemKindInfo.itemDataConfig.GetItemPrefab();
            if (itemPrefab != null)
            {
                itemGO = GameObjectFactory.Instance.PopItem(itemPrefab);
            }
            if (itemGO != null)
            {
                itemGO.SetActive(true);
                itemGO.transform.position = transform.position;
            }
            else
            {
                itemGO = GameObjectFactory.Instance.PopItem("Prefab/ItemPacker");
                itemGO.SetActive(true);
                itemGO.transform.SetParent(null, false);
                itemGO.transform.position = transform.position + Vector3.up * 0.1f;
            }

            DropItemObject _DropItemObject = itemGO.GetComponent<DropItemObject>();
            if (_DropItemObject == null)
            {
                _DropItemObject = itemGO.AddComponent<DropItemObject>();
            }
            DropItemPacker packer = new DropItemPacker();
            packer.relativeObject = itemGO;
            if (packer.dropItemKindDatas == null)
            {
                packer.dropItemKindDatas = new List<DropItemData>();
            }
            packer.AddItem(new DropItemData(itemKindInfo, itemKindInfo.RandomCount()));
            _DropItemObject.itemPacker = packer;
        }
    }
    public bool IsInActionRange(Vector3 targetPos)
    {
        if (actionRange == null)
        {
            return true;
        }
        return actionRange.IsInActionRange(targetPos);
    }
    public override void OnDie(DamageData damageData)
    {
        DeadAfterDropItem();
        SetCharacterColliderEnable(false);
        moveComponent.EnableGravity(false);
        anim.SetInteger(AnimatorParameter.HitAction.ToString(), -1);
        moveSpeedMult = 0;
        isController = false;
        targetBlend = 0;
        ClearAllMotionData(false);
        if (EndSkillTimeLineManager.Instance.EndKillTLPlayed)
        {
            return;
        }
        EnableRagDoll(damageData);
    }
    public bool IsCanPatrol()
    {
        return isTogglePatrol && moveComponent.Agent != null;
    }
    public NavMeshPointGroup StartPatrol()
    {
        NavPointGroup = MapManager.Instance.enemyNavMeshWorld.GetFineNavPath(moveComponent.Agent);

        GameRoot.Instance.AddTimeTask(2, () =>
        {
            if (this.NavPointGroup==null)
            {
                BattleManager.Instance.RemoveHpBarEntity(damageable);
            }
        }, PETime.PETimeUnit.Seconds, 1);

        return NavPointGroup;
    }
    public void StopPatrol()
    {
        MapManager.Instance.enemyNavMeshWorld.LeaveNavMeshPath(moveComponent.Agent, NavPointGroup);
        StopMove();
        NavPointGroup = null;
    }
    public virtual void SetNavMeshTarget(Vector3 target)
    {
        moveComponent.SetNavMeshTarget(target);
    }
    public virtual void StopMove()
    {
        LerpForwardBlend(0);
        LerpAnimationFloatVlaue(AnimatorParameter.Horizontal.ToString(), 0, 1, true);
        moveComponent.StopNavAgent();
    }
    public void SetFollow(RoleController roleController)
    {
        followTarget = roleController;
    }
    public void LoseFollow()
    {
        followTarget = null;
    }
    public bool IsHaveFollowTarget()
    {
        return FollowTarget != null;
    }

}

