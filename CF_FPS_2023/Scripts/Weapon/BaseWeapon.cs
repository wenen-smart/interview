using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Resolution.Scripts.Weapon
{
    public abstract class BaseWeapon : MonoBehaviour
    {
        public Renderer[] weaponRenderList;
        [HideInInspector]public Renderer[] allRenderList;
        public abstract int baseDamage { get; }
        public abstract WeaponType weaponType { get; }
        protected RoleController user;
        protected bool userIsPlayer;
        public Animator userAnimator { get; private set; }
        public AnimatorMachine userAnimatorMachine{ get; private set; }
        public float takeOutSeconds=0.3f;
        public Action OnGetEventHandler;
        public Action OnHideEventHandler;
        [HideInInspector] public WeaponIKInfo weaponIK;
        [HideInInspector] public WeaponAnimationParameterComponent AnimationParameterComponent;
        public float maxAttackDistance;
        public virtual void Equiped(RoleController _user)
        {
            this.user = _user;
            userIsPlayer = _user is FPS_PlayerController;
             gameObject.SetActive(true);
            if (userIsPlayer)
            {
                userAnimatorMachine = GetComponent<AnimatorMachine>();
                userAnimator = GetComponent<Animator>();
                user.UpdateArmAnimator(userAnimator);
                userAnimator.CrossFade(WeaponActionType.Get.ToString(), 0);
            }
            else
            {
                userAnimatorMachine = _user.animatorMachine;
                userAnimator = userAnimatorMachine.animator;
            }
            OnGetEventHandler?.Invoke();
            weaponIK = GetComponent<WeaponIKInfo>();
            AnimationParameterComponent = GetComponent<WeaponAnimationParameterComponent>();
        }

        public virtual void UnEqiped()
        {
            //TODO
            user = null;
            gameObject.SetActive(false);
            OnHideEventHandler?.Invoke();
        }
        public virtual void Awake()
        {
            allRenderList = GetComponentsInChildren<Renderer>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        /// <summary>
        /// Init初始化 也会将所有委托数据清空。相当这是一个新的，一切重新开始
        /// </summary>
        public  virtual void RenewWeapon()
        {
            OnGetEventHandler = null;
            OnHideEventHandler = null;
        }
        public abstract void FillAllAmmo();
        public abstract bool Use(bool isContinueInput,WeaponUseInputType weaponUseInputType=WeaponUseInputType.Default);

        public abstract WeaponDataConfig GetItemDataConfig();

        public virtual void WeaponRender(bool state)
        {
            foreach (var item in weaponRenderList)
            {
                item.enabled = state;
            }
        }
        public virtual void AllRender(bool state)
        {
            if (allRenderList==null)
            {
                return;
            }
            foreach (var item in allRenderList)
            {
                item.enabled = state;
            }
        }
    }
}
