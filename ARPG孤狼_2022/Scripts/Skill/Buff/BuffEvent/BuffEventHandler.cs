using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Buff.Event
{
    
    //public delegate void BuffEvent(BuffEventArgs args);
    //public delegate void BuffEvent<T>(T arg1);
    //public delegate void BuffEvent<T, T1>(T arg1, T1 arg2);
    //public delegate void BuffEvent<T, T1, T2>(T arg1, T1 arg2, T2 arg3);
    public class BuffEventHandler:EventHandler<BuffEventHandler, BuffEventArgs>
    {
        public BuffEventHandler(AEvent<BuffEventArgs> buffEvent):base(buffEvent)
        {
            
        }
    }
    public class BuffEventArgs:EventArgs
    {
        
    }
}