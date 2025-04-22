using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Game : MonoSingleTon<Game>, I_Init
{
    public void Init()
    {

    }
    
    public void StartGame(bool isContinue)
    {
        UIManager.Instance.PopPanel();
        if (isContinue==false)
        {
            GameLoader.LoadScene(SCENE_NAME.Map);
        }
    }
}

