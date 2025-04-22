using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


public class AssemblyTool
{
    public static Type GetCorrentType(string className)
    {
        Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblys)
        {
            Type[] types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.Name == className)
                {
                    return type;
                }
            }
        }
        return null;

    }
}

