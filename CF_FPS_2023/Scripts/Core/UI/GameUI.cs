using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameUI:MonoSingleTon<GameUI>,I_Init
{
    public UIRoot UIROOT;
    [HideInInspector]
    public UICamera UICAMERA;
    [HideInInspector]
    public UIManager UIManagerRoot;

    public Crosshair_HUD crosshair_HUD;
    public PlayerInfoHUD playerInfoHUD;
    public void Init()
    {
        UIManagerRoot = UIManager.Instance;
    }
}
