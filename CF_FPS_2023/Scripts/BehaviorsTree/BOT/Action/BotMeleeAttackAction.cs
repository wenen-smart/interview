using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Assets.Resolution.Scripts.Inventory;
using UnityEngine;
using Assets.Resolution.Scripts.Weapon;

[TaskCategory("Bot")]

public class BotMeleeAttackAction : Action
{
	int comboID = -1;
	private RobotController bot;
	private MyRuntimeInventory RuntimeInventory;
	public SharedTransform target;
	private int upperLayerIndex;
	public override void OnAwake()
	{
		base.OnAwake();
		bot = GetComponent<ActorComponent>().GetActorComponent<RobotController>();
		RuntimeInventory = bot.GetActorComponent<MyRuntimeInventory>();
		//upperLayerIndex = bot.animatorMachine.animator.GetLayerIndex("Upper");
	}
	public override void OnStart()
	{
		base.OnStart();
		if (IsMeleeWeapon())
		{
			MeleeWeapon meleeWeapon = RuntimeInventory.currentWeapon as MeleeWeapon;
			meleeWeapon.OutLight();
		}
	}
	public bool IsMeleeWeapon()
	{
		WeaponDataConfig so = RuntimeInventory.currentWeapon.GetItemDataConfig();
		return so.weaponType == WeaponType.Knife || so.weaponType == WeaponType.Empty_Hand;
	}
}
