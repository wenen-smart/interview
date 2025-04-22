using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiedPopup : BasePanel
{
    public void DeathAfter()
    {
        UIManager.Instance.TryPopAllPanel();
        GameLoader.LoadScene(SCENE_NAME.HOME);
    }
}
