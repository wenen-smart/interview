using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Const_Animation
{
    public const string BackAir_Roll_End = "BackAir_Roll_End";
    public const string Forward = "Forward";
    public const string Horizontal = "Horizontal";
}
public enum WeaponActionType
{
    Get=-1,
    Idle=0,
    Walk=1,
    Run=2,
    Shot=3,
    Recharge=4,
    Aim_Idle=5,
    Aim_Walk=6,
    Aim_Shot=7,
    Hide=8,
    PreThrow=9,
    ThrowOut=10,
    PullBolt=11,
    
}
public enum WeaponUseInputType
{
    Default,
    Thump,//重击
}

