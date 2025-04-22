using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buff.Enum
{
    public enum BuffActionEffect
    {
        Stun = 1 << 1,//目标不响应任何操控
        Root = 1 << 2,//不响应移动请求，但可以执行操作 如释放某些技能 解控 加血 驱散buff
        Picked =1 << 3,//被挑起
        Heaven = 1<<4,//浮空
        Force = 1<<5,//施加力
    }
}