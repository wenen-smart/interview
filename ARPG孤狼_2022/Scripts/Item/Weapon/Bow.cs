using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Bow : IWeapon
{
    public GameObject arrowConsumablePrefab;
    public Transform equipConsumablePoint;//Align Position and Rotation
    private ArrowConsumable currentArrow;
    public override bool DoDamage(IDamageable damageTrans,Vector3 attackPoint,Vector3 _attackDir=default(Vector3))
    {
        return true;
    }

    public override void ComboAttack()
    {
        base.ComboAttack();
    }
    /// <summary>
    /// ��䵯ҩ��ʼ   ��䵯ҩ��һ�����̣��ֿ�ʼ���н���
    /// </summary>
    public override IWeaponConsumable FillConsumables()
    {
        if (currentArrow==null)
        {
            GameObject arrow = GameObjectFactory.Instance.PopItem(arrowConsumablePrefab);
            currentArrow=arrow.GetComponent<ArrowConsumable>();
        }
        currentArrow.Renew();
        currentArrow.gameObject.SetActive(true);
        currentArrow.transform.SetParent(weaponManager.rightConsumablePos,true);
        currentArrow.transform.localPosition = Vector3.zero;
        currentArrow.transform.localEulerAngles = Vector3.zero;
        weaponManager.facade.characterAnim.delayUntilAnimationUpdateFrameFinish += () => { weaponManager.facade.OpenRightArrowPoseRig(); };
        return currentArrow;
    }
    /// <summary>
    /// ��䵯ҩ���� ִ�й̶���ʸ
    /// </summary>
    public void FillConsumableFinish()
    {
       
    }
    public override void UnEquipConsumable()
    {
        if (currentArrow == null)
        {
            return;
        }
        currentArrow.gameObject.SetActive(false);
        GameObjectFactory.Instance.PushItem(currentArrow.gameObject);
        currentArrow = null;
    }
    public virtual void Shoot()
    {
        //��Ļ���ķ������� ��õ�hitPoint 
        //��ʸ����=��hitpoint-��ʸ����
        Vector3 point = weaponManager.facade.roleController.GetAimPoint(currentArrow.transform.position,equipConsumablePoint.rotation);
        
        currentArrow.transform.SetParent(null);
        currentArrow.transform.rotation = Quaternion.LookRotation(point - currentArrow.transform.position);
        currentArrow.caster = weaponManager.facade.roleController;
        currentArrow.Used();
    }

    public override void Equip(bool immediately)
    {
        if (tid != 0)
        {
            GameRoot.Instance.DelTimeTask(tid);
        }
        base.Equip(immediately);
        if (immediately == false)
        {
            if (weaponManager == null)
            {
                weaponManager = GetComponentInParent<WeaponManager>();
            }
            weaponManager.facade.playerStateManager.characterAnim.SetLayerWeight(2, 1, false);

            weaponManager.facade.playerStateManager.characterAnim.SetInt(AnimatorParameter.EquipType.ToString(), 2);
            weaponManager.facade.playerStateManager.characterAnim.SetInt(AnimatorParameter.Equipment.ToString(), 1);
            tid=GameRoot.Instance.AddTimeTask(900, () => { weaponManager.facade.playerStateManager.characterAnim.SetLayerWeight(2, 0, false); });
        }
        else
        {
            weaponManager.facade.playerStateManager.characterAnim.SetLayerWeight(2, 0, false);
        }
    }
    int tid;
    public override void UnEquip(bool immediately)
    {
        UnEquipConsumable();
        if (tid != 0)
        {
            GameRoot.Instance.DelTimeTask(tid);
        }
        if (immediately == false)
        {
            weaponManager.facade.playerStateManager.characterAnim.SetLayerWeight(2, 1, false);
            weaponManager.facade.playerStateManager.characterAnim.SetInt(AnimatorParameter.EquipType.ToString(), 2);
            weaponManager.facade.playerStateManager.characterAnim.SetInt(AnimatorParameter.Equipment.ToString(), 2);
            tid=GameRoot.Instance.AddTimeTask(1100, () => { weaponManager.facade.playerStateManager.characterAnim.SetLayerWeight(2, 0, false); base.UnEquip(immediately);});
        }
        else
        {
            weaponManager.facade.playerStateManager.characterAnim.SetLayerWeight(2, 0, false);
            weaponUnEquipAndUnTake?.Invoke();
            base.UnEquip(immediately);
        }
        ///can appearing bug
    }
    public override Vector3 GetAttackDir()
    {
        return Vector3.zero;
    }
}
