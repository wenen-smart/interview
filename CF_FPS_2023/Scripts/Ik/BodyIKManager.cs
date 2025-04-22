using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyIKManager : MonoBehaviour
{
    public ArmIK rightArmIK;
    public LimbIK leftArmIK;

    public LookAtIK LookAtIK;
    public AimIK aimIK;

    public void Evaulate(ArmIKParameter armIkParameter,LookAtIKParameter lookAtIKParamter,AimIKParameter aimIKParameter,float normalizeTime)
    {
        normalizeTime = Mathf.Repeat(normalizeTime, 1);
        if (lookAtIKParamter.isLookAtIK)
        {
            if (LookAtIK == null)
            {
                Debug.Log("LookAtIKÎ´¸³Öµ£¬Çë¼ì²é");
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
                //LookAtIK.UpdateSolverExternal();
            }

        }

        if (aimIKParameter.isAimIK && aimIK)
        {
            aimIK.solver.SetIKPositionWeight(aimIKParameter.weightCurve.Evaluate(normalizeTime));
            if (aimIK.enabled == false)
            {
                aimIK.enabled = true;
            }
            //aimIK.UpdateSolverExternal();
        }

        if (armIkParameter.isRightHandIk)
        {
            if (rightArmIK == null)
            {
                Debug.Log("rightArmIKÎ´¸³Öµ£¬Çë¼ì²é");
            }
            else
            {
                rightArmIK.solver.SetIKPositionWeight(armIkParameter.rightHandPositionWeight.Evaluate(normalizeTime));
                if (rightArmIK.enabled == false)
                {
                    rightArmIK.enabled = true;
                }
                //rightArmIK.UpdateSolverExternal();
            }
        }

        if (armIkParameter.isLeftHandIk)
        {
            if (leftArmIK == null)
            {
                Debug.Log("LeftArmIKÎ´¸³Öµ£¬Çë¼ì²é");
            }
            else
            {
                leftArmIK.solver.SetIKPositionWeight(armIkParameter.leftHandPositionWeight.Evaluate(normalizeTime));
                
                if (leftArmIK.enabled == false)
                {
                    leftArmIK.enabled = true;
                }
                //leftArmIK.UpdateSolverExternal();
            }

        }
    }

    public void CloseAllIK(bool immeSolve=false)
    {
        
        leftArmIK.solver.SetIKPositionWeight(0);
        leftArmIK.solver.SetIKRotationWeight(0);
        rightArmIK.solver.SetIKPositionWeight(0);
        rightArmIK.solver.SetRotationWeight(0);
        LookAtIK.solver.SetIKPositionWeight(0);
        LookAtIK.solver.headWeight = (0);
        LookAtIK.solver.bodyWeight = (0);
        aimIK.solver.SetIKPositionWeight(0);
        leftArmIK.UpdateSolverExternal();
        rightArmIK.UpdateSolverExternal();
        LookAtIK.UpdateSolverExternal();
        aimIK.UpdateSolverExternal();
        leftArmIK.enabled = false;
        rightArmIK.enabled = false;
        LookAtIK.enabled = false;
        aimIK.enabled = false;

    }
}
