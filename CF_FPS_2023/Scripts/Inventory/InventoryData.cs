using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Resolution.Scripts.Inventory
{
    //仓库数据
    public class InventoryData:ScriptableObject
    {
        public List<Bag> bags;
        public int maxBagCount;
        public List<int> items;
        public bool IsInBag(int id)
        {
            return true;
        }
    }
}
