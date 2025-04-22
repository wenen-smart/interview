using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AnimatorMotionDriver))]
public abstract class CharacterAnim : ActorComponent
{

#if UNITY_EDITOR
    [SerializeField, Header("动画采样-测试用")]
    private AnimationClip animationClip;
    [HideInInspector]
    public float sampleClipLength;
    private bool startSampleTest;
    [HideInInspector]
    public float sampleSpeed=1;
#endif

    [HideInInspector]
    public Animator anim;
    public List<MonoBehaviour> receiveIKMonoList = new List<MonoBehaviour>();
    public AnimatorStateInfo stateInfo;
    public AnimatorStateInfo nextStateInfo;
    [HideInInspector]
    public ActorStateManager stateManager;
    protected AnimatorMotionDriver motionDriver;
    [HideInInspector]
    public CharacterFacade characterFacade;
    public bool leftHandIK;
    public bool rightHandIK;
    public bool leftFeetIK;
    public bool rightFeetIK;
    //When animationUPdateFrame finish start Excute Action and set null；
    public Action delayUntilAnimationUpdateFrameFinish;
    public override void Awake()
    {
        base.Awake();
    }
    public override void Init()
    {
        base.Init();
        characterFacade = GetActorComponent<CharacterFacade>();
        stateManager = characterFacade.playerStateManager;
        motionDriver = GetActorComponent<AnimatorMotionDriver>();
        anim = GetComponent<Animator>();
    }
    public override void Start()
    {
        base.Start();
    }
    public void RegisterReceiveIK(MonoBehaviour mono)
    {
        if (receiveIKMonoList != null && receiveIKMonoList.Contains(mono) == false)
        {
            receiveIKMonoList.Add(mono);
        }
    }
    public virtual void Update()
    {
        
    }
    public virtual void LateUpdate()
    {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        nextStateInfo = anim.GetNextAnimatorStateInfo(0);
        delayUntilAnimationUpdateFrameFinish?.Invoke();
        delayUntilAnimationUpdateFrameFinish = null;
        HandlerRigOrIK();
    }
    /// <summary>
    /// AnimationRigging的权重在动画帧被限制死掉了，猜测官方代码逻辑上 为每个Rig限定在动画帧前后的权重保持一致（保存临时变量）
    /// 解决方案：凡事涉及到动画帧上去处理权重，一律推迟到动画帧结束后，这里推迟到LateUpdate。
    /// </summary>
    public virtual void HandlerRigOrIK()
    {
        if (UnEquipFinish)
        {
            UnEquipFinish = false;
            characterFacade.weaponManager.currentWeapon?.weaponUnEquipAndUnTake?.Invoke();
        }
        if (equipAndTake)
        {
            equipAndTake = false;
            characterFacade.weaponManager.currentWeapon?.weaponEquipAndTake?.Invoke();
        }
    }
    public void OnAnimatorIK(int layerIndex)
    {
        foreach (var item in receiveIKMonoList)
        {
            if (item != null)
            {
                item.SendMessage("OnAnimatorIK", layerIndex, SendMessageOptions.DontRequireReceiver);
            }

        }
    }
    public void SetInt(string parameter, int value)
    {
        anim.SetInteger(parameter, value);
    }
    public void SetFloat(string parameter, float value)
    {
        anim.SetFloat(parameter, value);
    }
    public void SetTrigger(string parameter, bool reset = false)
    {
        if (reset)
        {
            ResetTrigger(parameter);
        }
        anim.SetTrigger(parameter);
    }
    public void ResetTrigger(string parameter)
    {
        anim.ResetTrigger(parameter);
    }
    public void SetBool(string parameter, bool value)
    {
        anim.SetBool(parameter, value);
    }
    public Transform GetBodyTransform(HumanBodyBones humanBodyBones)
    {

        return anim.GetBoneTransform(humanBodyBones);
    }
    public virtual void SwitchEnableAttack(int booleanInt)
    {
        if (booleanInt==1)
        {
            SuperBodyStart();
        }
        else
        {
            SuperBodyFinish();
        }
    }

    public virtual void OpenNextAttackVaildSign()
    {
      
    }
    public virtual void CloseNextAttackVaildSign()
    {
       
    }
    public virtual void AfterShakeStart()
    {
        stateManager.ShakeAfter(true);
    }
    public virtual void AfterShakeFinish()
    {
        stateManager.ShakeAfter(false);

    }
    public virtual void BeforeShakeStart()
    {
        stateManager.ShakeBefore(true);
    }

    public virtual void BeforeShakeFinish()
    {
        stateManager.ShakeBefore(false);
    }

    public virtual void SuperBodyStart()
    {
        stateManager.SuperBody(true);
    }
    public virtual void SuperBodyFinish()
    {
        stateManager.SuperBody(false);
    }
    public virtual void OpenReceiverInput()
    {
        stateManager.SwitchReceiverInput(true);
    }
    public virtual void CloseReceiverInput()
    {
        stateManager.SwitchReceiverInput(false);
    }


