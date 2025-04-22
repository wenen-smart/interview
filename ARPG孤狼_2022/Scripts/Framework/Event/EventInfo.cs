using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public struct EventInfo
{
    public string eventTips;
    public EventCode eventCode;
    public KeyBehaviourType keyBehaviourType;

    public EventInfo(string eventTips, EventCode eventCode, KeyBehaviourType keyBehaviourType)
    {
        this.eventTips = eventTips;
        this.eventCode = eventCode;
        this.keyBehaviourType = keyBehaviourType;
    }
}
public enum KeyBehaviourType
{
    RoleBehaviour = 1,//按键是响应人物行为的
    UIBehaviour = 2,//按键响应UI的
}
