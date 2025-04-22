using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CoreEventSystem : IEventSystem<CoreEventSystem, EventCode, CoreEventHandler, EventArgs>
{
    public override void ClearAllEvent()
    {
        throw new NotImplementedException();
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }

    public override void RemoveEvent(EventCode eventCode)
    {
        throw new NotImplementedException();
    }

    public override void RemoveEvent(CoreEventHandler eventAction)
    {
        throw new NotImplementedException();
    }
}

