using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BodyIkEditorAssist:MonoBehaviour
{
    private bool lastPauseState;
    public bool isPauseAnim = false;
    private bool isToChangeAnimTime = false;
    public int testLayer=0;
    public Animator animator;
    [Range(0,1)]
    public float normalizeTime;
    public ArmIKParameter armIkParameter;
    public AimIKParameter aimIKParameter;
    public LookAtIKParameter lookAtIKParamter;
    private BodyIKManager bodyIkManager;
    private ArmIK rightArmIK;
    private LookAtIK LookAtIK;
    private AimIK aimIK;

    public void Awake()
    {
        bodyIkManager = animator.GetComponent<BodyIKManager>();
        aimIK = bodyIkManager.aimIK;
        rightArmIK = bodyIkManager.rightArmIK;
        LookAtIK = bodyIkManager.LookAtIK;
    }
    void OnValidate()
    {
        if (isPauseAnim&&lastPauseState==false)
        {
            lastPauseState = true;
        }
        else
        {
            if (isPauseAnim==false&&lastPauseState)
            {
                animator.speed = 1;
                lastPauseState = false;
            }
        }
        if (isPauseAnim)
        {
            var cur = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(testLayer).normalizedTime, 1);
            int isChange = (cur-normalizeTime).GetNormalizeValue();
            isToChangeAnimTime = (isChange!=0);
        }
        if (aimIKParameter.isAimIK==false && aimIK)
        {
            aimIK.solver.SetIKPositionWeight(0);
            aimIK.UpdateSolverExternal();
        }
        if (LookAtIK&&lookAtIKParamter.isLookAtIK==false)
        {
            LookAtIK.solver.SetIKPositionWeight(0);
            LookAtIK.solver.headWeight = (0);
            LookAtIK.solver.bodyWeight = (0);
            if (LookAtIK.enabled == false)
            {
                LookAtIK.enabled = true;
                LookAtIK.UpdateSolverExternal();
            }
        }
        if (armIkParameter.isRightHandIk==false&&rightArmIK)
        {
            rightArmIK.solver.SetIKPositionWeight(0);
            if (rightArmIK.enabled == false)
            {
                rightArmIK.enabled = true;
                rightArmIK.UpdateSolverExternal();
            }

        }
        UpdateIK();
        //var clip = animator.GetCurrentAnimatorClipInfo(testLayer)[0].clip;
        //clip.SampleAnimation(animator.gameObject, normalizeTime*clip.length);
    }
    public void Update()
    {
        if (isPauseAnim==false)
        {
            normalizeTime = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(testLayer).normalizedTime, 1);
        }
        if (isToChangeAnimTime)
        {
            var current = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(testLayer).normalizedTime, 1);
            var s = normalizeTime - current;
            if (s<0)
            {
                s=(1 - current) + normalizeTime;
            }
            if (animator.updateMode == AnimatorUpdateMode.AnimatePhysics)
            {
                animator.speed = s / Time.fixedDeltaTime;
            }
            else
            {
                animator.speed = s / Time.deltaTime;
            }
            TimeSystem.Instance.AddFrameTask(1, () => { animator.speed = 0;
                 });
            isToChangeAnimTime = false;
        }
        UpdateIK();
    }
    public void UpdateIK()
    {
        bodyIkManager?.Evaulate(armIkParameter,lookAtIKParamter,aimIKParameter,normalizeTime);
    }
}
