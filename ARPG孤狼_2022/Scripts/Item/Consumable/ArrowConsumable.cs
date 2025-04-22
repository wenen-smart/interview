using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ArrowConsumable:IWeaponConsumable
{
    Rigidbody rigid;
    private bool canMove;
    public float speed = 10;
    private Transform top;
    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        top = transform.Find("Top");
    }
    public override void Renew()
    {
        base.Renew();
        rigid.isKinematic = true;
        canMove = false;
    }
    public override void Used()
    {
        rigid.isKinematic = false;
        GetComponent<BoxCollider>().enabled = true;
        canMove = true;
    }
    public override void AffectLose()
    {
        canMove = false;
    }
    public void FixedUpdate()
    {
        if (canMove)
        {
            rigid.position += transform.forward * speed * Time.fixedDeltaTime;
        }
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        if (canMove==false)
        {
            return;
        }
        if (((1<<other.gameObject.layer)&PlayerableConfig.Instance.arrowCanTriggerCheckLayer.value)!=0)
        {
            bool used = false;
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable)
            {
                if (damageable!=caster.damageable)
                {
                    used = true;
                    damageable.Hit(caster.transform,10,transform.forward,top.position);
                    Transform bone = damageable.characterFacade.GetClosetBoneTransGeneric(top.position);
                    if (bone != null)
                    {
                        transform.SetParent(bone);
                        transform.position = bone.position - (top.position - transform.position);
                        //记得定时销毁
                    }
                    else
                    {
                        transform.position += transform.forward * damageable.characterFacade.roleController.GetCharacterRadius();
                        transform.SetParent(damageable.transform);
                    }

                }
            }
            else
            {
                IWeapon.AttackCanFragmentItem(other,top.position,40);
                used = true;
                canMove = false;
            }
            if (used)
            {
                rigid.isKinematic = true;
                AffectLose();
            }
        }
    }

    public virtual void OnCollisionEnter(Collision other)
    {
        if (canMove == false)
        {
            return;
        }
        AffectLose();
    }
}
