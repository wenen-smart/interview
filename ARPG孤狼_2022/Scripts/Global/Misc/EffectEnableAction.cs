using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RayFire;

public class EffectEnableAction : MonoBehaviour
{
    public ParticleSystem[] particleSystems;
    public void OnEnable()
    {
        foreach (var item in particleSystems)
        {
            item.Play();
            
        }
    }
}
