using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "New_HitterEffectSettingSO", menuName = "HitterEffect/CreateSettingSO")]
public class HittableEffectSettingSO:ScriptableObject
{
    public List<HESettingAssociateWeapon> settings;

    public bool GetHittableEffectSetting(int id,out HittableEffectSetting setting)
    {
        setting = null;
        var aw =  settings.FirstOrDefault((s) =>
        {
            if (s.IsHaveTheWeaponInBind(id))
            {
                return true;
            }
            return false;
        });
        if (aw!=null)
        {
            setting = aw.EffectSetting;
            return true;
        }
        return false;
    }
}
[Serializable]
public class HESettingAssociateWeapon
{
    public string Description;
    public List<WeaponDataConfig> bindWeapons;
    public HittableEffectSetting EffectSetting;
    public bool IsHaveTheWeaponInBind(int id)
    {
        return bindWeapons.Any((wc) => wc.id == id);
    }
}
