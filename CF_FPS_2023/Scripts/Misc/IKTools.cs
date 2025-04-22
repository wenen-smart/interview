using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTools : MonoBehaviour
{
    private Animator animator;

    [Range(0, 2)] [SerializeField] private float heightFromGroundRaycast = 1.2f; //从地面向上的cast距离
    [Range(0, 2)] [SerializeField] private float raycastDownDistance = 1.5f; //向下cast 距离
    [SerializeField] private LayerMask environmentLayer; //检测layer
    [Range(0, 1)] [SerializeField] private float FromFootBoneToBottomOffset = 0f; //从脚的骨骼处到脚底部的偏移

    public float feetToIKTargetSpeed=1;
    public float movePelvisToIKTargetSpeed=1;

    public bool enableFootIK;
    private bool is_openAfterStayUpdate=false;
    public bool enableFookIkRotation;
    public bool isUpdatePelvis=true;
    private Vector3 _leftFootIKPosition;
    private Vector3 _rightFootIKPosition;
    private Quaternion _leftFootIkRotation;
    private Quaternion _rightFootIKRotation;
    private float _lastLeftFootIKPositionY;
    private float _lastRightFootIKPositionY;
    private float _lastPelvisIKPositionY;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (enableFootIK)
        {
            StartFootIK();
        }
    }
    private void Update()
    {
        if (enableFootIK == false) return;
        if (is_openAfterStayUpdate==false)
        {
            is_openAfterStayUpdate = true;
        }
        AdjustFeetCastOrigin(ref _leftFootIKPosition,HumanBodyBones.LeftFoot);
        AdjustFeetCastOrigin(ref _rightFootIKPosition,HumanBodyBones.RightFoot);

        CalculateFootTarget(_leftFootIKPosition,ref _leftFootIKPosition,ref _leftFootIkRotation);
        CalculateFootTarget(_rightFootIKPosition,ref _rightFootIKPosition,ref _rightFootIKRotation);
    }
    private void AdjustFeetCastOrigin(ref Vector3 footIKPosition,HumanBodyBones humanBodyBones)
    {
        footIKPosition = animator.GetBoneTransform(humanBodyBones).position;
        footIKPosition.y = transform.position.y+heightFromGroundRaycast;
    }
    private void CalculateFootTarget(Vector3 castOrigin,ref Vector3 feetIkPosition,ref Quaternion feekIkRotation)
    {
        if (Physics.Raycast(castOrigin,Vector3.down,out RaycastHit hitinfo,raycastDownDistance+heightFromGroundRaycast,environmentLayer))
        {
             feetIkPosition = castOrigin;
            feetIkPosition.y = (hitinfo.point.y);
            feekIkRotation=Quaternion.FromToRotation(Vector3.up,hitinfo.normal)*transform.rotation;
            return;
        }
        feetIkPosition = Vector3.zero; //没有hit，归零
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (!(enableFootIK&&is_openAfterStayUpdate)) return;

        if (isUpdatePelvis)
        {
            UpdatePelvisHeight();
            //After...
        }

        if (enableFookIkRotation)
        {
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
        }
        else
        {
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
        }


        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        UpdateFootToPoint(AvatarIKGoal.LeftFoot, _leftFootIKPosition, _leftFootIkRotation,ref _lastLeftFootIKPositionY);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
        UpdateFootToPoint(AvatarIKGoal.RightFoot, _rightFootIKPosition, _rightFootIKRotation,ref _lastRightFootIKPositionY);
        

    }
    private void UpdatePelvisHeight()
    {
        if (_leftFootIKPosition == Vector3.zero || _rightFootIKPosition == Vector3.zero || _lastPelvisIKPositionY == 0)
        {
            _lastPelvisIKPositionY = animator.bodyPosition.y;

            return;
        }
        float lf_offset = _leftFootIKPosition.y - transform.position.y;
        float rf_offset = _rightFootIKPosition.y - transform.position.y;

        //choose lesser offset Distance
        float offset = lf_offset < rf_offset ? lf_offset : rf_offset;
        Vector3 newPelvisPosition = animator.bodyPosition + Vector3.up * offset; //updatePelvisPosition
        var currentBody_Y = Mathf.Lerp(_lastPelvisIKPositionY, newPelvisPosition.y, movePelvisToIKTargetSpeed*Time.deltaTime);
        newPelvisPosition.y = currentBody_Y;
        _lastPelvisIKPositionY = currentBody_Y;
        animator.bodyPosition = newPelvisPosition;
    }
    private void UpdateFootToPoint(AvatarIKGoal avatarIKGoal,Vector3 footPosition,Quaternion footRotation,ref float lastFootIKPositionY)
    {
        Vector3 targetIkPosition = animator.GetIKPosition(avatarIKGoal); //获取animator IK Goal 的 原本 pos
        if (footPosition != Vector3.zero)
        {
            //targetIkPosition.y = footPosition.y+FromFootBoneToBottomOffset;
            var currentFootY = Mathf.Lerp(lastFootIKPositionY,footPosition.y+FromFootBoneToBottomOffset,feetToIKTargetSpeed*Time.deltaTime);
            targetIkPosition.y = currentFootY;
            animator.SetIKPosition(avatarIKGoal, targetIkPosition);
            animator.SetIKRotation(avatarIKGoal, footRotation);
            lastFootIKPositionY = currentFootY;
        }
    }
    private void StartFootIK()
    {
        if (enableFootIK==false)
        {
            is_openAfterStayUpdate = false;
        }
        enableFootIK = true;
        _lastLeftFootIKPositionY = animator.GetBoneTransform(HumanBodyBones.LeftFoot).position.y;
        _lastRightFootIKPositionY= animator.GetBoneTransform(HumanBodyBones.RightFoot).position.y;
        _lastPelvisIKPositionY = 0;
    }
    private void CloseFootIK()
    {
        enableFootIK = false;
    }
}
[Serializable]
public struct IKInfo
{
    public Vector3 point;
    public Vector3 normal;
    public Collider collider;
    public Quaternion ikRotation;
    public IKInfo(RaycastHit hitInfo,Quaternion ikRotation)
    {
        this.point = hitInfo.point;
        this.normal = hitInfo.normal;
        this.collider = hitInfo.collider;
        this.ikRotation = ikRotation;
    }
    public IKInfo(Vector3 point, Vector3 normal, Collider collider,Quaternion ikRotation)
    {
        this.point = point;
        this.normal = normal;
        this.collider = collider;
        this.ikRotation = ikRotation;
    }
    public bool ConvertTo(Vector3 point, Vector3 normal, Collider collider,Quaternion ikRotation)
    {
        this.point = point;
        this.normal = normal;
        this.collider = collider;
        this.ikRotation = ikRotation;
        return true;
    }
    public bool ConvertTo(RaycastHit hitInfo,Quaternion ikRotation)
    {
        this.point = hitInfo.point;
        this.normal = hitInfo.normal;
        this.collider = hitInfo.collider;
        this.ikRotation = ikRotation;
        return true;
    }
}
