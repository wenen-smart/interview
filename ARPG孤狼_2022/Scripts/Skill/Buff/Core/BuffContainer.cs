using Buff.Base;
using Buff.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Buff.Core
{
    /// <summary>
    /// buff管理 每个影响对象都应该挂载
    /// </summary>
    public class BuffContainer : ActorComponent
    {
        public LinkedList<BuffBase> linkedList = new LinkedList<BuffBase>();
        private Queue<BuffBase> startQueue = new Queue<BuffBase>();
        private Queue<BuffBase> destoryQueue = new Queue<BuffBase>(); 
        public BuffBase DOBuff(int BID,ActorSystem caster)
        {
            //BuffSystem.Pop
            BuffData data= BuffManager.Instance.GetBuffData(BID);
            BuffBase buff = BuffPoolSystem.PopupBuff(data.buffClassTypeName);//此处可能需要优化
            buff.buffContainer = this;
            buff.caster = caster;
            buff.AffectedPerson = this.actorSystem;
            buff.buffData = data;
            LinkedListNode<BuffBase> reference = linkedList.First;
            //找到一个比buff还小的，插入到它的前面
            while (reference != null)
            {
                if (reference.Value.buffData.priority <= buff.buffData.priority)
                {
                    break;
                }
                else
                {
                    reference = reference.Next;
                }
            }
            if (reference != null)
            {
                var pre = reference.Previous;
                LinkedListNode<BuffBase> linkedListNode = linkedList.AddBefore(reference, buff);
            }
            else
            {
                linkedList.AddLast(buff);
            }
            bool vaild = InitBuff(buff);
            if (vaild==false)
            {
                //不走生命周期 直接移除掉
                linkedList.Remove(buff);
                return null;
            }
            startQueue.Enqueue(buff);
            return buff;
        }
        public void PushBuff(BuffBase buff)
        {
            if (buff==null)
            {
                return;
            }
            buff.OnRemove();
            destoryQueue.Enqueue(buff);
        }
        public void PushBuff(int BID)
        {
            PushBuff(TryGetBuff(BID));
        }
        private void Update()
        {
            while (startQueue.Count > 0)
            {
                BuffBase buff = startQueue.Dequeue();
                BuffStart(buff);
            }
            while (destoryQueue.Count > 0)
            {
                BuffBase buff = destoryQueue.Dequeue();
                BuffDead(buff);
            }
            OnTickAllBuff();
        }
        private bool InitBuff(BuffBase buff)
        {
            bool buffVaild = true;
            buffVaild = BuffOverrideLogic(buff);
            if (buffVaild)
            {
                buff.OnAwake();
                SetBuffCallback(buff);
            }
            return buffVaild;
        }
        private void SetBuffCallback(BuffBase buff)
        {
            int actionID = buff.buffData.actionID;
            buff.OnBuffAwake = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnBuffAwake));
                 buff.OnBuffStart = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnBuffStart));
            buff.OnBuffRefresh = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnBuffRefresh));
            buff.OnBuffRemove = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnBuffRemove));
            buff.OnBuffDestory = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnBuffDestory));
            buff.OnSkillExecuted = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnSkillExecuted));
            buff.OnBeforeGiveDamage = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnBeforeGiveDamage));
            buff.OnAfterGiveDamage = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnAfterGiveDamage));
            buff.OnBeforeTakeDamage = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnBeforeTakeDamage));
            buff.OnAfterTakeDamage = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnAfterTakeDamage));
            buff.OnBeforeDead = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnBeforeDead));
            buff.OnAfterDead = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnAfterDead));
            buff.OnBeforeKill = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnBeforeKill));
             buff.OnAfterKill = CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnAfterKill));
                buff.OnIntervalThink=CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnIntervalThink));
            buff.OnIntervalThinkLifeFinish=CoreEventSystem.Instance.GetEventAction(BuffManager.Instance.GetEventCode(actionID,_Data_BuffEvent.OnIntervalThinkLifeFinish));
        }
        private bool BuffOverrideLogic(BuffBase buff)
        {
            List<BuffBase> filterBuffs = FilterBuff(buff.buffData.buffActionEffect);
            MyDebug.DebugPrint("同效果buff"+filterBuffs.Count);
            bool buffVaild = true;
            if (filterBuffs != null && filterBuffs.Count > 0)
            {
                int sameBuffCount = filterBuffs.Count;
                if (buff.IsOverride() == false)
                {
                    //logic-1
                    foreach (var filterBuff in filterBuffs)
                    {
                        if (filterBuff!=buff)
                        {
                            //遍历中过滤掉当前buff
                            PushBuff(filterBuff);
                        }
                    }

                                  //logic-2
                                  //logic-2

                    //还没涉及这部分 框架封装的不必太超前，首先需要把游戏玩法搞出来 根据技能需求来决定是否要这些覆盖逻辑
                }
                else
                {
                    bool sameCastOverride = buff.IsSameCasterOverride();
                    if (filterBuffs[0].caster == buff.caster)
                    {
                        //相同可叠加
                        //叠加逻辑
                        if (sameCastOverride)
                        {
                            if (buff.buffData.maxOverrideLayer > sameBuffCount)
                            {
                                //还可以叠加
                            }
                            else
                            {
                                //不可叠加 -- 替换掉一个旧Buff
                                PushBuff(filterBuffs[0]);
                            }
                        }
                    }
                    else
                    {
                        if (sameCastOverride == false)
                        {
                            //不同施加者- 不可叠加
                            //当前buff不生效  还是覆盖
                            //如果覆盖又走上面不叠加逻辑
                            foreach (var filterBuff in filterBuffs)
                            {
                                if (filterBuff != buff)
                                {
                                    //遍历中过滤掉当前buff
                                    PushBuff(filterBuff);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return buffVaild;
        }

        private void BuffStart(BuffBase buff)
        {
            buff.OnStart();
            buff.StartInterval();
            ImmunityBuff(buff.buffData.buffImmunityTag);
            GameRoot.Instance.AddTimeTask(buff.buffData.intervalExecute, () => { }, PETime.PETimeUnit.Seconds, buff.buffData.executeCount);
            if (buff.buffData.duration > 0)
            {
                GameRoot.Instance.AddTimeTask(buff.buffData.duration, () => { PushBuff(buff); }, PETime.PETimeUnit.Seconds, buff.buffData.executeCount);//计时回收
            }
            else if (buff.buffData.duration == 0)
            {
                PushBuff(buff);//持续时间等于0立刻回收
            }
        }

        private void BuffDead(BuffBase buff)
        {
            buff.OnDestory();
            BuffPoolSystem.PushBuff(buff);
            linkedList.Remove(buff);
        }

        private void OnTickAllBuff()
        {
            LinkedListNode<BuffBase> pointer = linkedList.First;
            while (pointer != null)
            {
                pointer.Value.OnUpdate();
                pointer = pointer.Next;
            }
        }
        /// <summary>
        /// 处理免疫列表中的buff
        /// </summary>
        private void ImmunityBuff(BuffTag immunityTag)
        {
            LinkedListNode<BuffBase> pointer = linkedList.First;
            List<LinkedListNode<BuffBase>> matchBuffs = new List<LinkedListNode<BuffBase>>();
            while (pointer != null)
            {
               bool isMatch=pointer.Value.buffData.buffTag.IsSelectThisEnumInMult(immunityTag);
                //免疫全部  TODO未来可能得配置可免疫个数
                if (isMatch)
                {
                    matchBuffs.Add(pointer);
                }
                pointer = pointer.Next;
            }
            matchBuffs.ForEach((rmvBuff)=> {linkedList.Remove(rmvBuff); });
        }

        public bool IsContainBuff(Buff.Enum.BuffActionEffect buffActionEffect)
        {
            return TryGetBuff(buffActionEffect) != null;
        }
        public bool IsContainBuff(int bid)
        {
            return TryGetBuff(bid) != null;
        }
        public BuffBase TryGetBuff(Buff.Enum.BuffActionEffect buffActionEffect)
        {
            LinkedListNode<BuffBase> pointer = linkedList.First;
            while (pointer != null)
            {
                if ((pointer.Value.buffData.buffActionEffect & buffActionEffect) != 0)
                {
                    break;
                }
                pointer = pointer.Next;
            }
            return pointer?.Value;
        }
        public BuffBase TryGetBuff(int BID)
        {
            LinkedListNode<BuffBase> pointer = linkedList.First;
            while (pointer != null)
            {
                if ((pointer.Value.buffData.BID == BID))
                {
                    return pointer.Value;
                }
                pointer = pointer.Next;
            }

            MyDebug.DebugWarning("当前队列中没有对应的buff");
        
            return null;
        }
        public List<BuffBase> FilterBuff(Buff.Enum.BuffActionEffect buffActionEffect)
        {
            LinkedListNode<BuffBase> pointer = linkedList.First;
            List<BuffBase> tp_buffs = new List<BuffBase>();
            while (pointer != null)
            {
                if ((pointer.Value.buffData.buffActionEffect & buffActionEffect) != 0)
                {
                    tp_buffs.Add(pointer.Value);
                }
                pointer = pointer.Next;
            }
            return tp_buffs;
        }
    }
}