using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class IWeaponConsumable:ItemObject
{
    public RoleController caster;//施加者
    public virtual void Renew()
    {

    }
    public abstract void Used();
    public abstract void AffectLose();
}

