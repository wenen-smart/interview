using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public class EnemyFacade:CharacterFacade
    {
    public override HpBarEntity ApplyHpBarEntity()
    {
        return BattleManager.Instance.ApplyHpBarEntity(roleController.damageable);
    }
}

