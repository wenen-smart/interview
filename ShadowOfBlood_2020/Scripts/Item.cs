using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool isDestory = false;
    public float price = 0;
    public float value = 10;
    // Start is called before the first frame update
    private void Update()
    {
        if (isDestory)
        {
            Debug.Log("ppoo!");
            // 销毁当前游戏物体
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    
    void OnCollisioStay(Collision collision)
    {
       
    }
}
