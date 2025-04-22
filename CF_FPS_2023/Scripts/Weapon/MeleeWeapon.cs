using Assets.Resolution.Scripts.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Resolution.Scripts.Weapon
{
	public abstract class MeleeWeapon : BaseWeapon
	{
		//public float meleeRange = 1;
		//public float meleeAngle;
		//public string lightKnifeAnim;
		//public string thumpKnifeAnim;
		public MeleeWeaponDataConfig data;
		public override int baseDamage => data.baseDamage;
		public override WeaponType weaponType => data.weaponType;

		public List<MeleeAttackData> light_meleeAttackDataWayList;

		public List<MeleeAttackData> Thump_meleeAttackDataWayList;

		public override void Awake()
		{
			base.Awake();
		}
		public override void Equiped(RoleController _user)
		{
			base.Equiped(_user);
		}
		public override bool Use(bool isContinueInput, WeaponUseInputType weaponUseInputType = WeaponUseInputType.Default)
		{
			if (weaponUseInputType == WeaponUseInputType.Default)
			{
				return OutLight();
			}
			else if (weaponUseInputType == WeaponUseInputType.Thump)
			{
				return OutThump();
			}
			return false;
		}
		public bool OutLight()
		{
			MeleeAttackData light_meleeAttackData;
			bool isPlayingLightAnim = IsPlayingAttackAction(WeaponUseInputType.Default, out light_meleeAttackData);
			if (isPlayingLightAnim && light_meleeAttackData.nextComboID != -1 && AnimatorMachine.CheckInAnimationDistrict(userAnimator, 0, light_meleeAttackData.animationName, light_meleeAttackData.ComboCheckDistinct.StartNormalizeTime, light_meleeAttackData.ComboCheckDistinct.FinishNormalizeTime,true))
			{
				userAnimatorMachine.CrossFade(light_meleeAttackDataWayList[light_meleeAttackData.nextComboID].animationName, 0,0,0,true);
				AudioManager.Instance.PlayAudioByClipClue(transform, Const_SoundClipCue.WeaponKnife_WaveSoundIdentity, true);
				AnimatorMachine.ListenerAnimation(userAnimator, 0, light_meleeAttackDataWayList[light_meleeAttackData.nextComboID].animationName, light_meleeAttackDataWayList[light_meleeAttackData.nextComboID].attackCheckDistinct.StartNormalizeTime, () => OnMeleeCheck(WeaponUseInputType.Default,light_meleeAttackData.nextComboID));
			}
			else if (isPlayingLightAnim == false && IsPlayingAttackAction(WeaponUseInputType.Thump) == false)
			{
				userAnimatorMachine.CrossFade(light_meleeAttackDataWayList[0].animationName, 0,0,0,true);
				AudioManager.Instance.PlayAudioByClipClue(transform, Const_SoundClipCue.WeaponKnife_WaveSoundIdentity, true);
				AnimatorMachine.ListenerAnimation(userAnimator, 0, light_meleeAttackDataWayList[0].animationName, light_meleeAttackDataWayList[0].attackCheckDistinct.StartNormalizeTime, () => OnMeleeCheck(WeaponUseInputType.Default));
			}
			return true;
		}
		public bool OutThump()
		{
			MeleeAttackData thump_meleeAttackData;
			bool isPlayingThumpAnim = IsPlayingAttackAction(WeaponUseInputType.Thump, out thump_meleeAttackData);
			if (isPlayingThumpAnim && thump_meleeAttackData.nextComboID != -1 && AnimatorMachine.CheckInAnimationDistrict(userAnimator, 0, thump_meleeAttackData.animationName, thump_meleeAttackData.ComboCheckDistinct.StartNormalizeTime, thump_meleeAttackData.ComboCheckDistinct.FinishNormalizeTime,true))
			{
				userAnimatorMachine.CrossFade(Thump_meleeAttackDataWayList[thump_meleeAttackData.nextComboID].animationName, 0,0,0,true);
				AudioManager.Instance.PlayAudioByClipClue(transform, Const_SoundClipCue.WeaponKnife_ThumpWaveSoundIdentity, true);
				AnimatorMachine.ListenerAnimation(userAnimator, 0, Thump_meleeAttackDataWayList[thump_meleeAttackData.nextComboID].animationName, Thump_meleeAttackDataWayList[thump_meleeAttackData.nextComboID].attackCheckDistinct.StartNormalizeTime, () => OnMeleeCheck(WeaponUseInputType.Thump));
			}
			else if (isPlayingThumpAnim == false && IsPlayingAttackAction(WeaponUseInputType.Default) == false)
			{
				userAnimatorMachine.CrossFade(Thump_meleeAttackDataWayList[0].animationName, 0,0,0,true);
				AudioManager.Instance.PlayAudioByClipClue(transform, Const_SoundClipCue.WeaponKnife_ThumpWaveSoundIdentity, true);
				AnimatorMachine.ListenerAnimation(userAnimator, 0, Thump_meleeAttackDataWayList[0].animationName, Thump_meleeAttackDataWayList[0].attackCheckDistinct.StartNormalizeTime, () => OnMeleeCheck(WeaponUseInputType.Thump));

			}
			return true;
		}
		public bool IsPlayingAttackAction(WeaponUseInputType inputType, out MeleeAttackData meleeAttackData)
		{
			if (inputType == WeaponUseInputType.Default)
			{
				foreach (var item in light_meleeAttackDataWayList)
				{
					if (AnimatorMachine.CheckNextOrCurrentAnimationStateByName(userAnimator, 0, item.animationName))
					{
						meleeAttackData = item;
						return true;
					}
				}
			}
			else if (inputType == WeaponUseInputType.Thump)
			{
				foreach (var item in Thump_meleeAttackDataWayList)
				{
					if (AnimatorMachine.CheckNextOrCurrentAnimationStateByName(userAnimator, 0, item.animationName))
					{
						meleeAttackData = item;
						return true;
					}
				}
			}
			meleeAttackData = new MeleeAttackData();
			return false;
		}
		public bool IsPlayingAttackAction(WeaponUseInputType inputType)
		{
			if (inputType == WeaponUseInputType.Default)
			{
				foreach (var item in light_meleeAttackDataWayList)
				{
					if (AnimatorMachine.CheckNextOrCurrentAnimationStateByName(userAnimator, 0, item.animationName))
					{
						return true;
					}
				}
			}
			else if (inputType == WeaponUseInputType.Thump)
			{
				foreach (var item in Thump_meleeAttackDataWayList)
				{
					if (AnimatorMachine.CheckNextOrCurrentAnimationStateByName(userAnimator, 0, item.animationName))
					{
						return true;
					}
				}
			}
			return false;
		}
		public bool IsPlayingHandAction()
		{
			foreach (var item in light_meleeAttackDataWayList)
			{
				if (AnimatorMachine.CheckNextOrCurrentAnimationStateByName(userAnimator, 0, item.animationName))
				{
					return true;
				}
			}
			foreach (var item in Thump_meleeAttackDataWayList)
			{
				if (AnimatorMachine.CheckNextOrCurrentAnimationStateByName(userAnimator, 0, item.animationName))
				{
					return true;
				}
			}
			return false;
		}
		private bool TargetIsInAngle(Transform target, float angle)
		{
			if (target == null /*|| TargetDiedOrNULL(target.GetComponent<RoleController>())*/)
			{
				return false;
			}
			Vector3 dir = (target.transform.position - user.transform.position).normalized;
			return (Vector3.Angle(user.transform.position, Vector3.ProjectOnPlane(dir, Vector3.up)) <= angle / 2);
		}

		public override void RenewWeapon()
		{
			base.RenewWeapon();
		}
		public override void FillAllAmmo()
		{

		}
		public override WeaponDataConfig GetItemDataConfig()
		{
			return data;
		}
		public void OnMeleeCheck(WeaponUseInputType useInputType, int comboID = 0)
		{
			MeleeCheckWay checkWay = null;
			switch (useInputType)
			{
				case WeaponUseInputType.Default:
					//light
					checkWay = light_meleeAttackDataWayList[comboID].checkWayData;
					break;
				case WeaponUseInputType.Thump:
					checkWay = Thump_meleeAttackDataWayList[comboID].checkWayData;
					break;
				default:
					break;
			}
			user.OnMeleeCheck(useInputType, checkWay, this, comboID);
		}
		public int GetDamage(Vector3 firePoint, Vector3 hitpoint, float maxAffectDistance, WeaponUseInputType weaponUseInputType, BodyPartMainType hitBodyPartType)
		{
			float damage = data.baseDamage;
			if (weaponUseInputType == WeaponUseInputType.Thump)
			{
				damage *= data.ThumpDamageRate;
			}
			var dis = (firePoint - hitpoint).magnitude;
			float damageRate = 1;
			if (data.affectCurve.keys.Length > 1)
			{
				damageRate = data.affectCurve.Evaluate(Mathf.Clamp01(dis / maxAffectDistance));//最近产生的影响越大
			}
			int result = (int)(damage * damageRate * BodyPartSetting.bodyDamageRateConfig.GetRate(hitBodyPartType));
			return result;
		}

		public float GetMaxAttackDistance()
		{
			float max1 = light_meleeAttackDataWayList.Max((v) => { return v.checkWayData.meleeCheckWayData[0].checkDis; });
			float max2 = Thump_meleeAttackDataWayList.Max((v) => { return v.checkWayData.meleeCheckWayData[0].checkDis; });
			return Mathf.Max(max1, max2);
		}
	}
}
