

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Assets.Resolution.Scripts.Inventory;
using UnityEngine;

[TaskCategory("Bot")]
public class Bot_FireAction:Action
{
    private RobotController bot;
    private MyRuntimeInventory RuntimeInventory;
    public SharedTransform target;
    private int upperLayerIndex;

    public override void OnAwake()
    {
        base.OnAwake();
        bot = GetComponent<ActorComponent>().GetActorComponent<RobotController>();
        RuntimeInventory = bot.GetActorComponent<MyRuntimeInventory>();
        upperLayerIndex = bot.animatorMachine.animator.GetLayerIndex("Upper");
    }
    public override void OnStart()
    {
        base.OnStart();
        bot.animatorMachine.animator.SetLayerWeight(upperLayerIndex, 1);
    }
    public override TaskStatus OnUpdate()
    {
        if (target==null||target.Value==null)
        {
            bot.animatorMachine.animator.SetLayerWeight(upperLayerIndex,0);
            return TaskStatus.Failure;
        }
        if (RuntimeInventory.currentWeapon.weaponIK && RuntimeInventory.currentWeapon.weaponIK.isAimIK)
        {
            if (target.Value != null)
            {
                bot.targetPoint.position = target.Value.position + UnityEngine.Vector3.up * 1.6f;
                bot.aimIK.solver.SetIKPositionWeight(Mathf.Lerp(bot.aimIK.solver.GetIKPositionWeight(), 1, bot.aimIkLerpSpeed * Time.deltaTime));
                if (((RuntimeInventory.currentGunWeapon != null && (RuntimeInventory.currentGunWeapon.HaveAmmo || (RuntimeInventory.currentGunWeapon.HaveAmmo == false && RuntimeInventory.currentGunWeapon.JustEmptyAmmo == true))) || RuntimeInventory.currentGunWeapon == null) && RuntimeInventory.currentWeapon.Use(false))
                {
                    Debug.Log("HitPlayer");
                }
            }
            else
            {
                bot.aimIK.solver.SetIKPositionWeight(0);
            }
        }
        return TaskStatus.Success;
    }
    public override void OnEnd()
    {
        base.OnEnd();
    }
    public override void OnConditionalAbort()
    {
        base.OnConditionalAbort();
        bot.animatorMachine.animator.SetLayerWeight(upperLayerIndex, 0);
        bot.ResetDefaultAimIKPose();
    }
}

