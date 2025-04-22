using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class CoroutineContainer
{
    private Dictionary<string,Coroutine> coroutines=new Dictionary<string, Coroutine>();
    public void StartCoroutine(MonoBehaviour mono, IEnumerator enumerator,string identity)
    {
        Coroutine coroutine;
        if (coroutines.TryGetValue(identity,out coroutine))
        {
            if (coroutine!=null)
            {
                mono.StopCoroutine(coroutine);
            }
        }
        var _coroutine = mono.StartCoroutine(enumerator);
        if (coroutines.ContainsKey(identity)==false)
        {
            coroutines.Add(identity, _coroutine);
        }
        else
        {
            coroutines[identity] = _coroutine;
        }
    }
}

