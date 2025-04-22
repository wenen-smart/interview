using Assets.Resolution.Scripts.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resolution.Scripts.Inventory
{
    public class Bag
    {
        public List<int> bagItems;
        public bool isLock = true;
        public bool IsHave(int id)
        {
            return true;
        }
        public int IsHave(WeaponType weaponType)
        {
            return -1;
        }
        public void Add(int id)
        {

        }
        public void ExChange(int id)
        {
            //for find sametype and remove

            Add(id);
        }
        public void Remove(int id)
        {

        }
    }
}
