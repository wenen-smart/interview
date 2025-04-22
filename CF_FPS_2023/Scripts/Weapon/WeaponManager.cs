using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Resolution.Scripts.Weapon
{
    public class WeaponManager:MonoBehaviour
    {
        public List<BaseWeapon> weapons;
        public int currentWeaponIndex;

        public bool ExchangeWeapon(BaseWeapon weapon)
        {
            return true;
        }
        public void ExchangeWeaponByScroll(int up)
        {
            if (up == 0)
            {
                return;
            }
            up = ((up > 0) ? 1 : -1);
            ExchangeWeapon(currentWeaponIndex + up);
        }
        public void ExchangeWeapon(int index)
        {
            if (weapons.Count == 0)
            {
                DebugTool.DebugWarning($"没有武器列表");
                return;
            }
            int weaponIndex = index % weapons.Count;
            if (weapons != null && weapons.Count > 0)
            {
                if (ExchangeWeapon(weapons[weaponIndex]))
                {
                    currentWeaponIndex = weaponIndex;
                }
            }
            else
            {
                Debug.Log("Current weaponList no weapon");
            }
        }
    }
}
