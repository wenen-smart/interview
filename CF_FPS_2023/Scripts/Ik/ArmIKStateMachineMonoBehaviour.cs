using RootMotion.FinalIK;
using System;
using UnityEngine;

public class ArmIKStateMachineMonoBehaviour:StateMachineBehaviour
{
    public ArmIKParameter armIkParameter;
    private ArmIK rightArmIK;
    private LimbIK leftArmIK;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rightArmIK = animator.GetComponent<ArmIK>();
        leftArmIK = animator.GetComponent<LimbIK>();
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
        float normalizeTime=Mathf.Repeat(stateInfo.normalizedTime,1);
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
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        leftArmIK.enabled = false;
        rightArmIK.enabled = false;
    }
}
