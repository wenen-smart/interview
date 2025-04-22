using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamCommunication
{
    public List<RoleController> members = new List<RoleController>();
    public UnitCamp unit;
    public List<RoleController> waiter = new List<RoleController>();
    public int waitEnemyHomeWaiterCount =>waiter.Count;
    public void AddTeam(RoleController bot)
    {
        members.Add(bot);
    }

    public void FindTarget(RoleController sender,Vector3 targetPoint)
    {
        for (int i = 0; i < members.Count; i++)
        {
            var member = members[i];
            if (sender==member)
            {
                continue;
            }
            member.FindTarget(targetPoint);
        }
    }

    public void WaiterEnemyHome(RoleController role)
    {
        if (!waiter.Contains(role))
        {
            waiter.Add(role);
        }
    }
    public void LeaveWaitEnemyHome(RoleController role)
    {
        if (waiter.Contains(role))
        {
            waiter.Remove(role);
        }
    }
}
