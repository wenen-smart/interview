using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum AnimClipJudgeWay
{
    ClipName=1,
    Tag=1<<1,
    TagAndName=ClipName|Tag,
}
public enum RootMotionHandlerAim
{
    Rotation=1,
    Position=1<<1,
    All=Rotation|Position,
}
public enum CalcuateRootMotionModal
{
    CommonAdd,//delta=delta+increase*mult  rotate*=increase
    AddThenAverage,//delta=(delta+increase*mult)/2
    AverageIncreaseThenAdd,//delta=delta+(increase*mult)/2
}
public enum RootMotionProcessClear
{
    None = 0,
    ClearX = 1,
    ClearY = 1 << 1,
    ClearZ = 1 << 2,
    ClearW = 1 << 3,
    ClearXAndY = ClearX | ClearY,
    ClearXAndZ = ClearX | ClearZ,
    ClearXAndW = ClearX | ClearW,
    ClearYAndZ = ClearY | ClearZ,
    ClearYAndW = ClearY | ClearW,
    ClearZAndW = ClearZ | ClearW,
    ClearAll = ClearX | ClearY | ClearZ | ClearW,
}