
using Assets.Scripts.Buff.Event;
using Buff.Core;
using Buff.Enum;
using PETime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buff.Base
{
    /// <summary>
    /// Buff基本类
    /// </summary>
    public class BuffBase
    {
        public int BID { get { return buffData.BID; } }
        public BuffData buffData;
        public ActorSystem AffectedPerson;
        public ActorSystem caster;//施加者
        public BuffContainer buffContainer;
        public object skill;//技能引用
        public int executedCount = 0;
        public CoreEventHandler OnBuffAwake;//buff对象一经创建就立即调用
        public CoreEventHandler OnBuffStart;//Buff生命周期开始
        public CoreEventHandler OnBuffRefresh;//Buff刷新 如叠加buff时候更新buff数据
        public CoreEventHandler OnBuffRemove;//Buff销毁前调用（还在Buff容器）
        public CoreEventHandler OnBuffDestory;//Buff销毁后调用（不在Buff容器）

        #region 定时相关
        public int TID = -1;
        public CoreEventHandler OnIntervalThink;//定时回调
        public CoreEventHandler OnIntervalThinkLifeFinish;
        /// <summary>
        /// 设置定时器
        /// </summary>
        /// <param name="intervalExecuteAction"></param>
        public void StartInterval()
        {
            if (buffData.executeType==BuffExecuteType.Once)
            {
                return;
            }
            if (buffData.executeType==BuffExecuteType.Loop)
            {
                AddIntervalTask(-1);
                return;
            }
            AddIntervalTask(buffData.executeCount,()=> { PushSelf(); });
        }
        private void AddIntervalTask(int loopCount,Action taskFinishExtralAction=null)
        {
            if (OnIntervalThink == null || OnIntervalThink.HaveListener == false)
            {
                return;
            }
            RemoveInterval();
            TID=GameRoot.Instance.AddTimeTask(buffData.intervalExecute, ()=> { OnIntervalThink?.Invoke(PackEventArgs()); this.executedCount++; },PETimeUnit.Seconds,loopCount,()=> { OnIntervalThinkLifeFinish?.Invoke(PackEventArgs());taskFinishExtralAction?.Invoke(); });
        }
        /// <summary>
        /// 移除定时器
        /// </summary>
        public void RemoveInterval()
        {
            if (TID==-1)
            {
                return;
            }
            GameRoot.Instance.peTimer.DeleteTimeTask(TID);
        }
        #endregion
        #region 其他事件  先整理重要事件 关于是否要遵循单一职责原则 看后期是否方便扩展
        public CoreEventHandler OnSkillExecuted;//当技能执行成功执行 
        public CoreEventHandler OnBeforeGiveDamage;//监听对方受到伤害前
        public CoreEventHandler OnAfterGiveDamage;//监听对方收到伤害后
        public CoreEventHandler OnBeforeTakeDamage;//监听自身受到伤害前
        public CoreEventHandler OnAfterTakeDamage;//监听自身受到伤害后
        public CoreEventHandler OnBeforeDead;//监听自身死亡前
        public CoreEventHandler OnAfterDead;//监听自身死亡后
        public CoreEventHandler OnBeforeKill;//监听击杀目标前
        public CoreEventHandler OnAfterKill;//监听击杀目标后
        #endregion

        public void OnInit() { Init(); }
        public void OnAwake() { Awake(); }
        public void OnStart() { Start(); }
        public void OnUpdate() { Update(); }
        public void OnRefresh() { Refresh(); }
        public void OnRemove() { Remove();RemoveInterval(); }
        public void OnDestory() { Destory(); }


        protected virtual void Init() { }
        protected virtual void Awake() { OnBuffAwake?.Invoke(PackEventArgs()); }
        protected virtual void Start()
        {
            OnBuffStart?.Invoke(PackEventArgs());
            if (buffData.executeType==BuffExecuteType.Once)
            {
                PushSelf();
            }
        }
        protected virtual void Update() { }
        protected virtual void Refresh() { OnBuffRefresh?.Invoke(PackEventArgs()); }
        protected virtual void Remove() { OnBuffRemove?.Invoke(PackEventArgs()); }
        protected virtual void Destory() { OnBuffDestory?.Invoke(PackEventArgs()); }

        public virtual EventArgs PackEventArgs()
        {
            return new EventArgs() { sender=caster,value=new object[] {this} };//暂时
        }
        public bool IsOverride()
        {
            return (buffData.overrideType & BuffOverrideType.Override) != 0;
        }
        public bool IsSameCasterOverride()
        {
            return buffData.overrideType == BuffOverrideType.SameCasterOverride;
        }
        public void Restore(bool fp_ClearDirtyData = true)
        {

        }
        protected void PushSelf()
        { 
            buffContainer.PushBuff(this);
        }
        public Type ownerType;

    }
}