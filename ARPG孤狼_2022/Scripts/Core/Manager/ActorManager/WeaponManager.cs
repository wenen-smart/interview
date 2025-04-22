using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponManager : ActorComponent
{
    public List<Rig> weaponRigList;
    private CoroutineContainer _CoroutineContainer=new CoroutineContainer();
    public Rig defenseRig;
    public float fadeWeightSpeed = 2;
    [HideInInspector]
    public IWeapon currentWeapon;
    [HideInInspector]
    public int currentWeaponIndex = 0;
    public List<IWeapon> weaponList;
    [HideInInspector]
    public IDamageable soonAttackedTarget;//即将要攻击的对象，还没有计算伤害
    [HideInInspector]
    public IDamageable attackedTarget;//攻击的对象，已经计算完伤害
    [HideInInspector]
    public IDamageable soonDiedTarget;//即将要死亡的对象，还没有计算伤害
    [HideInInspector]
    public IDamageable diedTarget;//end skill target
    public Func<bool> endSkillFunc;
    public Collider defenseCollider;
    public CharacterFacade facade { get {return GetActorComponent<CharacterFacade>(); } }
    public bool triggerCheck;
    public RoleWeaponManagerConfig managerConfig;
    public Transform rightConsumablePos;

    public override void Init()
    {
        base.Init();
        facade.roleController.UnBattleAction += () => { RestingNoBattle(false); };
        facade.roleController.BattleAction += () => { PrepareBattle(true); currentWeapon.gameObject.SetActive(true); if (currentWeapon.unEquipGo) { currentWeapon.unEquipGo.SetActive(false); } currentWeapon.ShowRenderer(); };
        GameRoot.Instance.AddTimeTask(500,()=> { ExchangeWeapon(0);});
    }
    public void CloseAllRig(bool immediatly)
    {
        foreach (var rig in weaponRigList)
        {
            if (immediatly)
            {
                _CoroutineContainer.StartCoroutine(this, FadeWeight(rig, 0), "FadeWeight" + rig.GetInstanceID());
            }
            else
            {
                _CoroutineContainer.StartCoroutine(this, FadeWeightImmdietely(rig, 0), "FadeWeightImmdietely" + rig.GetInstanceID());
            }
        }
    }
    public void StartDefense(bool immediatly = false)
    {
        OpenDefenseCol();
        currentWeapon.OnDefense();//TODO 左手护盾右手武器的怎么办？
        StopUnEquipResting();
        PrepareBattle(true);
        CloseAllRig(immediatly);
        if (immediatly)
        {
            _CoroutineContainer.StartCoroutine(this, FadeWeightImmdietely(defenseRig, 1), "FadeWeightImmdietely" + defenseRig.GetInstanceID());
        }
        else
            _CoroutineContainer.StartCoroutine(this, FadeWeight(defenseRig, 1), "FadeWeight" + defenseRig.GetInstanceID());
    }



    public void CanelDefense(bool immediatly = false)
    {
        CloseDefenseCol();
        PrepareBattle(false);
        if (immediatly)
        {
            _CoroutineContainer.StartCoroutine(this, FadeWeightImmdietely(defenseRig, 0), "FadeWeightImmdietely" + defenseRig.GetInstanceID());
        }
        else
            _CoroutineContainer.StartCoroutine(this, FadeWeight(defenseRig, 0), "FadeWeight" + defenseRig.GetInstanceID());
    }

    public IEnumerator FadeWeight(Rig rig, int weight)
    {
        while (true)
        {
            rig.weight = Mathf.Lerp(rig.weight, weight, Time.deltaTime * fadeWeightSpeed);
            if (Mathf.Abs(rig.weight - weight) < 0.01)
            {
                rig.weight = weight;
                break;
            }
            yield return 0;
        }
    }
    public IEnumerator FadeWeightImmdietely(Rig rig, int weight)
    {
        while (true)
        {
            rig.weight = Mathf.Lerp(rig.weight, weight, 1);
            if (Mathf.Abs(rig.weight - weight) < 0.01)
            {
                rig.weight = weight;
                break;
            }
            yield return 0;
        }
    }
    public bool ExchangeWeapon(IWeapon weapon)
    {
        bool haveInCharacter = false;
        if (facade==PlayerFacade.Instance)
        {
            foreach (var item in actorSystem.roleData.InventoryData.equipmentDataConfigs)
            {
                if (item is WeaponDataConfig)
                {
                    WeaponDataConfig weaponDataConfig = item as WeaponDataConfig;
                    if (weapon.weaponData.GetWeaponType() == weaponDataConfig.GetWeaponType())
                    {
                        haveInCharacter = true;
                        break;
                    }
                }
            }
        }
        else
        {
            haveInCharacter = true;
        }
        if (haveInCharacter)
        {
            facade.roleController.UnEquipWeaponHandler(currentWeapon);
            currentWeapon?.UnEquip(true);
            WeaponConfig_Manager config_manager = GetWeaponConfig(weapon);
            if (config_manager != null && weapon != currentWeapon)
            {
                facade.SetAnimatorController(config_manager.overrideController);
            }
            currentWeapon = weapon;
            PrepareBattle(false);
            currentWeapon.weaponManager = this;
        }
        
        //Show Hide
        return haveInCharacter;
    }
    public void ExchangeWeaponByScroll(int up)
    {
        if (up==0)
        {
            return;
        }
        up=((up>0)?1:-1);
        ExchangeWeapon(currentWeaponIndex+up);
    }
    public void ExchangeWeapon(int index)
    {
        if (weaponList.Count==0)
        {
            MyDebug.DebugWarning($"{facade.gameObject.name}没有武器列表");
            return;
        }
        int weaponIndex = index % weaponList.Count;
        if (weaponList != null && weaponList.Count > 0)
        {
            if (ExchangeWeapon(weaponList[weaponIndex]))
            {
                currentWeaponIndex = weaponIndex;
            }
        }
        else
        {
            Debug.Log("Current weaponList no weapon");
        }
    }
    public void SetEndSkill()
    {

    }

    public void CanelEndSkill()
    {

    }

    public void OpenDefenseCol()
    {
        defenseCollider.gameObject.SetActive(true);
    }
    public void CloseDefenseCol()
    {
        defenseCollider.gameObject.SetActive(false);
    }
    public void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Defense"))
        {
           currentWeapon.TriggerEnemyDefense(other);
        }
    }
    public virtual void PrepareBattle(bool immediately)
    {
        currentWeapon?.Equip(immediately);
        facade.roleController.EquipWeaponHandler(currentWeapon);
        facade.playerStateManager.isBattle = true;
        StopUnEquipResting();
    }
    public virtual void RestingNoBattle(bool immediately)
    {
        restingTimer = 0;
        resting = true;
    }
    public virtual void StopUnEquipResting()
    {
        restingTimer = 0;
        resting = false;
    }

    public float restingTime = 5;
    public float restingTimer = 0;
    public bool resting = false;
    public void Update()
    {
        if (resting)
        {
            if (restingTime > restingTimer)
            {
                restingTimer += Time.deltaTime;
            }
            else
            {
                FinishBattle(false);
                resting = false;
                restingTimer = 0;
            }
        }

    }
    public virtual void FinishBattle(bool immediately)
    {
        currentWeapon?.UnEquip(immediately);
        facade.playerStateManager.isBattle = false;
    }
    public Vector3 GetAttackDir()
    {
        return currentWeapon.GetAttackDir();
    }
    public bool CheckAttackTrigger(SkillEntity skillEntity,Action deploySkill)
    {
        return currentWeapon.CheckAttackTrigger(skillEntity,deploySkill);
    }
    public WeaponType GetWeaponType()
    {
        return currentWeapon.weaponData.GetWeaponType();
    }
    public WeaponConfig_Manager GetWeaponConfig(IWeapon weapon)
    {
        if (managerConfig==null)
        {
            return null;
        }
        foreach (var config in managerConfig.WeaponConfigList)
        {
            if (config.weaponName==weapon.weaponData.ITEM_Name)
            {
                return config;
            }
        }
        return null;
    }
    
}

public enum WeaponType
{
    Sword,
    Bow,
}

