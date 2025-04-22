using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using static IDamageable;

public class EnemyController: NPCController
{
    private int inVisible_CheckBarTid=-1;
    //public MyTimer hitAfterLookAttackWait=new MyTimer();//多少秒后锁定攻击者
    public override void Awake()
    {
        base.Awake();
    }
    public override void OnRenderVisible()
    {
        base.OnRenderVisible();
        HpBarEntity barEntity = BattleManager.Instance.GetHpBarEntity(damageable);
        if (barEntity)
        {
            barEntity.SetInfo(actorSystem.roleDefinition.object_Name,damageable);
            barEntity.gameObject.SetActive(true);
        }
    }
    public override void OnRenderInVisible()
    {
        base.OnRenderInVisible();
        HpBarEntity barEntity = BattleManager.Instance.GetHpBarEntity(damageable);
        if (barEntity)
        {
            barEntity.gameObject.SetActive(false);
            if (inVisible_CheckBarTid!=-1)
            {
                GameRoot.Instance.DelTimeTask(inVisible_CheckBarTid);
            }
            
            inVisible_CheckBarTid=GameRoot.Instance.AddTimeTask(5000, () =>
            {
                if (renderStateListener.isRenderObject==false)
                {
                    BattleManager.Instance.RemoveHpBarEntity(damageable);
                    inVisible_CheckBarTid = - 1;
                }
            });
        }
    }
}

