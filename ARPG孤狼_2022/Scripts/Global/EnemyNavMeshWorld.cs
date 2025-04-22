using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavMeshWorld:MonoBehaviour
{
    public List<NavMeshPointGroup> navMeshPointGroups;
    public NavMeshPointGroup GetFineNavPath(NavMeshAgent agent)
    {
        Vector3 currentPos = agent.transform.position;
        NavMeshPointGroup navMeshPointGroup=null;
        float minDis = -1;
        foreach (var item in navMeshPointGroups)
        {
            if (item.enemy==null)
            {
                Vector3 pathOrigin = item.pointGroup[0].position;
                float dis = Vector3.Distance(currentPos, pathOrigin);
                NavMeshPath navMeshPath = new NavMeshPath();
                if (NavMesh.CalculatePath(currentPos, pathOrigin, NavMesh.AllAreas, navMeshPath)&&(minDis==-1||dis<minDis))
                {
                    minDis = dis;
                    navMeshPointGroup = item;
                }
            }
        }
        return navMeshPointGroup;
    }
    public void LeaveNavMeshPath(NavMeshAgent navMeshAgent,NavMeshPointGroup navMeshPointGroup)
    {
        if (navMeshPointGroup==null)
        {
            return;
        }
        if (navMeshAgent==navMeshPointGroup.enemy)
        {
            navMeshPointGroup.enemy = null;
        }
    }
}
[Serializable]
public class NavMeshPointGroup
{
    public List<Transform> pointGroup;
    [HideInInspector]
    public NavMeshAgent enemy;
}

