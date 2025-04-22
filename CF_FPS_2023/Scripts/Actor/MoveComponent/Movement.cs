/**
 $ @Author       : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @Date         : 2023-01-09 15:20:12
 $ @LastEditors  : unity-mircale 9944586+unity-mircale@user.noreply.gitee.com
 $ @LastEditTime : 2023-02-01 17:04:03
 $ @FilePath     : \MovementProject\Assets\Resolution\Scripts\Movement.cs
 $ @Description  : 
 $ @
 $ @Copyright (c) 2023 by unity-mircale 9944586+unity-mircale@user.noreply.gitee.com, All Rights Reserved. 
 **/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(250)]
public class Movement : MonoBehaviour
{
    
    [HideInInspector]public Vector3 deltaPosition;
    [HideInInspector]public Quaternion deltaRotation=Quaternion.identity;
    [HideInInspector]public float vertical;
    /*[HideInInspector] */public Vector3 increase_Velocity;
    public Rigidbody rigid;
    public CapsuleCollider capsuleCollider;
    [Range(0, 1),Header("���߱�"),Tooltip("���ﲽ�߱�ָ���������ĵ���߸߶ȵİٷֱȡ�\n�߽�:\n��������Ϊ������Сֵ��\n����߶ȴ�СΪ�������ֵ��")]
    public float stepHeightRadio=0.2f;
    [Range(0, 1)]
    public float stepFactor;
    [Range(0, 1)]
    public float centerOffset=0.5f;
    public float centerHeight=1.8f;
    public float centerDiameter=0.8f;//��Чֱ��
    public bool _isInGrounded=true;
    private Vector3 lastPosition;
    [HideInInspector] public Vector3 moveDirection { get; private set; }
    public bool useGravity;
    public LayerMask EnvironmentLayer; 
    public bool IsInGrounded
    {
        get
        {
            return _isInGrounded;
        }
        private set
        {
            if (_isInGrounded != value)
            {
                // change and maybe apply event
                _isInGrounded = value;
                changeGroundStateEventHandler?.Invoke(value);
            }
        }
    }
    public event StateChangeDelegate changeGroundStateEventHandler;
    public float gravity=30f;
    public Vector3 predictToGroundVelocity;
    [SerializeField]private bool freedomCtrlGravity=false;
    float safeCastMinDistance=0.001f;
    public float verticalMovement;
    public void AllowFreedomCtrlGravity()
    {
        freedomCtrlGravity = true;
    }
    public void DisallowFreedomCtrlGravity()
    {
        freedomCtrlGravity = false;
    }
    private void Start() {
        Setup();
        ResetColliderConfiguration();
       
    }
    private void Setup()
    {
        rigid=GetComponent<Rigidbody>();
        rigid.useGravity = false;
        capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider==null)
        {
            capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        }
    }
    private void OnValidate()
    {
        if (capsuleCollider==null)
        {
             capsuleCollider = GetComponent<CapsuleCollider>();
        }
        ResetColliderConfiguration();
    }
    public void ResetColliderConfiguration()
    {
        if (capsuleCollider!=null)
        {
            if (centerHeight <= centerDiameter)
            {
                DebugTool.DebugWarning("���õĽ�ɫ��ײ���߶ȵ��ڰ뾶�����顣");
            }
            capsuleCollider.height = centerHeight*(1-stepFactor);
            capsuleCollider.radius = centerDiameter/2;
            capsuleCollider.center = Vector3.up * centerOffset*centerHeight;
            //center-> set-> AdjustCenter:add half height decreament (centerHeight*stepFactor)/2  ���¸�����һ����
            capsuleCollider.center = capsuleCollider.center + Vector3.up*centerHeight * 0.5f*stepFactor;
            if (capsuleCollider.height <= centerDiameter)
            {
                DebugTool.DebugWarning("��̬�������ɫ��ײ���߶ȵ��ڰ뾶���粻ϣ����ˣ����顣");
            }
            ResetCast();
        }
    }
    private void ResetCast()
    {
        SetCastDirection(Vector3.down);
        /*Ĭ�����߳���Ϊ����߶ȵĴ�С���������ﲽ��ָ����������Ϊ��ʼ�㣬����߶�Ϊ���ֵ�� ���ݲ��ߣ��������ߵĳ���*/
        castLength =capsuleCollider.center.y * (1 + stepHeightRadio)/*���ݲ��ߣ��������ߵĳ���*/*(1 + safeCastMinDistance);
        DebugTool.DebugPrint("castlength:"+castLength);
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
        float towardDistance=capsuleCollider.center.y-hitDistance;//��ײ��ײ�������ײλ�õ�λ��
        predictToGroundVelocity = transform.up*(towardDistance/Time.fixedDeltaTime); //v=s/t ��֤ʵ�ʽ�ɫ���ڵ�����
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
        hit = Physics.Raycast(origin, direction, out _hitInfo, castLength,EnvironmentLayer,QueryTriggerInteraction.Ignore);
        //hit = Physics.SphereCast(origin + (Vector3.up * castLength), centerDiameter/2, Vector3.down, out _hitInfo, castLength - centerDiameter/2);
        if (hit)
        {
            hitPoint = _hitInfo.point;
            hitNormal = _hitInfo.normal;
            hitDistance = _hitInfo.distance;
        }
    }

    #endregion

    public void OnFixedUpdate()
    {
        Detection();
        Vector3 _velocity=Vector3.zero;
        if (deltaPosition!=Vector3.zero)
        {
            rigid.MovePosition(rigid.position+deltaPosition);
        }
        if (useGravity==true)
        {
            if (freedomCtrlGravity)
            {
                _velocity += Vector3.up * vertical;
            }
            else
            {
                verticalMovement -= gravity * Time.deltaTime;
            }
        }
        

        if (increase_Velocity!=Vector3.zero)
        {
            _velocity+=increase_Velocity;
            increase_Velocity = Vector3.zero;
        } 
        
        deltaPosition=Vector3.zero;
        if (deltaRotation!=Quaternion.identity)
        {
            var endRotation=rigid.rotation*deltaRotation;
            if (rigid.rotation==Quaternion.identity)
            {
                transform.rotation=endRotation;
            }
            else
            {
                rigid.MoveRotation(endRotation);
            }
        }
        deltaRotation=Quaternion.identity;
        if (IsInGrounded)
        {
            verticalMovement = 0;
        }
        _velocity += Vector3.up * verticalMovement + predictToGroundVelocity;
        rigid.velocity = _velocity;
        moveDirection = Vector3.ProjectOnPlane((rigid.position - lastPosition).normalized,Vector3.up);
        lastPosition = rigid.position;
    }
    public void Move(Vector3 motion,Space space=Space.World)
    {
        if (space==Space.Self)
        {
            motion = transform.TransformVector(motion);
        }
        deltaPosition += motion;
    }
    public  void EnableGravity(bool isGravity)
    {
        useGravity = isGravity;
    }
    public void EnableKinematic(bool isKinematic)
    {
        rigid.isKinematic = isKinematic;
    }
    //0Ϊ�٣�����ֵΪ��  ��0����
    public void EnableGravity(int isGravity)
    {
        useGravity = (isGravity!=0);
    }
    public void EnableKinematic(int isKinematic)
    {
        rigid.isKinematic = (isKinematic!=0);
    }
    public void SetVelocity(Vector3 _velocity)
    {
        rigid.velocity = _velocity + predictToGroundVelocity;
    }
    public float GetIntoAirMinUpVelocity()
    {
        if (IsInGrounded)
        {
            return (((stepFactor+0.02f) * centerHeight)/Time.fixedDeltaTime);
        }
        return 0;
    }
}
