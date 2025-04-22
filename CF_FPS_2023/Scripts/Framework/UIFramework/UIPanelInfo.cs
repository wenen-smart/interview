using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class UIPanelInfo : ISerializationCallbackReceiver {
    [NonSerialized]
    public UIPanelIdentity panelIdentity;
    public string path;
    public string panelIdentityString;
    [NonSerialized]
    public UIPopupType popupType;
    public string popupTypeString;
    public void OnAfterDeserialize()
    {
        panelIdentity = (UIPanelIdentity)System.Enum.Parse(typeof(UIPanelIdentity), panelIdentityString) ;
        popupType = (UIPopupType)System.Enum.Parse(typeof(UIPopupType), popupTypeString) ;
    }

    public void OnBeforeSerialize()
    {
        
    }
}
