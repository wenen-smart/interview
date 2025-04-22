using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemolitionItem : MonoBehaviour
{
    public void Awake()
    {
        Initialized();
    }
    public void Initialized()
    {
        GameRoot.Instance.AddTimeTask(5000,DestoryCollision);
    }
    public void DestoryCollision()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var item in rigidbodies)
        {
            if (item!=null)
            {
                Destroy(item);
            }
            
        }
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var item in colliders)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
    }
}
