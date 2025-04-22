using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ClonelyEnemyAnim:EnemyAnim
    {

    public ClonelyEnemyFacade enemyFacade;
    
    public void Awake()
    {
        enemyFacade = GetComponentInParent<ClonelyEnemyFacade>();

    }
    public override void SwitchEnableAttack(int booleanInt)
    {
        base.SwitchEnableAttack(booleanInt);
        bool enableAttack = booleanInt == 1 ? true : false;
        Debug.Log(enableAttack);
        if (enableAttack)
        {
            enemyFacade.weaponManager.currentWeapon.EnableAttack();
            DeploySkill();
        }
        else
        {
            enemyFacade.weaponManager.currentWeapon.DisableAttack();
        }
    }
}

