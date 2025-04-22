using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionGet : MonoBehaviour
{ //public GameObject gun;
    public float timer=0 ;
   
    public static  float money;
    public Gun Gunplay;
    public bool ISONGUI=false;
    // Start is called before the first frame update
    void Start()
    {
      
       // gun = gameObject;
        Gunplay = gameObject.GetComponent<Gun>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (ISONGUI&&timer <= 0)
        {
           
            Gui.OffMoney();

        }
    }
    public void OnCollisionStay(Collision other)

    {
        if (other.collider.tag == "ITEM")
        {

            if (/*Input.GetKeyDown("q") &&*/ money >= other.collider.GetComponent<Item>().price)
            {
                if (other.collider.name == "EN(Clone)")
                {

                    // if (Gunplay.GetComponent<Gun>().energy < Gunplay.GetComponent<Gun>().energyUp)
                    // {
                    Gunplay.GetComponent<Gun>().energy += other.gameObject.GetComponent<Item>().value;
                    money -= other.collider.GetComponent<Item>().price;
                    other.collider.GetComponent<Item>().isDestory = true;
                    Debug.Log("+10!EN");
                    if (Gunplay.GetComponent<Gun>().energy > Gunplay.GetComponent<Gun>().energyUp)
                    {
                        Gunplay.GetComponent<Gun>().energy = Gunplay.GetComponent<Gun>().energyUp;//不能多余能量上限
                        money -= other.collider.GetComponent<Item>().price;
                        other.collider.GetComponent<Item>().isDestory = true;
                    }
                    //  }
                }
                if (other.collider.name == "ENUP(Clone)")
                {
                    Debug.Log("+10!UP");

                    Gunplay.GetComponent<Gun>().energyUp += other.gameObject.GetComponent<Item>().value;
                    money -= other.collider.GetComponent<Item>().price;
                    other.collider.GetComponent<Item>().isDestory = true;
                }
                if (other.collider.name == "POWER(Clone)")
                {
                    Debug.Log("+2!Powet");
                    Settings.power += other.gameObject.GetComponent<Item>().value;
                    money -= other.collider.GetComponent<Item>().price;
                    other.collider.GetComponent<Item>().isDestory = true;
                }
                if (other.collider.name == "SHOOTL")
                {
                    Gunplay.gun = GameObject.FindGameObjectWithTag("L");
                    Gunplay.Getsj();
                    money -= other.collider.GetComponent<Item>().price;
                    other.collider.GetComponent<Item>().isDestory = true;
                }
                if (other.collider.name=="SmallMapItem")
                {
                    Gui.SetSmallMap();
                }
            }
            else
            {
                timer = 2;
                Gui.NoMoney();
                ISONGUI = true;
            }
        }
        else
        {
            return;
        }
    }
}
