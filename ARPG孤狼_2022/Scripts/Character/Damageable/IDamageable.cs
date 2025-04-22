using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public abstract class IDamageable:ActorComponent
    {

    public int hp;
    public Action<DamageData> dieAction;
    public Action<DamageData> hitAction;
    public bool isDenfense;
    [HideInInspector]
    public CharacterFacade characterFacade;
    public Vector3 lastDamagedDir;
    public List<IDamageable> attackerList=new List<IDamageable>();
    
    public override void Start()
    {
        base.Start();
        characterFacade = GetComponent<CharacterFacade>();
    }
    public bool IsDie { get => hp<=0; }

    protected virtual void Damage(DamageData damageData)
    {
        if (IsDie)
        {
            return;
        }
        
        attackerList.Clear();
        attackerList.Add(damageData.attacker.GetComponent<IDamageable>());
        hp -= damageData.damage;

        HpBarEntity _HpBarEntity = characterFacade.ApplyHpBarEntity();
        _HpBarEntity?.SetInfo(actorSystem.roleDefinition.object_Name, this);
        _HpBarEntity?.gameObject?.SetActive(true);
        _HpBarEntity?.UpdateHpBar(this);

        if (hp <= 0)
        {
           
            Die(damageData);
        }
        PlayHitEffectAudio(IsDie);
    }
    public virtual void Hit(Transform attacker,int damage,Vector3 attackDir,Vector3 attackPoint,int hitIntAction=1,int defenseAction=0)
    {
        MyDebug.DebugPrint("Hit");
        Vector3 horAttackDir = Vector3.ProjectOnPlane(attackDir, Vector3.up);
        MyDebug.DebugWireSphere(new Vector3[] { transform.position + Vector3.up * 1f + horAttackDir * 5},0.5f,Color.blue,5);
        MyDebug.DebugLine(transform.position + Vector3.up * 1f, transform.position + Vector3.up * 1f + horAttackDir * 5, Color.red, 5);
        Vector3 cross = Vector3.Cross(transform.forward, horAttackDir);
        float angle = Vector3.Angle(transform.forward, horAttackDir);
        if (cross.y < 0)
        {
            angle = -angle;
        }
        Debug.Log(cross);
        GetComponent<RoleController>().OnHit(hitIntAction, angle, defenseAction);
        DamageData damageData = new DamageData(attacker, damage, attackDir);
        Damage(damageData);
        lastDamagedDir = horAttackDir;
        hitAction?.Invoke(damageData);
        if (attackPoint!=Vector3.zero)
        {
            Vector3 PointAddtiveAttackDir = attackPoint + attackDir;
            Transform closetBone = characterFacade.GetClosetBoneTrans(PointAddtiveAttackDir);
            if (closetBone!=null)
            {
                InstantiateBloodEffect(closetBone.position,attackDir);
            }
            
        }
    }
    public void  InstantiateBloodEffect(Vector3 pos,Vector3 attackDir)
    {
        int min = 1;
        int max = 16;
        int bloodsuffix = UnityEngine.Random.Range(min,max);
        string bloodPath = "Prefab/Blood/Blood" + bloodsuffix;
        GameObject blood = GameObjectFactory.Instance.PopItem(bloodPath);
        blood.transform.position = pos;
        blood.transform.rotation = Quaternion.LookRotation(attackDir) * Quaternion.Euler(blood.transform.TransformDirection(new Vector3(0,-90,0)));
        BFX_BloodSettings bFX_BloodSettings = blood.GetComponent<BFX_BloodSettings>();
        bFX_BloodSettings.GroundHeight = transform.position.y;
        GameRoot.Instance.AddTask(5000,()=> { GameObjectFactory.Instance.PushItem(blood);});
    }
    public virtual void Hit(int baseDamage,Vector3 attackDir,Transform attacker,SkillData skillData=null,Vector3 attackPoint=default(Vector3))
    {
        if (skillData == null)
        {
            MyDebug.DebugError("SkillData 为Null 严重错误");
            return;
        }
        if (skillData.IsCtrlLookAttacker)
        {
            characterFacade.roleController.FixedCharacterLookToTarget(attacker);
        }
        Hit(attacker,baseDamage, attackDir,attackPoint, skillData?.anim_hitAction != null ? skillData.anim_hitAction : 0);
    }
    public void PlayHitEffectAudio(bool isDie)
    {
        string identity = isDie ?"Die":"Hit";
        AudioClipCue.Play(transform,identity,true);
    }
    public virtual void Die(DamageData damageData)
    {
        hp = 0;
        BattleManager.Instance.RemoveHpBarEntity(this);
        dieAction?.Invoke(damageData);
    }
    public void OnTriggerEnter(Collider other)
    {
        
    }
    public int MaxHP => 100;
   
    public struct DamageData
    {
        public Transform attacker;
        public int damage;
        public Vector3 attackDir;

        public DamageData(Transform attacker, int damage, Vector3 attackDir)
        {
            this.attacker = attacker;
            this.damage = damage;
            this.attackDir = attackDir;
        }
    }
}

