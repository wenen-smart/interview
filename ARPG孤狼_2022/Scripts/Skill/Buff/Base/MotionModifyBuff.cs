using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buff.Base
{
    /// <summary>
    /// 产生或影响运动的Buff 会涉及到属性修改继承与修改器buff
    /// </summary>
    public class MotionModifyBuff : ModifierBuff
    {
        /// <summary>
        /// 位移前的回调
        /// </summary>
        public Action updateBeforeMovement;
        /// <summary>
        /// 位移结束后的回调
        /// </summary>
        public Action updateAfterMovement;
        public Vector3 targetPos;
        protected override void Start()
        {
            base.Start();
            ApplyMotion(buffData.MID,0,true);//TODO
        }

        /// <summary>
        /// 申请运动效果 播放片段
        /// </summary>
        /// <param name="MID">位移数据配置项ID</param>
        /// <param name="priority">优先级</param>
        /// <param name="ignorePriority">无视优先级，直接打断</param>
        /// <returns></returns>
        public bool ApplyMotion(int MID, int priority, bool ignorePriority)
        {
            MoveComponent moveComponent=AffectedPerson.GetActorComponent<MoveComponent>();
            if (moveComponent==null)
            {
                return false;
            }
            updateBeforeMovement?.Invoke();
            var clip=MotionModifyManager.GetMotionClip(MID);
            clip._goalPos = targetPos;
            clip.finishAction += ()=> { updateAfterMovement?.Invoke();buffContainer.PushBuff(this); };
            clip.buff = this;
            clip.Init();
            moveComponent.UpdateMotionClip(clip);
            return true;
        }
    }
}