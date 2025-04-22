using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buff.Enum
{
    public enum BuffOverrideType
    {
        NonOverride = 0,//不可叠加
        Override = 1,//可叠加
        SameCasterOverride = 1 << 1 | 1,//相同施加者可叠加
    }
}