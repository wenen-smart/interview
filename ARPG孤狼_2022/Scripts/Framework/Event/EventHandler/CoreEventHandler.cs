using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CoreEventHandler : EventHandler<CoreEventHandler, EventArgs>
{
    public CoreEventHandler(AEvent<EventArgs> _event) : base(_event)
    {
    }
}

