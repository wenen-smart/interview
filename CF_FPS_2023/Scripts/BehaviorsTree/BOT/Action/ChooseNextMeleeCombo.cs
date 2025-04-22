using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Assets.Resolution.Scripts.Inventory;
using UnityEngine;

[TaskCategory("Bot")]

public class ChooseNextMeleeCombo : Action
{
	bool randomLightCombo=false;
	bool randomThumpCombo=false;
	bool randomAllCombo=false;
	int comboID=-1;
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
		//bot.animatorMachine.animator.SetLayerWeight(upperLayerIndex, 1);
	}
}
