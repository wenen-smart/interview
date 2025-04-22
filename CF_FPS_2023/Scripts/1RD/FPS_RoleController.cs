using Assets.Resolution.Scripts.Inventory;
using Assets.Resolution.Scripts.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum RoleBehaviourType
{
    Stand,
    Shot,
}
public enum ControllerState
{
    ///// <summary>
    ///// Controller state disabled.
    ///// </summary>
    //None = 0,

    /// <summary>
    /// Controller is idle.
    /// </summary>
    Idle = 1 << 0,

    /// <summary>
    /// Controller is walking.
    /// </summary>
    Walking = 1 << 1,

    /// <summary>
    /// Controller is running.
    /// </summary>
    Running = 1 << 2,

    /// <summary>
    /// Controller is sprinting.
    /// </summary>
    Sprinting = 1 << 3,

    /// <summary>
    /// Controller is jumping.
    /// </summary>
    Jumped = 1 << 4,

    /// <summary>
    /// Controller in air.
    /// </summary>
    InAir = 1 << 5,

    /// <summary>
    /// Controller is idle crouching.
    /// </summary>
    Crouched = 1 << 6,
    /// <summary>
    /// Controller is zomming.
    /// </summary>
    Zooming = 1 << 7,
}
public enum RoleLocomotionType
{
    Idle,
    Walk,
    Run,
    Jump,
}
public  partial class RoleController
{
    //public RoleBehaviourType behaviourType;
    //public RoleLocomotionType locomotionType;
    [SerializeField,MultSelectTags]private ControllerState _controllerState;
    public ControllerState controllerState { get=>_controllerState; protected set { _controllerState = value; } }
    public Animator armAnimator { get; set; }
    [HideInInspector] public MyRuntimeInventory RuntimeInventory;
    public abstract bool isRecharging { get;  }
    public abstract bool isPullBoltting { get; }
    public UnitCamp unit;
    [NonSerialized]public TeamCommunication team;
    public LayerMask itemLayerMask;
    public Transform head { get; protected set; }

    public RoleDataConfig roleDataConfig;
    public abstract Vector3 GetPreScansTargetPoint(float maxDis);
    /// <summary>
    /// 后面会有系统调用。
    /// </summary>
    public abstract void PlayerAwakeOnRoundGameStart();
    public void UpdateArmAnimator(Animator animator)
    {
        armAnimator = animator;
    }
    public abstract void FindTarget(Vector3 targetPoint);
    public TeamCommunication GetTeamForEditor()
    {
        if (team!=null)
        {
            return team;
        }
        return RuntimeGame.Instance.GetTeam(unit);
    }
    public UnitCamp GetEnemyUnitTag()
    {
        if (unit == UnitCamp.A)
        {
            return  UnitCamp.B;
        }
        else
        {
            return UnitCamp.A;
        }
    }
    public abstract void ThrowOutWeapon() ;
    public abstract void ThrowOutWeapon(BaseWeapon weapon);
     public abstract void ReceiveFlashGrenade();
    public abstract void AttachWeapon(GameObject go,string attachPoint="");
    public abstract bool CheckItem(out Feed feed);
    protected void FindHead()
    {
        head = animatorMachine.animator.GetBoneTransform(HumanBodyBones.Head);
    }
    public abstract void OnMeleeCheck(WeaponUseInputType useInputType,MeleeCheckWay checkWay,MeleeWeapon meleeWeapon,int comboID=0);
    public abstract void AddWeaponInRuntimeInventory(BaseWeapon weapon);
}

