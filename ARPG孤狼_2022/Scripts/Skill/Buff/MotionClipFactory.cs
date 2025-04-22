using Buff.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MotionClipFactory : CommonSingleTon<MotionClipFactory>,IObjectFactory<MotionModifyClip>
{
    public  Dictionary<string, ObjectPool<MotionModifyClip>> motionClipPool=new Dictionary<string, ObjectPool<MotionModifyClip>>();
    public void Init()
    {
       
    }

    public  MotionModifyClip GetClip(MotionModifyType motionModifyType)
    {
        MotionModifyClip clip = null;
        switch (motionModifyType)
        {
            case MotionModifyType.LineTracking:
                break;
            case MotionModifyType.Tracking:
                break;
            case MotionModifyType.MoveToPosition:
                clip=PopItem(typeof(MoveToPosition_MotionClip).Name);
                break;
            default:
                break;
        }
        return clip;
    }

    public void PushItem(MotionModifyClip objectEntity)
    {
        ObjectPool<MotionModifyClip> clipPool = null;
        string symbol = objectEntity.GetType().Name;
        motionClipPool.TryGetValue(symbol, out clipPool);
        if (clipPool!=null)
        {
            clipPool.PushItem(objectEntity);
        }
        else
        {
             MyDebug.DebugError($"没找到对应标识:{symbol} 无法存回对象池");
        }
    }

    public MotionModifyClip PopItem(string symbol)
    {
        if (symbol==null)
        {
            return null;
        }
        ObjectPool<MotionModifyClip> clipPool = null;
        MotionModifyClip clip = null;
        if (motionClipPool.TryGetValue(symbol, out clipPool)) { }
        if (clipPool==null)
        {
            clipPool = new ObjectPool<MotionModifyClip>();
            clipPool.symbol = symbol;
            motionClipPool.Add(symbol,clipPool);
        }
        Type type = Type.GetType(symbol);
        if (type != null)
        {
            clip = clipPool.GetObject(type);
        }
        else
        {
            MyDebug.DebugError("当前没有从程序集中找到对应的Type，可能是程序集不一致或者未指定命名空间");
        }
        return clip;
    }

    public static bool IsVelocityMode(MotionModifyType motionModifyType)
    {
        return motionModifyType.HasFlag(MotionModifyType.LineTracking | MotionModifyType.Tracking);
    }
    public static bool IsMoveToPositionMode(MotionModifyType motionModifyType)
    {
        return motionModifyType.HasFlag(MotionModifyType.MoveToPosition);
    }
}

