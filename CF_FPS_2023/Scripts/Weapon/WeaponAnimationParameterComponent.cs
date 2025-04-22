using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponAnimationParameterComponent : MonoBehaviour
{
    public WeaponAnimationStruct[] weaponAnimationStructs;

    public WeaponAnimationStruct GetAniamtionParameter(string IdentitySymbol)
    {
        return weaponAnimationStructs.SingleOrDefault((s)=> { return s.IdentitySymbol == IdentitySymbol; });
    }
}
[System.Serializable]
public struct WeaponAnimationStruct
{
    public string IdentitySymbol;
    public int layer;
    public string stateName;
    //TODO
}
