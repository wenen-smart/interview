/**
 $ @Author       : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @Date         : 2023-01-11 16:04:18
 $ @LastEditors  : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @LastEditTime : 2023-02-01 16:24:14
 $ @FilePath     : \MovementProject\Assets\Resolution\Scripts\PlayerInputController.cs
 $ @Description  : 
 $ @
 $ @Copyright (c) 2023 by unity-mircale 9944586+unity-mircale@user.noreply.gitee.com, All Rights Reserved. 
 **/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using static UnityEngine.InputSystem.InputAction;
[DefaultExecutionOrder(0)]
public class PlayerInputController : MonoSingleTon<PlayerInputController>
{
    CharacterInputActions  _InputActionCtrl;
    //InputSystem 的输入不能平滑处理
    public Vector2 MovementValue{get{return _InputActionCtrl.Default.Movement.ReadValue<Vector2>();}}
    private Vector2 _MovementSmoothValue;
    [SerializeField]private float movementValueSensitivity=3;
    public Vector2 MovementSmoothValue{get{ return _MovementSmoothValue; }}
    /// <summary>
    /// 最近一次的MovementValue
    /// </summary>
    public Vector2 LastMovementValue { get; private set; }
    public bool isInputingMove{get{return _InputActionCtrl.Default.Movement.IsPressed();}}
    public bool JustInputedMoveThisFrame{get{return _InputActionCtrl.Default.Movement.WasPressedThisFrame();}}
    public bool JustReleasedMoveThisFrame{get{return _InputActionCtrl.Default.Movement.WasReleasedThisFrame();}}

    public bool isHoldingMovementValueState;
    
    public bool JustInputedJumpThisFrame{get{return _InputActionCtrl.Default.Jump.WasPressedThisFrame();}}
    public bool isInputingJump{get{return _InputActionCtrl.Default.Jump.IsPressed();}}

    public Vector2 MouseDelta { get { return _InputActionCtrl.Default.MouseDelta.ReadValue<Vector2>(); } }
    public float MouseAxisX { get { return Input.GetAxis("Mouse X"); } }//Now:old input manager TODO:Replace New InputSystem  
    public float MouseAxisY { get { return Input.GetAxis("Mouse Y"); } }
    public float MouseScroll { get { return _InputActionCtrl.Default.MouseScroll.ReadValue<float>(); } }
    public bool isInputingFire { get { return _InputActionCtrl.Default.Fire.IsPressed(); } }
    public bool isInputedFireThisFrame { get { return _InputActionCtrl.Default.Fire.WasPressedThisFrame(); } }
    public bool JustReleasedFireThisFrame { get { return _InputActionCtrl.Default.Fire.WasReleasedThisFrame(); } }

    public bool isInputingWalk { get { return _InputActionCtrl.Default.Walk.IsPressed();} }
    public bool JustReleasedWalkThisFrame { get { return _InputActionCtrl.Default.Walk.WasReleasedThisFrame(); } }

    public bool isInputingThump { get { return _InputActionCtrl.Default.Thump.IsPressed(); } }
    public bool isInputedThumpThisFrame { get { return _InputActionCtrl.Default.Thump.WasPressedThisFrame(); } }
    public bool JustReleasedThumpThisFrame { get { return _InputActionCtrl.Default.Thump.WasReleasedThisFrame(); } }

    public bool isInputedCrouchThisFrame{ get { return _InputActionCtrl.Default.Crouch.WasPressedThisFrame(); } }
    public bool JustReleasedCrouchThisFrame { get { return _InputActionCtrl.Default.Crouch.WasReleasedThisFrame(); } }

    public bool JustInputedRechargeThisFrame { get { return _InputActionCtrl.Default.Recharge.WasPressedThisFrame(); } }

    public bool JustInputedThrowWeaponThisFrame{ get { return _InputActionCtrl.Default.ThrowWeapon.WasPressedThisFrame(); } }

    private List<ValueSmoothData<object>> smoothValueData_List;
    private void Awake() {
        if (_InputActionCtrl==null)
        {
            _InputActionCtrl=new CharacterInputActions();
        }
    }
    private void Start()
    {
        _InputActionCtrl.Default.Movement.performed += OnMovementInteractionPerformed;
    }
    public void OnMovementInteractionPerformed(CallbackContext callbackContext)
    {
        LastMovementValue = callbackContext.ReadValue<Vector2>();
    }
    private void Update()
    {
        if (_MovementSmoothValue != MovementValue)
        {
            _MovementSmoothValue = Vector2.MoveTowards(_MovementSmoothValue, MovementValue, Time.deltaTime*movementValueSensitivity);
            if ((_MovementSmoothValue - MovementValue).magnitude < 0.001f)
            {
                _MovementSmoothValue = MovementValue;
            }
        }
    }
    private void OnEnable() {
        _InputActionCtrl.Enable();
    }
    private void OnDisable() {
        _InputActionCtrl.Disable();
    }
    
}

public abstract class ValueSmoothData<T> where T:new()
{
    public T current;
    public T target;
    public float delta;

    protected ValueSmoothData(T current, T target, float delta)
    {
        this.current = current;
        this.target = target;
        this.delta = delta;
    }

    public abstract T Lerp();
    public abstract bool Finish(float c);
}
public class Vector3SmoothData : ValueSmoothData<Vector3>
{
    public Vector3SmoothData(Vector3 current, Vector3 target, float delta) : base(current, target, delta)
    {
    }

    public override bool Finish(float c)
    {
        return Mathf.Abs(Vector3.Magnitude(current-target))<c;
    }

    public override Vector3 Lerp()
    {
        return Vector3.Lerp(current,target,delta);
    }
}
public class FloatSmoothData : ValueSmoothData<float>
{
    public FloatSmoothData(float current, float target, float delta) : base(current, target, delta)
    {
    }

    public override bool Finish(float c)
    {
         return Mathf.Abs(current-target)<c;
    }

    public override float Lerp()
    {
        return Mathf.Lerp(current, target, delta);
    }
}

