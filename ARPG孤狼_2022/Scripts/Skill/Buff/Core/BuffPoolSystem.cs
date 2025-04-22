using Buff.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// buff对象池
/// </summary>
namespace Buff.Core
{
    public class BuffPoolSystem
    {
        public static Dictionary<Type, Queue<BuffBase>> buffPools=new Dictionary<Type, Queue<BuffBase>>();
        public static BuffBase PopupBuff(Type type)
        {
             Queue<BuffBase> buffBases=null;
            if (buffPools.TryGetValue(type,out buffBases))
            {
                if (buffBases.Count>0)
                {
                    return buffBases.Dequeue();
                }
            }
            BuffBase buffBase=Activator.CreateInstance(type) as BuffBase;
            buffBase.ownerType = type;
            return buffBase;
        }
        public static BuffBase PopupBuff(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }
            Type type = Type.GetType(typeName);
            return PopupBuff(type);
        }

        public static void PushBuff(BuffBase buff)
        {
            Type type = buff.ownerType;
            PushBuff(type, buff);
        }
        public static void PushBuff<T>(T buff) where T:BuffBase
        {
            Type type = typeof(T);
            PushBuff(type, buff);
        }
        public static void PushBuff(Type type, BuffBase buff)
        {
            if (type == null)
            {
                Debug.LogError("BuffBaseID:" + buff.BID + "无法获取到Type类型");
                return; 
            }
            Queue<BuffBase> buffBases = null;
            if (!buffPools.TryGetValue(type, out buffBases))
            {
                //
            }
            if (buffBases == null)
            {
                buffBases = new Queue<BuffBase>();
                buffPools.Add(type, buffBases);
            }
            buffBases.Enqueue(buff);
        }
    }
}