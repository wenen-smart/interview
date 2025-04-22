using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RigidMoveComponent : MoveComponent
{
    Rigidbody rigid;
    private ExertForceData exertForceData;
    private RigidbodyConstraints awake_rigidbodyConstraints;
    protected override Vector3 _mVelocity { get => rigid.velocity; set => rigid.velocity = value; }
    /// <summary>
    /// 为了解决在动画事件中或Timeline中设置rigidbody position无效的问题。原因可能因为在动画帧后RigidPosition又刷新了一下，归置原先的fixedUpdate中计算保存的rigidbodyposition；所以动画帧设置的position无效
    /// </summary>
    private Vector3 _willRigidbodyPosition;
    public override Vector3 POSITION
    {
        get
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return transform.position;
            }
#endif
            if (rigid == null) rigid = GetComponent<Rigidbody>(); return rigid.position;
        }
        set
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                transform.position = value;
                return;
            }
#endif
            if (rigid == null) rigid = GetComponent<Rigidbody>(); _willRigidbodyPosition = value;
        }
    }
    ActorPhyiscal actorPhyiscal;
    private float yV;

    [Range(0, 1), Header("步高比"), Tooltip("这里步高比指以身体中心到最高高度的百分比。\n边界:\n身体中心为步高最小值，\n身体高度大小为步高最大值。")]
    public float stepHeightRadio = 0.2f;
    [Range(0, 1)]
    public float step_Factor = 0.2f;
    [Range(0, 1)]
    public float centerOffset = 0.5f;
    public float centerHeight = 1.8f;
    public float centerDiameter = 0.8f;//有效直径
    [HideInInspector] public Vector3 moveDirection;
    public Vector3 predictToGroundVelocity;
    float safeCastMinDistance = 0.001f;
    public float verticalMovement;


    public override void Init()
    {
        base.Init();
        rigid = GetComponent<Rigidbody>();
        rigid.useGravity = false;
        //rigid.useGravity = (useCustomGravity == false);
        awake_rigidbodyConstraints = rigid.constraints;
        actorPhyiscal = GetActorComponent<ActorPhyiscal>();
        Setup();
        ResetColliderConfiguration();
    }
    public override void CalcuateMove()
    {
        bool isInputVelocity = IsInputVelocity();
        SetAffectForceToggle(isInputVelocity);
        if (isInputVelocity == false)
        {
            return;
        }
        if (IsPlayMotionClip)
        {
            MotionClipTick();
            return;
        }
        if (GetActorComponent<ActorStateManager>().isJump)
        {
            Debug.Log("Jump");
        }
        
        if (allDirectionsSpeedMult>0.001f)
        {
            Detection();
            ////判断是否有外部对_willRigidbodyPosition赋值
            if (_willRigidbodyPosition==Vector3.zero)//没有
            {
                _willRigidbodyPosition = rigid.position;
            }
            _willRigidbodyPosition += deltaPosition * allDirectionsSpeedMult;


            //记得_willRigidbodyPosition 归0
            rigid.velocity = ((transform.forward * baseMoveSpeed * speedMult+ increaseVelo)-CalcuateGravityVelocity())*allDirectionsSpeedMult;
            if (actorPhyiscal.GetCurrentTouchCollider() != null && actorPhyiscal.GetCurrentTouchCollider().gameObject.tag == "Stair")
            {
                rigid.velocity += Vector3.up * 0.01f;
            }
            HandlerCollisionThroughInLocomotionDirection();
        }
        else
        {
            rigid.velocity = Vector3.zero;
            rigid.Sleep();
        }
        if (_willRigidbodyPosition!=Vector3.zero)
        {
            //HandlerColliderThroughInMovePosition();
            rigid.position = _willRigidbodyPosition;
            _willRigidbodyPosition = Vector3.zero;
        }
        deltaPosition = Vector3.zero;
        ClearIncreaseVelo();
        UpdateForce();
    }

    private void Setup()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.useGravity = false;
        capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider == null)
        {
            capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        }
    }
    private void OnValidate()
    {
        if (capsuleCollider == null)
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
        }
        ResetColliderConfiguration();
    }
    public void ResetColliderConfiguration()
    {
        if (capsuleCollider != null)
        {
            if (centerHeight <= centerDiameter)
            {
                MyDebug.DebugWarning("设置的角色碰撞器高度低于半径，请检查。");
            }
            capsuleCollider.height = centerHeight * (1 - step_Factor);
            capsuleCollider.radius = centerDiameter / 2;
            capsuleCollider.center = Vector3.up * centerOffset * centerHeight;
            //center-> set-> AdjustCenter:add half height decreament (centerHeight*stepFactor)/2  上下各缩放一半量
            capsuleCollider.center = capsuleCollider.center + Vector3.up * centerHeight * 0.5f * step_Factor;
            if (capsuleCollider.height <= centerDiameter)
            {
                MyDebug.DebugWarning("动态调整后角色碰撞器高度低于半径，如不希望如此，请检查。");
            }
            ResetCast();
        }
    }
    private void ResetCast()
    {
        SetCastDirection(Vector3.down);
        /*默认射线长度为身体高度的大小，所以这里步高指以身体中心为起始点，身体高度为最大值。 根据步高，增加射线的长度*/
        castLength = capsuleCollider.center.y * (1 + stepHeightRadio)/*根据步高，降低射线的长度*/* (1 + safeCastMinDistance);
        MyDebug.DebugPrint("castlength:" + castLength);
    }

    public void Detection()
    {
        SetCastOrigin(capsuleCollider.bounds.center);
        predictToGroundVelocity = Vector3.zero;
        Cast();
        if (!hit)
        {
            IsInGrounded = false;

            return;
        }
        IsInGrounded = true;
        float towardDistance = capsuleCollider.center.y - hitDistance;//碰撞体底部到到碰撞位置的位移
        predictToGroundVelocity = transform.up * (towardDistance / Time.fixedDeltaTime); //v=s/t 保证实际角色踩在地面上
    }
    #region Detection
    public Vector3 origin;
    public Vector3 direction;
    public float castLength;
    public bool hit;
    private Vector3 hitPoint;
    private Vector3 hitNormal;
    private float hitDistance;


    public void SetCastOrigin(Vector3 _origin)
    {
        origin = _origin;
    }
    public void SetCastDirection(Vector3 _direction)
    {
        direction = _direction;
    }
    public void Cast()
    {
        RaycastHit _hitInfo;
        hit = Physics.Raycast(origin, direction, out _hitInfo, castLength, ~(1 << LayerMask.NameToLayer("Character")));
        //hit = Physics.SphereCast(origin + (Vector3.up * castLength), centerDiameter/2, Vector3.down, out _hitInfo, castLength - centerDiameter/2);
        if (hit)
        {
            hitPoint = _hitInfo.point;
            hitNormal = _hitInfo.normal;
            hitDistance = _hitInfo.distance;
        }
    }

    #endregion
    public override void AddImpluseForce(Vector3 tp_force)
    {
        exertForceData = new ExertForceData() { force = tp_force, forceMode = ForceMode.Impulse, isWorld = true };
    }
    private void UpdateForce()
    {
        if (exertForceData.force==Vector3.zero)
        {
            return;
        }
        rigid.AddForce(exertForceData.force,exertForceData.forceMode);
        exertForceData= default(ExertForceData);
    }
    public override void EnableGravity(bool isGravity)
    {
        base.EnableGravity(isGravity);
        if (useCustomGravity == false)
        {
            rigid.useGravity = isGravity;
        }
    }

    public override Vector3 CalcuateGravityVelocity()
    {
        if (useCustomGravity==false&&ignoreGravity==false)
        {
            return -Physics.gravity*Time.deltaTime;
        }
        return CalcuateCustomGravityVelocity();
    }

    public override void MotionClipTick()
    {
        ApplyMotionClipVelocity();
    }

    public override bool IsInputVelocity()
    {
        if ((transform.forward * baseMoveSpeed * speedMult + increaseVelo) != Vector3.zero || deltaPosition != Vector3.zero || IsPlayMotionClip||rigid.velocity.sqrMagnitude>0.001f||exertForceData.force!=Vector3.zero||_willRigidbodyPosition!=Vector3.zero)
        {
            return true;
        }
        if (IsOnGround()==false)
        {
            return true;
        }
        return false;
    }
    public override void MoveToPositionMode()
    {
        var targetPos = _CurrentRunTimeMotionClip.GetCurrentFramePos();
        rigid.MovePosition(targetPos);
    }

    public Vector3 CorrentForce(Vector3 force)
    {
        return force * rigid.mass;
    }

    public override bool IsOnGround()
    {
        return IsInGrounded;
    }

    public override void SetAffectForceToggle(bool unlock)
    {
        base.SetAffectForceToggle(unlock);
        if (unlock)
        {
            rigid.constraints = awake_rigidbodyConstraints;
        }
        else
        {
            if (rigid.constraints.HasFlag(RigidbodyConstraints.FreezePositionX|RigidbodyConstraints.FreezePositionZ)==false)
            {
                rigid.constraints = awake_rigidbodyConstraints|RigidbodyConstraints.FreezePositionX|RigidbodyConstraints.FreezePositionZ;
            }
        }
    }
    public bool HandlerCollisionThroughInLocomotionDirection()
    {
        Vector3 velocityProject = Vector3.ProjectOnPlane(rigid.velocity, transform.up);
        if (velocityProject.magnitude<=8/*8*/)
        {
            return false;
        }
        Vector3 normalizedDir = velocityProject.normalized;
        //此处仅判断了XZ轴的速度方向。后期如果出现Y轴方向上的运动，需要处理。
        float forecastNextDistance = velocityProject.magnitude * Time.fixedDeltaTime;
        RaycastHit raycastHit;
        bool isCollision = actorPhyiscal.SpherecastFromBodySphere(false,false,normalizedDir,capsuleCollider.radius+forecastNextDistance,-1,out raycastHit,0.1f,QueryTriggerInteraction.Ignore,true,Color.green);
        if (isCollision)
        {
            if (raycastHit.collider==capsuleCollider)
            {
                isCollision = false;
            }
            else
            {
                Vector3 distancePoint = raycastHit.point - POSITION;
                Vector3 disPointProject = Vector3.ProjectOnPlane(distancePoint, transform.up);

                rigid.position = POSITION + normalizedDir * (disPointProject.magnitude - capsuleCollider.radius);
                rigid.velocity = Vector3.zero-CalcuateGravityVelocity();
            }
        }
        return isCollision;
    }
    public void HandlerColliderThroughInMovePosition()
    {        
        float forecastNextDistance = (_willRigidbodyPosition - POSITION).magnitude;
        if (forecastNextDistance<0.1f)
        {
            return;
        }
        Vector3 normalizedDir = (_willRigidbodyPosition - POSITION).normalized;
        RaycastHit raycastHit;
        bool isCollision = actorPhyiscal.SpherecastFromBodySphere(false, false, normalizedDir,forecastNextDistance, -1, out raycastHit, 0.1f, QueryTriggerInteraction.Ignore, true, Color.green);
        if (isCollision)
        {
            if (raycastHit.collider == capsuleCollider)
            {
                isCollision = false;
            }
            else
            {
                Vector3 distancePoint = raycastHit.point - POSITION;
                Vector3 disPointProject = Vector3.ProjectOnPlane(distancePoint, transform.up);

                _willRigidbodyPosition = POSITION + normalizedDir * (disPointProject.magnitude);
            }
        }
    }
    public override void StopNavAgent()
    {
        base.StopNavAgent();
        rigid.isKinematic = false;
    }
    public override void SetNavMeshTarget(Vector3 target)
    {
        base.SetNavMeshTarget(target);
        rigid.isKinematic = true;
    }
    public void SetVelocity(Vector3 _velocity)
    {
        rigid.velocity = _velocity + predictToGroundVelocity;
    }
    public override float GetIntoAirMinUpVelocity()
    {
        if (IsInGrounded)
        {
            return (((step_Factor + 0.02f) * centerHeight) / Time.fixedDeltaTime);
        }
        return 0;
    }
}
public struct ExertForceData
{
    public Vector3 force;
    public ForceMode forceMode;
    public bool isWorld;
}

