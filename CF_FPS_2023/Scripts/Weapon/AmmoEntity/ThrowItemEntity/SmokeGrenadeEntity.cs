using Assets.Resolution.Scripts.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SmokeGrenadeEntity : ThrowItemAmmoEntity
{
    protected override void Activate()
    {
        InstantiateEffect();
    }
}