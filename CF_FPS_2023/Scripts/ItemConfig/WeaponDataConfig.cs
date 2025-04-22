using Assets.Resolution.Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class WeaponDataConfig : ItemDataConfig
{
    [Header("第一人称手臂预制件"), FormerlySerializedAs("firstPersonHandAttachWeaponItemPrefa")]
    public GameObject firstPersonHandAttachWeaponItemPrefab;//第一人称手臂预制件
    public WeaponType weaponType;
    public int baseDamage;//基础伤害
    public AnimationCurve affectCurve;//x轴描述的是距离。  0代表当前发起影响的位置  1表示最大影响的范围  故值越小代表距离越近
    public float useIntervalTime;//开火的间隔时间
    public GameObject[] fireEmitterPrefabs;
    public string itemAttachPointName;
    public bool isMaintainAttach = true;
    public bool isCanThrow = false;
    [Header("第一人称准星配置")]
    public CrosshairSetting crosshairSetting;
    public int cellIndex { get; protected set; }
    public string iconInHUD_Name;
    public virtual void OnEnable()
    {
        cellIndex = weaponType - WeaponType.Primarily;
    }
    public virtual BaseWeapon InstantiateWeaponAndTryAttach(RoleController role)
    {
        BaseWeapon newWeapon = null;
		if (role is FPS_PlayerController)
		{
			GameObject go = GameObject.Instantiate(firstPersonHandAttachWeaponItemPrefab);
			role.AttachWeapon(go, "");
			newWeapon = go.GetComponent<BaseWeapon>();
		}
		else
		{
			GameObject go = GameObject.Instantiate(_itemPrefab);
			role.AttachWeapon(go, itemAttachPointName);
			newWeapon = go.GetComponent<BaseWeapon>();
		}
        return newWeapon;
	}
}

public enum GunType
{
    //步枪
    Rifle,
    //冲锋
    Assault,
    //狙击
    Snipe,
    //手枪
    Pistol,
    //霰弹
    Shotgun
}
public enum ThrowItemType
{
    Explosion,
    FalshLight,
    Smoke,
}
