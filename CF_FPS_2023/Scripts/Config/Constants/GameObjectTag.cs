using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// * 绘制多值枚举选择框,0 全部不选, -1 全部选中, 其他是枚举之和
         //* 枚举值 = 当前下标值 ^ 2
         //* 默认[0 ^ 2 = 1, 1 ^ 2 = 2, 4, 16, .....]
         //*/
/// </summary>
  public  enum GameObjectTag
    {
    Player=1<<0,
    Enemy=1<<1,
    Stair=1<<2,
    }
public static class TagManager
{
    public static string[] GameObjectTagList = Enum.GetNames(typeof(GameObjectTag));
}

