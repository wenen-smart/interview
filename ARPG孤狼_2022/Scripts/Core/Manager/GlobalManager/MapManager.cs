using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : InstanceMono<MapManager>
{

    public Dictionary<string, EnemyController> enemyDics;
    public EnemyNavMeshWorld enemyNavMeshWorld;
    public Dictionary<string, List<RoleController>> campDics = new Dictionary<string, List<RoleController>>();

    public void FindClosetEnemy()
    {

    }

    public void RegisterCamp(string unitCamp,RoleController roleController)
    {
        List<RoleController> list=null;
        if (campDics.ContainsKey(unitCamp))
        {
            campDics.TryGetValue(unitCamp,out list);
        }
        if (list!=null)
        {
            if (list.Contains(roleController)==false)
            {
                list.Add(roleController);
            }
        }
        else
        {
            campDics.Add(unitCamp,new List<RoleController>() {roleController});
        }
    }
    public List<RoleController> FindActors(string camp)
    {
        if (campDics==null||campDics.Count==0)
        {
            return null;
        }
        List<RoleController> list;
        campDics.TryGetValue(camp,out list);
        return list;
    }
}
