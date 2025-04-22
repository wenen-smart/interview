using Assets.Resolution.Scripts.Weapon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Resolution.Scripts.Inventory
{
    public class MyRuntimeInventory:ActorComponent
    {
        [SerializeField]private List<WeaponDataConfig> weaponDataConfigs;
        [HideInInspector]public List<BaseWeapon> weaponList;
        private int currentWeaponIndex=-1;
        //public BaseWeapon lastWeapon;
        public Coroutine exchangeWeaponCoroutine;
        public BaseWeapon currentWeapon { get { return currentWeaponIndex==-1?null:weaponList[currentWeaponIndex]; } }
        public Gun_Weapon currentGunWeapon = null;
        public bool isRecharging { get; protected set; }
        private int lastScroolDir = 0;
        private int targetIndex;
        private Queue<BaseWeapon> willRemovedWeapon=new Queue<BaseWeapon>();
        public Action OnSwitchWeapnAfterHandler;
        public Action OnSwitchWeapnBeforeHandler;
        public Action<BaseWeapon> WeaponLoadInInventoryHandler;
        public Action<BaseWeapon> ThrowWeaponHandler;
        public Action InventoryUpdatedHandler;
        public override void Init()
        {
            base.Init();
            InitializeRuntimeInventory();
        }
        public void InitializeRuntimeInventory()
        {
            //lastWeapon = null;
            currentWeaponIndex = -1;
            isRecharging = false;
            lastScroolDir = 0;
            targetIndex = 0;
            if (exchangeWeaponCoroutine != null)
            {
                StopCoroutine(exchangeWeaponCoroutine);
            }
            exchangeWeaponCoroutine = null;
            currentGunWeapon = null;
            willRemovedWeapon.Clear();
            ReloadInventoryByConfig();
            foreach (var item in weaponList)
            {
                item.gameObject.SetActive(false);
                item.RenewWeapon();
                WeaponLoadInInventoryHandler?.Invoke(item);
                item.FillAllAmmo();
            }
            SortWeapon();
            InventoryUpdatedHandler?.Invoke();
        }
        public void SwitchBag(int index=0)
        {
            //TODO
            //set weapons
        }
        public override void ActorComponentAwake()
        {
            base.ActorComponentAwake();
        }
        public int GetWeaponIndex(int _index)
        {
            if (_index <= -1)
            {
                _index = weaponList.Count - 1;
            }
            int weaponIndex = _index % weaponList.Count;
            return weaponIndex;
        }
        public void ExchangeWeaponByScroll(int up)
        {
            if (up == 0)
            {
                return;
            }
            up = ((up > 0) ? 1 : -1);
            ExchangeWeapon(targetIndex + up);
            lastScroolDir = up;
        }
        public void ExchangeWeapon(int index)
        {
            if (weaponList.Count == 0)
            {
                DebugTool.DebugWarning($"没有武器列表");
                return;
            }
            int weaponIndex = GetWeaponIndex(index);
            targetIndex = weaponIndex;
            if (weaponList != null && weaponList.Count > 0)
            {
                if (TryExchangeWeapon(weaponIndex))
                {
                    
                }
            }
            else
            {
                Debug.Log("Current weaponList no weapon");
            }
        }
        protected bool TryExchangeWeapon(int weaponIndex)
        {
            if (isRecharging)
            {
                //weapons[GetWeaponIndex(currentWeaponIndex-lastScroolDir)]?.UnEqiped();
                weaponList[GetWeaponIndex(currentWeaponIndex)]?.UnEqiped();
                weaponList[GetWeaponIndex(currentWeaponIndex)]?.gameObject?.SetActive(false);
                //RemoveWeaponByWaitRemoveQueue();
            }
            if (exchangeWeaponCoroutine!=null)
            {
                StopCoroutine(exchangeWeaponCoroutine);
                exchangeWeaponCoroutine = null;
            }
            OnSwitchWeapnBeforeHandler?.Invoke();
            exchangeWeaponCoroutine = StartCoroutine(ExchangeWeaponCoroutine(currentWeapon,weaponIndex));
            Debug.Log("ExchangeWeapon");
            return true;
        }

        public IEnumerator  ExchangeWeaponCoroutine(BaseWeapon _lastWeapon,int weaponIndex)
        {
            if (_lastWeapon)
            {
                isRecharging = true;
                float takeOutSeconds = _lastWeapon.takeOutSeconds;
                _lastWeapon.userAnimator?.CrossFade(WeaponActionType.Hide.ToString(), 0);
                Debug.Log("lastWeapon:"+_lastWeapon);
                Debug.Log("userAnimator:"+_lastWeapon.userAnimator);
                MyTimer timer = TimeSystem.Instance.CreateTimer();
                timer.Go(takeOutSeconds);
                while (timer.timerState == MyTimer.TimerState.Run)
                {
                    yield return 0;
                }
                _lastWeapon.UnEqiped();
            }
            //if (RemoveWeaponByWaitRemoveQueue()==0)
            //{
            //    //没有移除的武器 在上一个武器切换到下一个武器后 记录lastweapon
            //    lastWeapon = _lastWeapon;
            //}
            currentWeaponIndex = weaponIndex;
            if (!(currentWeapon is Gun_Weapon))
            {
                currentGunWeapon = null;
            }
            else
            {
                currentGunWeapon = currentWeapon as Gun_Weapon;
            }
            currentWeapon.Equiped(actorSystem.GetActorComponent<RoleController>());
            OnSwitchWeapnAfterHandler?.Invoke();
            isRecharging = false;
            yield return 0;
        }
        public void RemoveWeaponRuntimeInventory(BaseWeapon weapon)
        {
            weapon.gameObject.SetActive(false);
            willRemovedWeapon.Enqueue(weapon);
            RemoveWeaponByWaitRemoveQueue();
            ExchangeWeapon(currentWeaponIndex);
            InventoryUpdatedHandler?.Invoke();
        }
        private int RemoveWeaponByWaitRemoveQueue()
        {
            int removedWeaponCount = willRemovedWeapon.Count;
            if (removedWeaponCount==0)
            {
                return 0;
            }
            while (willRemovedWeapon.Count>0)
            {
                BaseWeapon weapon = willRemovedWeapon.Dequeue();
                int weaponIndex = weaponList.FindIndex(0, (w) => w == weapon);
                if (weaponIndex != -1)
                {
                    weaponList.RemoveAt(weaponIndex);
                    if (weaponIndex < currentWeaponIndex)
                    {
                        currentWeaponIndex = GetWeaponIndex(currentWeaponIndex-1);
                    }
                    //移除列表中最后一个元素， currentIndex值一定要更新。
                    if (currentWeaponIndex >= weaponList.Count)
                    {
                        currentWeaponIndex = 0;
                    }
                }
            }
            //lastWeapon = null;
            
            return removedWeaponCount;
        }

        public void ReloadInventoryByConfig()
        {
            weaponList.Clear();
            //是玩家还是Robot
            RoleController role = GetActorComponent<RoleController>();
			for (int i = 0; i < weaponDataConfigs.Count; i++)
			{
                weaponList.Add(weaponDataConfigs[i].InstantiateWeaponAndTryAttach(role));
			}
		}
        
        public BaseWeapon AddWeapon(WeaponDataConfig configs)
        {
            BaseWeapon newWeapon = null;
            RoleController role = GetActorComponent<RoleController>();
            //
            newWeapon = configs.InstantiateWeaponAndTryAttach(role);
            //
            int weaponIndex = -1;
            if (configs.weaponType==WeaponType.Thrown)
            {
               var throwItemDataConfig = (ThrowItemDataConfig)configs;
               weaponIndex = weaponList.FindIndex((w) => {
                   if (w.weaponType == configs.weaponType)
                   {
                       ThrowItemDataConfig w_throwItemData = (ThrowItemDataConfig)w.GetItemDataConfig();
                       if (w_throwItemData.throwItemType == throwItemDataConfig.throwItemType)
                       {
                           return true;
                       }
                   }
                   return false;
               });
            }
            else 
            {
                weaponIndex = weaponList.FindIndex((w) => { return w.weaponType == configs.weaponType; });
            }
            WeaponLoadInInventoryHandler?.Invoke(newWeapon);
            weaponList.Add(newWeapon);
            if (weaponIndex != -1)
            {
                ReplaceWeapon(weaponIndex, weaponList.Count-1);
            }
            else
            {
                //背包里不存在该武器，那么应该直接切换到新增的武器
                //先排序再切。切换使用是协程在不加等待队列无法先切再排序，记录会错乱。
                SortWeapon();
                //排序后再去查找对应的索引
                int newindex = weaponList.FindIndex((w) => { return w == newWeapon; });
                ExchangeWeapon(newindex);
            }
            InventoryUpdatedHandler?.Invoke();
            return newWeapon;
        }
        private void SortWeapon()
        {
            BaseWeapon _currentWeapon = currentWeapon;
            weaponList = weaponList.OrderBy(
                (w) => {
                    int weight = (int)w.weaponType;
                    if (w.weaponType == WeaponType.Thrown)
                    {
                        ThrowItemDataConfig w_throwItemData = (ThrowItemDataConfig)w.GetItemDataConfig();
                        weight += (int)w_throwItemData.throwItemType;
                    }
                    return weight;
                    }
                ).ToList<BaseWeapon>();
            if (_currentWeapon!=null)
            {
                int weaponIndex = weaponList.FindIndex((w) => { return w == _currentWeapon; });
                if (weaponIndex == -1)
                {
                    DebugTool.DebugError("排序后找不到武器");
                    return;
                }
                currentWeaponIndex = weaponIndex;
            }
        }
        private void ReplaceWeapon(int current, int newWeaponIndex)
        {
            BaseWeapon currentWeapon = weaponList[current];
            BaseWeapon newWeapon = weaponList[newWeaponIndex];
            if (currentWeapon.weaponType == WeaponType.Primarily || newWeapon.weaponType == WeaponType.Pistol)
            {
                //枪械抛出再替换
                //再切换枪械 -> 已由角色发起
                ThrowWeaponHandler?.Invoke(currentWeapon);
                //TODO:有问题  过渡问题。
            }
            else if (newWeapon.weaponType == WeaponType.Knife || newWeapon.weaponType == WeaponType.Thrown)
            {
                //直接替换 不抛出
                ExchangeWeapon(newWeaponIndex);
            }
        }

        public bool IsExistThisWeaponOfType(WeaponType weaponType)
        {
            return weaponList.Any((w)=>w.weaponType==weaponType);
        }
        public bool IsExistThisThrowItemOfType(ThrowItemType throwItemType)
        {
            return weaponList.Any((w)=> {
                ThrowItemDataConfig w_throwItemData = (ThrowItemDataConfig)w.GetItemDataConfig();
                return w_throwItemData.throwItemType == throwItemType;
            });
        }
        

    }
}


