using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AutoSampleClipEditorAssist : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField, Header("动画采样-测试用")]
    private AnimationClip animationClip;
    public Animator animator;
    [HideInInspector]
    public float sampleClipLength;
    public float sampleClipPercent;
    private bool startSampleTest;
    [HideInInspector]
    public float sampleSpeed = 1;
    public float currentClipPercent;
    public void Update()
    {
        EditorUpdate();
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
        animator.transform.localPosition = new Vector3(0, 0, 0);
        animator.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
    public void EditorUpdate()
    {
        if (startSampleTest)
        {
            animationClip.SampleAnimation(animator.gameObject, sampleClipLength);
            sampleClipLength += Time.deltaTime * sampleSpeed;
            sampleClipPercent = sampleClipLength / animationClip.length;
            if (sampleClipLength >= animationClip.length)
            {
                animationClip.SampleAnimation(animator.gameObject, animationClip.length);
                sampleClipLength = 0;
                sampleClipPercent = 1;
                CtrlSampleAnimationInCharacter(false);
            }
        }
        if (Application.isPlaying)
        {
            currentClipPercent = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }
    }
}
#endif