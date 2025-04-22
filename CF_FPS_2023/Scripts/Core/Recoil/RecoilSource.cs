using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilSource : MonoBehaviour
{
    private RecoilListener _RecoilListener;
    public void Awake()
    {
        _RecoilListener = FindObjectOfType<RecoilListener>();
    }
    public void PutRecoil(RecoilData recoilData)
    {
        _RecoilListener.ReceiveRecoil(recoilData);
    }
}
