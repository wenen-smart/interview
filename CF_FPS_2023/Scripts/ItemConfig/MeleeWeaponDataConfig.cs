using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MeleeWeaponDataConfig : WeaponDataConfig
{
	[Range(1, 2)]
	public float ThumpDamageRate = 1;

	//public List<MeleeAttackData> light_meleeAttackDataWayList;

	//public List<MeleeAttackData> Thump_meleeAttackDataWayList;

	
}
