using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public struct RecoilData
{
    public RecoilDandohElement[] recoilDandohs;

    public float Get(int shot_Num)
    {
        if (recoilDandohs==null||recoilDandohs.Length==0)
        {
            Debug.LogError("Lose RecoilDandoh Data");
            return 0;
        }
        int dandohIndex = 0;
        int ammoUpperLimit = 0;
        do
        {
            ammoUpperLimit = recoilDandohs[dandohIndex].ammoCount;//弹道轨迹数据 
            dandohIndex++;
        } while (dandohIndex<recoilDandohs.Length&&ammoUpperLimit<shot_Num);
        //ammoUpperLimit>=shot_Num  to choose and use this dandoh
        dandohIndex--;
        RecoilDandohElement dandohsElem = recoilDandohs[dandohIndex];
        if (dandohsElem.left+dandohsElem.right==0)
        {
            return 0;
        }
        float dropPoint = UnityEngine.Random.Range(dandohsElem.left,dandohsElem.right*1.0f);
        return (dropPoint>0?1:-1)*dandohsElem.recoilMultiply.x;
    }
    public float GetRecoilYMult(int shot_Num)
    {
        if (recoilDandohs == null || recoilDandohs.Length == 0)
        {
            Debug.LogError("Lose RecoilDandoh Data");
            return 0;
        }
        int dandohIndex = 0;
        int ammoUpperLimit = 0;
        do
        {
            ammoUpperLimit = recoilDandohs[dandohIndex].ammoCount;//弹道轨迹数据 
            dandohIndex++;
        } while (dandohIndex < recoilDandohs.Length && ammoUpperLimit < shot_Num);
        //ammoUpperLimit>=shot_Num  to choose and use this dandoh
        dandohIndex--;
        RecoilDandohElement dandohsElem = recoilDandohs[dandohIndex];
        return dandohsElem.recoilMultiply.y;
    }
}

//后座力弹道轨迹Element
[Serializable]
public class RecoilDandohElement
{
    public Vector2 recoilMultiply=Vector2.one;
    public int ammoCount;
    public int left;
    public int right;
}
