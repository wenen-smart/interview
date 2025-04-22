using Assets.Resolution.Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class WeaponDataConfig : ItemDataConfig
{
    [Header("��һ�˳��ֱ�Ԥ�Ƽ�"), FormerlySerializedAs("firstPersonHandAttachWeaponItemPrefa")]
    public GameObject firstPersonHandAttachWeaponItemPrefab;//��һ�˳��ֱ�Ԥ�Ƽ�
    public WeaponType weaponType;
    public int baseDamage;//�����˺�
    public AnimationCurve affectCurve;//x���������Ǿ��롣  0����ǰ����Ӱ���λ��  1��ʾ���Ӱ��ķ�Χ  ��ֵԽС�������Խ��
    public float useIntervalTime;//����ļ��ʱ��
    public GameObject[] fireEmitterPrefabs;
    public string itemAttachPointName;
    public bool isMaintainAttach = true;
    public bool isCanThrow = false;
    [Header("��һ�˳�׼������")]
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
    //��ǹ
    Rifle,
    //���
    Assault,
    //�ѻ�
    Snipe,
    //��ǹ
    Pistol,
    //����
    Shotgun
}
public enum ThrowItemType
{
    Explosion,
    FalshLight,
    Smoke,
}
