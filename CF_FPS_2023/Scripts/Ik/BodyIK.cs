using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using UnityEngine.Animations;

public class BodyIK : StateMachineBehaviour
{
    #region UpperBody
    public ArmIKParameter armIkParameter;
    public AimIKParameter aimIKParameter;
    public LookAtIKParameter lookAtIKParamter;
    private BodyIKManager bodyIkManager;
    private ArmIK rightArmIK;
    private LimbIK leftArmIK;
    private LookAtIK LookAtIK;
    private AimIK aimIK;
    private bool DisableIK = false;
    #endregion

    //TODO 统一接口 To BodyIKManager
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bodyIkManager = animator.GetComponent<BodyIKManager>();
        if (bodyIkManager==null)
        {
            DebugTool.DrawWireSphere(animator.bodyPosition,0.1f,Color.red,5,"Error-GetBodyIkManager");
            return;
        }
        aimIK = bodyIkManager.aimIK;
        rightArmIK = bodyIkManager.rightArmIK;
        LookAtIK = bodyIkManager.LookAtIK;
        leftArmIK = bodyIkManager.leftArmIK;
        if (aimIKParameter.isAimIK == false && aimIK)
        {
            aimIK.solver.SetIKPositionWeight(0);
            if (aimIK.enabled == false)
            {
                aimIK.enabled = true;
            }
   
        }
        if (LookAtIK && lookAtIKParamter.isLookAtIK == false)
        {
            LookAtIK.solver.SetIKPositionWeight(0);
            LookAtIK.solver.headWeight = (0);
            LookAtIK.solver.bodyWeight = (0);
            if (LookAtIK.enabled == false)
            {
                LookAtIK.enabled = true;
            }
 
        }
        if (armIkParameter.isRightHandIk == false && rightArmIK)
        {
            rightArmIK.solver.SetIKPositionWeight(0);
            if (rightArmIK.enabled == false)
            {
                rightArmIK.enabled = true;
            }
        }
        if (armIkParameter.isLeftHandIk == false && leftArmIK)
        {
            leftArmIK.solver.SetIKPositionWeight(0);
            leftArmIK.solver.SetIKRotationWeight(0);
            if (leftArmIK.enabled == false)
            {
                leftArmIK.enabled = true;
            }
        }
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (bodyIkManager == null)
        {
            return;
        }
        if (animator.GetLayerWeight(layerIndex).GetNormalizeValue() == 0)
        {
            if (DisableIK==false)
            {
                DisableIK = true;
                bodyIkManager.CloseAllIK();
            }
            return;
        }
        else
        {
            DisableIK = false;
        }
        float normalizeTime=Mathf.Repeat(stateInfo.normalizedTime,1);
        if (lookAtIKParamter.isLookAtIK)
        {
            if (LookAtIK==null)
            {
                Debug.Log("LookAtIK未赋值，请检查");
            }
            else
            {
                LookAtIK.solver.SetIKPositionWeight(lookAtIKParamter.weightCurve.Evaluate(normalizeTime));
                LookAtIK.solver.headWeight = (lookAtIKParamter.headWeightCurve.Evaluate(normalizeTime));
                LookAtIK.solver.bodyWeight = (lookAtIKParamter.bodyWeightCurve.Evaluate(normalizeTime));
                if (LookAtIK.enabled == false)
                {
                    LookAtIK.enabled = true;
                }

            }
            
        }

        if (aimIKParameter.isAimIK&&aimIK)
        {
            aimIK.solver.SetIKPositionWeight(aimIKParameter.weightCurve.Evaluate(normalizeTime));
            if (aimIK.enabled == false)
            {
                aimIK.enabled = true;
            }
        }

        if (armIkParameter.isRightHandIk)
        {
            if (rightArmIK == null)
            {
                Debug.Log("rightArmIK未赋值，请检查");
            }
            else
            {
                rightArmIK.solver.SetIKPositionWeight(armIkParameter.rightHandPositionWeight.Evaluate(normalizeTime));
                if (rightArmIK.enabled == false)
                {
                    rightArmIK.enabled = true;
                }
            }
        }

        if (armIkParameter.isLeftHandIk)
        {
            if (leftArmIK == null)
            {
                Debug.Log("LeftArmIK未赋值，请检查");
            }
            else
            {
                leftArmIK.solver.SetIKPositionWeight(armIkParameter.leftHandPositionWeight.Evaluate(normalizeTime));
                leftArmIK.solver.SetIKRotationWeight(armIkParameter.leftHandRotationWeight.Evaluate(normalizeTime));
                if (leftArmIK.enabled == false)
                {
                    leftArmIK.enabled = true;
                }
            }

        }
    }
}

[System.Serializable]
public struct ArmIKParameter
{
    public bool isLeftHandIk;
    public bool isRightHandIk;

    public AnimationCurve leftHandPositionWeight;
    public AnimationCurve leftHandRotationWeight;

    public AnimationCurve rightHandPositionWeight;
    //public AnimationCurve rightHandRotationWeight;
}
[System.Serializable]
public struct LookAtIKParameter
{
    public bool isLookAtIK;
    public AnimationCurve weightCurve;
    public AnimationCurve bodyWeightCurve;
    public AnimationCurve headWeightCurve;
}
[System.Serializable]
public struct AimIKParameter
{
    public bool isAimIK;
    public AnimationCurve weightCurve;
}