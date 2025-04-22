using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NGUI_Slot:MonoBehaviour
{
    public void Awake()
    {
        UIEventListener.Get(gameObject).onHover += new UIEventListener.BoolDelegate((target, hoverState) => { OnHover(hoverState); });
        UIEventListener.Get(gameObject).onPress += new UIEventListener.BoolDelegate((target, pressState) =>
        {
            if (pressState)
            {
                OnPointerDown();
            }
        });
    }
    public virtual void SetItem(ItemDataConfig itemDataConfig,int count)
    {

    }
    public virtual void ReduceItem(int count)
    {

    }
    public void OnHover(bool pointerEnter)
    {
        if (pointerEnter)
        {
            OnPointerEnter();
        }
        else
        {
            OnPointerExit();
        }
    }
    public virtual void OnPointerExit()
    {

    }
    public virtual void OnPointerEnter()
    {

    }
    public virtual void OnPointerDown()
    {

    }
}

