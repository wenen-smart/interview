using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerDamageable:IDamageable
    {


    private PlayerStateManager stateManager;


    public override void Start()
    {
        base.Start();
        stateManager = PlayerFacade.Instance.playerStateManager as PlayerStateManager;
      
    }
    public void Update()
    {
        isDenfense = stateManager.isDenfense;
    }
    public override void Hit(Transform attacker,int damage, Vector3 attackDir,Vector3 attackPoint, int hitIntAction = 1, int defenseAction = 0)
    {
        base.Hit(attacker,damage, attackDir,attackPoint, hitIntAction, defenseAction);

    }

    /// <summary>
    /// 返回值：减免的伤害的百分比；0-1 1为免伤。
    /// </summary>
    /// <returns></returns>
    public float DefenseDamagePercent()
    {
        return 1;
    }
}

