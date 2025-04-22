using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
[ExecuteAlways]
public class AutoSampleClipEditorAssist : MonoBehaviour
{

    [SerializeField, Header("动画采样-测试用")]
    private AnimationClip animationClip;
    public bool ChooseSelfTransformAnimator=true;
    [ Header("在初始帧时是否自动采样")]
    public bool isAwakeAutoSample=false;
    [HideInInspector]public Animator animator;
    [HideInInspector]
    public float sampleClipLength;
    public float sampleClipPercent;
    private bool startSampleTest;
    [HideInInspector]
    public float sampleSpeed = 1;
    public float currentClipPercent;
    public AutoSampleClipPlayAnimationType playAnimationType;

    public void Awake()
    {
        if (Application.isPlaying)
        {
            if (isAwakeAutoSample)
            {
                CtrlSampleAnimationInCharacter(true);
            }
        }
        
    }
    public void OnValidate()
    {
        if (ChooseSelfTransformAnimator==true)
        {
            animator = GetComponent<Animator>();
        }
    }
    public void Update()
    {
        EditorUpdate();
    }
    void OnDrawGizmosSelected()
    {
        // Your gizmo drawing thing goes here if required...

#if UNITY_EDITOR
        // Ensure continuous Update calls.
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
#endif
    }
    public void CtrlSampleAnimationInCharacter(bool state)
    {
        startSampleTest = state;
        animator.GetComponent<Animator>().enabled = !state;
    }
    public void SampleAnimationInCharacter(float length)
    {
        sampleClipPercent = sampleClipLength / animationClip.length;
        animationClip.SampleAnimation(animator.gameObject, length);
    }

    public void ResetSample()
    {
        sampleClipLength = 0;
        sampleSpeed = 1;
        CtrlSampleAnimationInCharacter(false);
        var info = animator.GetCurrentAnimatorClipInfo(0);
        if (info != null && info.Length > 0)
        {
            if (info[0].clip != null)
            {
                info[0].clip.SampleAnimation(animator.gameObject, 0);
            }
        }
    }
    public void ClearTransfromInfo()
    {
        animator.transform.localPosition = new Vector3(0, 0, 0);
        animator.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
    public void PauseAnimation()
    {
        startSampleTest = false;
        animator.speed = 0;
    }
    public void EditorUpdate()
    {
        if (enabled==false)
        {
            return;
        }
        if (startSampleTest)
        {
            animationClip.SampleAnimation(animator.gameObject, sampleClipLength);
            sampleClipLength += Time.deltaTime * sampleSpeed;
            sampleClipPercent = sampleClipLength / animationClip.length;
            if (sampleClipLength >= animationClip.length)
            {
                animationClip.SampleAnimation(animator.gameObject, animationClip.length);
                ClipFinish(playAnimationType);
            }
        }
        currentClipPercent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
    public void ClipFinish(AutoSampleClipPlayAnimationType animationType)
    {
        switch (animationType)
        {
            case AutoSampleClipPlayAnimationType.Once:
                sampleClipLength = 0;
                sampleClipPercent = 1;
                CtrlSampleAnimationInCharacter(false);
                break;
            case AutoSampleClipPlayAnimationType.Loop:
                sampleClipLength = 0;
                sampleClipPercent = 0;
                break;
            case AutoSampleClipPlayAnimationType.ByClipLoopFiled:
                if (animationClip.isLooping)
                {
                    ClipFinish(AutoSampleClipPlayAnimationType.Loop);
                }
                else
                {
                    ClipFinish(AutoSampleClipPlayAnimationType.Once);
                }
                break;
            default:
                break;
        }
    }

}
public enum AutoSampleClipPlayAnimationType
{
    Once,
    Loop,
    ByClipLoopFiled
}
#endif