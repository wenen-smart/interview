using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buff.Enum
{
    /// <summary>
    /// 暂时无用
    /// </summary>
    public enum BuffIntervalThinkMode
    {
        OnUpdate=1,
        Start=1<<1,
        FinishAfter=1<<2,
    }
}