    public void StartAllHandIK()
    {
        StartLeftHandIK();
        StartRightHandIK();
    }
    public void CloseAllHandIK()
    {
        CloseLeftHandIK();
        CloseRightHandIK();
    }
    public void CloseAllFeetIK()
    {
        CloseLeftFeetIK();
        CloseRightFeetIK();
    }
    public void OpenHandAndFeetIK()
    {
        StartAllHandIK();
        StartLeftFeetIK();
        StartRightFeetIK();
    }
    public void StartLeftHandIK()
    {
        leftHandIK = true;
    }
    public void StartRightHandIK()
    {
        rightHandIK = true;
    }
    public void CloseLeftHandIK()
    {
        leftHandIK = false;
    }
    public void CloseRightHandIK()
    {
        rightHandIK = false;
    }
    public void StartLeftFeetIK()
    {
        leftFeetIK = true;
    }
    public void StartRightFeetIK()
    {
        rightFeetIK = true;
    }
    public void CloseLeftFeetIK()
    {
        leftFeetIK = false;
    }
    public void CloseRightFeetIK()
    {
        rightFeetIK = false;
    }
    public void SetColliderHeightWhenJump()
    {
        GetActorComponent<CharacterFacade>().SetColliderHeightWhenJump();
    }
    public void RenewCollideHeight()
    {
        GetActorComponent<CharacterFacade>().RenewCollideHeight();
    }
    public void SetColliderHeight(float height)
    {
        GetActorComponent<CharacterFacade>().SetColliderHeight(height);
    }
    public abstract void HitEnter();
    public abstract void DieEnter();
    public virtual void OnAnimatorMove()
    {
        motionDriver.AnimatorMove(anim, anim.deltaPosition, anim.deltaRotation);
    }
    public void SetLayerWeight(int layerIndex, float target,bool isLerp = false, Action action = null)
    {
        if (layerIndex <= 0)
        {
            return;
        }
        if (isLerp)
        {
            var cor = StartLerpLayerWeight(layerIndex, target, action);

            if (lerpWeigthCorList.Count >= layerIndex)
            {
                if (lerpWeigthCorList[layerIndex - 1] != null)
                {
                    StopCoroutine(lerpWeigthCorList[layerIndex - 1]);
                    lerpWeigthCorList[layerIndex - 1] = cor;

                }
            }
            else
            {
                lerpWeigthCorList.Add(cor);
            }
            StartCoroutine(cor);

        }
        else
        {
            anim.SetLayerWeight(layerIndex, target);
            action?.Invoke();
        }
    }
    public float lerpWeightSpeed;
    private List<IEnumerator> lerpWeigthCorList = new List<IEnumerator>() { };
    public IEnumerator StartLerpLayerWeight(int layerIndex, float target,Action action)
    {
        while (true)
        {
            var v = Mathf.Lerp(anim.GetLayerWeight(layerIndex), target, Time.deltaTime * lerpWeightSpeed);

            if (Mathf.Abs(v - target) < 0.05f)
            {
                anim.SetLayerWeight(layerIndex, target);
                action?.Invoke();
                break;
            }
            anim.SetLayerWeight(layerIndex, v);
            yield return 0;
        }
    }
    
    public virtual void PlayWeaponEffect(int id)
    {

    }
    public bool equipAndTake = false;//Trigger 实际动画 收武器的那一刻触发的委托 需要配置动画Event
    public bool UnEquipFinish = false;//Trigger 实际动画 拿武器 拔出来的那一刻触发的委托 需要配置动画Event
    public void EquipAndTakeWeapon()
    {
        equipAndTake = true;
    }
    public void UnEquipWeaponFinish()
    {
        UnEquipFinish = true;
    }
    public void ShowWeaponRenderer()
    {
        characterFacade.weaponManager.currentWeapon.ShowRenderer();
    }
    public void HideWeaponRenderer()
    {
        characterFacade.weaponManager.currentWeapon.HideRenderer();
    }

    public void SetAnimatorController(RuntimeAnimatorController runtimeAnimatorController)
    {
        anim.runtimeAnimatorController=runtimeAnimatorController;
        motionDriver?.InitRootMotionConfigs();
    }
    public void FillWeaponConsumables()
    {
        characterFacade.weaponManager.currentWeapon?.FillConsumables();
    }
    public void FillWeaponConsumableFinish()
    {
        characterFacade.weaponManager.currentWeapon?.SendMessage("FillConsumableFinish",SendMessageOptions.DontRequireReceiver);
    }
    public void Shoot()
    {
        characterFacade.weaponManager.currentWeapon?.SendMessage("Shoot", SendMessageOptions.DontRequireReceiver);
    }
#if UNITY_EDITOR
    public void CtrlSampleAnimationInCharacter(bool state)
    {
        startSampleTest = state;
        if (state)
        {
            if (gameObject.GetComponent<AutoSampleClipEditor>()==null)
            {
                gameObject.AddComponent<AutoSampleClipEditor>();
            }
        }
        else
        {
            if (gameObject.GetComponent<AutoSampleClipEditor>() != null)
            {
                DestroyImmediate(gameObject.GetComponent<AutoSampleClipEditor>());
            }
        }
    }
    public void SampleAnimationInCharacter(float length)
    {
        animationClip.SampleAnimation(gameObject,length);
    }

    public void ResetSample()
    {
        sampleClipLength = 0;
        sampleSpeed = 1;
        CtrlSampleAnimationInCharacter(false);
        var info = gameObject.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
        if (info!=null&&info.Length>0)
        {
            if (info[0].clip!=null)
            {
                info[0].clip.SampleAnimation(gameObject,0);
            }
        }
    }
    public void EditorUpdate()
    {
        if (startSampleTest)
        {
            animationClip.SampleAnimation(gameObject, sampleClipLength);
            sampleClipLength += Time.deltaTime*sampleSpeed;
            if (sampleClipLength >= animationClip.length)
            {
                animationClip.SampleAnimation(gameObject, animationClip.length);
                sampleClipLength = 0;
                CtrlSampleAnimationInCharacter(false);
            }
        }
    }
    public void DeploySkill()
    {
        characterFacade.attackHandler.Invoke();
    }
    public void PlayActorAudio(string identity)
    {
        characterFacade.roleController.PlayActorAudio(identity);
    }
#endif
}
