using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CircleAttackSelector : IAttackSelector
{
    public override IDamageable[] SelectTarget(SkillEntity skillEntity, Transform skillTransform, float angle, float distance)
    {
        var skillData = skillEntity.skillData;
        List<RoleController> tempList = new List<RoleController>();
        for (int i = 0; i < skillEntity.attackerTags.Length; i++)
        {
            //var list = GameObject.FindGameObjectsWithTag(skillEntity.attackerTags[i]);
            var list = MapManager.Instance.FindActors(skillEntity.attackerTags[i]);
            if (list != null && list.Count > 0)
            {
                tempList.AddRange(list);
            }
        }
        List<IDamageable> targetList = ArrayHelper.ToComponents<RoleController,IDamageable>(tempList);
        //
        if (targetList == null)
        {

            return null;
        }
        var enemys = targetList.FindAll((t) =>
        {
            float dis = Vector3.Distance(skillTransform.position/*skillEntity.owner.position*/, t.transform.position);


            return dis < distance && t.GetComponent<IDamageable>().IsDie == false;
        });
        //
        if (enemys == null || enemys.Count <= 0)
        {

            return null;
        }
        for (int i = 0; i < enemys.Count - 1; i++)
        {
            for (int j = 0; j < enemys.Count - i - 1; j++)
            {
                if (Vector3.Distance(enemys[j].transform.position, skillTransform.position) > Vector3.Distance(enemys[j + 1].transform.position, skillTransform.position))
                {
                    var temp = enemys[j];
                    enemys[j] = enemys[j + 1];
                    enemys[j + 1] = temp;
                }
            }
        }
        List<IDamageable> filterEnemy = new List<IDamageable>();
        foreach (var damageable in enemys)
        {
            bool isCanDamaged = false;
            isCanDamaged = (skillEntity.DamagedTimeIsOver(damageable));
            if (isCanDamaged)
            {
                if (skillEntity.IsCanAttackGroup() == false)
                {
                    if (skillEntity.CurrentIsHaveDamageAnyObject())
                    {
                        continue;
                    }
                }
                filterEnemy.Add(damageable);
            }
        }
        IDamageable[] result = null;
        if (filterEnemy != null&&filterEnemy.Count>0)
        {
            switch (skillData.attackType)
            {
                case SkillAttackType.Single:
                    result = new IDamageable[] { filterEnemy[0] };
                    break;
                case SkillAttackType.Group:
                    result = filterEnemy.ToArray();
                    break;
                default:
                    break;
            }
        }
        
        
        return result;
    }
    
}
