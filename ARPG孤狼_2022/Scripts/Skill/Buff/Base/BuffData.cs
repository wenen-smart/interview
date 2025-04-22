using Buff.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buff.Base
{
    public class BuffData
    {
        public int BID;//Buff标识
        public int actionID;
        public int MID;
        public int priority = 0;//优先级 默认为0
        public string buffClassTypeName;
        public BuffActionEffect buffActionEffect;//作用效果\状态
        public BuffBenefitType benefitType;//效益类型
        public BuffTag buffTag;//buff种类
        public BuffTag buffImmunityTag;//当前buff免疫的种类
        public float duration = 0;//持续时间 -1为永久不自动销毁
        public float intervalExecute = 0;//间隔执行
        public BuffExecuteType executeType;//执行类型
        public int executeCount = 0;
        public float currency;//基础值
        //public BuffContext context;//上下文
        public BuffOverrideType overrideType;//叠加类型
        public int maxOverrideLayer = 0;//叠加的最大层数
    }
}
