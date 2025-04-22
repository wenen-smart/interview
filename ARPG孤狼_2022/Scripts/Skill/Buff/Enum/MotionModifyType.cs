using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buff.Enum
{
    public enum MotionModifyType
    {
        MoveToPosition=1,
        LineTracking=1<<2,
        Tracking=1<<3,//轨道追踪
    }
}
