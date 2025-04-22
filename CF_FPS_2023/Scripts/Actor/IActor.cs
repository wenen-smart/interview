using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface  IActor :  I_Init
{
    public ActorSystem actorSystem { get; set; }
    public  void OnDestory() { }
    public void AddActorComponent<T>() where T : ActorComponent;
    public T GetActorComponent<T>() where T : MonoBehaviour,IActor;
}

