using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class PopupDefinition
{
    private UIPanelIdentity PanelIdentity;
    public UIPanelIdentity GetUIPanelIdentity()
    {
        return PanelIdentity;
    }
    public UIPanelInfo panelInfo;
    public bool PauseOtherWhenPopup = true;//当此面板启用 是否影响其他面板
    public UILayerType LayerType;//层级标识
    public PopupDefinition(UIPanelIdentity PanelIdentity,UILayerType LayerType=UILayerType.Default,bool PauseOtherWhenPopup=true)
    {
        this.PanelIdentity=PanelIdentity;
        this.PauseOtherWhenPopup = PauseOtherWhenPopup;
        this.LayerType = LayerType;
    }
}

