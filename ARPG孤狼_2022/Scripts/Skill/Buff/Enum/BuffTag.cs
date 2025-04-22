using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buff.Enum
{
    public enum BuffTag
    {
        None=0,
        Common = 1<<0,
        Fire = 1 << 1,//火系
        Water = 1 << 2,//水系
        Wood = 1 << 3//木系
    }
}