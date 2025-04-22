using Assets.Resolution.Scripts.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardSwitchWeapon : MonoBehaviour
{
    [MultSelectTags]
    public SwitchWeaponKeyCode switchKeyCode;
    public KeyCode exchangeKeycode=KeyCode.None;
    private int mindigitalCode = (int)KeyCode.Alpha0;
    private int maxdigitalCode = (int)KeyCode.Alpha9;
    private MyRuntimeInventory _RuntimeInventory;
    private MyRuntimeInventory RuntimeInventory
    {
        get
        {
            if (_RuntimeInventory == null)
            {
                _RuntimeInventory = GetComponent<MyRuntimeInventory>();
            }
            return _RuntimeInventory;
        }
    }
    private int weaponCount { get { return RuntimeInventory.weaponList.Count; } }
    public void Update()
    {
        if (switchKeyCode.IsSelectThisEnumInMult(SwitchWeaponKeyCode.AlphaNum))
        {
            AlphaCtrl();
        }
        if(switchKeyCode.IsSelectThisEnumInMult(SwitchWeaponKeyCode.KeyCode))
        {
            CodeQCtrl();
        }
    }
    public void AlphaCtrl()
    {
        for (int i = 1; i <= weaponCount; i++)
        {
            if (Input.GetKeyDown((KeyCode)(mindigitalCode + i)))
            {
                RuntimeInventory.ExchangeWeapon(i - 1);
            }
        }
    }
    public void CodeQCtrl()
    {
        if (Input.GetKeyDown(exchangeKeycode))
        {
            RuntimeInventory.ExchangeWeaponByScroll(1);
        }
    }
    //public void OnGUI()
    //{
    //    GUI.Label(new Rect(Screen.width-300,0,300,30),new GUIContent("KeyboardSwitchWeapon:"+gameObject.name));
    //}
}
public enum SwitchWeaponKeyCode
{
    AlphaNum=1,
    KeyCode=1<<1
}
