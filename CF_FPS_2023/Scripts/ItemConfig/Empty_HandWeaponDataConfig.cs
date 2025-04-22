using Assets.Resolution.Scripts.Weapon;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New_Empty_HandSO", menuName = "CreateWeaponSO/CreateEmpty_HandSO")]
public class Empty_HandWeaponDataConfig : MeleeWeaponDataConfig
{
	[Header("Bot的动画")]
	public WeaponAnimationStruct[] weaponAnimationStructs;
	public override BaseWeapon InstantiateWeaponAndTryAttach(RoleController role)
	{
		EmptyHand_Weapon newWeapon = null;
		if (role is FPS_PlayerController)
		{
			GameObject go = GameObject.Instantiate(firstPersonHandAttachWeaponItemPrefab);
			role.AttachWeapon(go, "");
			newWeapon = go.GetComponent<EmptyHand_Weapon>();
		}
		else
		{
			GameObject go = GameObject.Instantiate(_itemPrefab);
			role.AttachWeapon(go, itemAttachPointName);
			newWeapon = go.GetComponent<EmptyHand_Weapon>();
		}
		return newWeapon;
	}

}

