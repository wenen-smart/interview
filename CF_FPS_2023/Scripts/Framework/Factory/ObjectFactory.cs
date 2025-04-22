using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IObjectFactory<ObjectT> :IFactory where ObjectT : new()
{
    public  void PushItem(ObjectT objectEntity);
    public   ObjectT PopItem(string symbol);
}

