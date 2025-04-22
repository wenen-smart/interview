using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public class SkillData
    {
    public string skillName;
    public int skillID;
    public string derscription;
    public float coolTime;
    public int costSp;
    public float attackDistance;
    public float attackangle;
    public int nextComboId;
    public float damagePre;
    public float durationTime;//unit-> ms
    public int damageInterval;//unit-> ms
    public int endSkillID=-1;
    public int backStabID_endSkill = -1;

    public int skillLevel;
    public bool isAutoLockTarget;
    public DamageMode damageMode;
    public bool isTriggerCheck=false;
    public SkillAttackType attackType;
   [MultSelectTags]
    public SelfImpartType selfImpartEnum;
   [MultSelectTags]
    public TargetImpact targetImpartEnum;

    //public string skillPrefabName;
    //public string animationName;
    //public string hitFxName;

    public int anim_attackAction;
    public int anim_hitAction=1;//规定1为默认HIt

    public GameObject skillPrefab;
    public GameObject hitFxPrefab;
    public bool IsCtrlLookAttacker = false;
    public bool IsAutoAttackDir = false;//是否自动获取武器瞬时攻击方向作为目标移动方向
    
    public Vector3 force;
    [Header("超出攻击范围是否追踪")]
    public bool OverAckDisIsTrack = false;
    public float trackingRange = 0;
    /// <summary>
    /// 攻击者 局部坐标系
    /// </summary>
    [Tooltip("当isTriggerCheck为False的时候 以此值作为攻击方向。")]
    public Vector3 attackDir;//
}

