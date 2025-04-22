 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Resolution.Scripts.Weapon
{

    public class ThrowItem_Weapon : BaseWeapon
    {
        public GameObject throwItemPrefab;
        private GameObject[] renderThrowItem;
        public Transform throwPoint;
        public float force=10;
        private ThrowItemAmmoEntity itemAmmoEntity;
        public int ammoAmount = 3;
        public ThrowItemDataConfig data;
        public override int baseDamage => data.baseDamage;
        public override WeaponType weaponType => data.weaponType;

        public override void Awake()
        {
            base.Awake();
        }
        public override void Equiped(RoleController _user)
        {
            base.Equiped(_user);
            foreach (var item in weaponRenderList) item.gameObject.SetActive(true);
        }
        public override bool Use(bool isContinueInput, WeaponUseInputType weaponUseInputType = WeaponUseInputType.Default)
        {
            if (isContinueInput==false)
            {
                if (weaponUseInputType == WeaponUseInputType.Default)
                {
                    if (!AnimatorMachine.CheckAnimationStateByName(userAnimator, 0, WeaponActionType.Shot.ToString()))
                    {
                        userAnimator.ResetTrigger("Throw");
                        userAnimator.SetTrigger("Throw");
                        GameObject throwItem = GameObjectFactory.Instance.PopItem(throwItemPrefab);//TODO
                        
                        itemAmmoEntity = throwItem.GetComponent<ThrowItemAmmoEntity>();
                        throwItem.gameObject.SetActive(false);
                        itemAmmoEntity.IgnoreAllCollision = false;
                        itemAmmoEntity.PreThrow(user,data);
                    }
                    return true;
                }
            }
            return false;
        }
        public void OnThrowResult()
        {
            if (itemAmmoEntity)
            {
                foreach (var item in weaponRenderList) item.gameObject.SetActive(false);
                itemAmmoEntity.gameObject.SetActive(true);
                itemAmmoEntity.transform.position = throwPoint.position;
                itemAmmoEntity?.ThrowOut(Camera.main.transform.forward, force);
                itemAmmoEntity = null;
                ammoAmount--;
                if (ammoAmount<=0)
                {
                    user.RuntimeInventory.RemoveWeaponRuntimeInventory(this);

                }
                else
                {
                    foreach (var item in weaponRenderList) item.gameObject.SetActive(true);
                }
            }
        }
        public override void RenewWeapon()
        {
            base.RenewWeapon();
        }
        public override void FillAllAmmo()
        {
            ammoAmount = 1;
        }
        public override WeaponDataConfig GetItemDataConfig()
        {
            return data;
        }
    }

}