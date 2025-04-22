using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resolution.Scripts.Inventory
{
    //仓库管理器
    public class InventoryManager : IManager
    {
        public InventoryData inventoryData;
        public void Init()
        {
            
        }

        public void LoadScript(object args)
        {
            
        }
        public void Add()
        {

        }
        public void Remove()
        {

        }
        public void Equip(int groupID,int id)
        {
            if (inventoryData.maxBagCount<=groupID)
            {
                return;
            }
            inventoryData.bags[groupID].Add(id);
        }
        public void UnEquip(int groupID,int id)
        {
            if (inventoryData.maxBagCount <= groupID)
            {
                return;
            }
            inventoryData.bags[groupID].Remove(id);
        }
    }
}
