using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLoaderPopup : BasePanel
{
    public Transform popup;
    public Text prgTxt;
    public Text tipTxt;
    public Scrollbar scrollbar;


    public void UpdateProgressText(float prg)
    {
        prgTxt.text = "Loading..." + (int)(prg * 100)+"%";
        scrollbar.size = prg;
    }
    public void UpdateTip(params string[] tipsGroup)
    {
        
    }
}
