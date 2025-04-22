using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponManagerConfig", menuName = "CreateConfig/CreateWeaponManagerConfig", order = 1)]
public class RoleWeaponManagerConfig:ScriptableConfig
{
    public string masterName;
    public WeaponConfig_Manager[] WeaponConfigList;
}
[Serializable]
public class WeaponConfig_Manager
{
    public string weaponName;
    public RuntimeAnimatorController overrideController;
}

