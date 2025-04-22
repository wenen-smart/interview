using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


   public static class BaseClasseExtral
    {
    public static bool  IsEqual(this string str,string target,bool ignoreUpper)
    {
        return str.ToLower().Equals(target.ToLower());
    }
}

