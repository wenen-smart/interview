using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRigid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().isKinematic=true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 axis = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        if (axis.magnitude>0.1f)
        {
            GetComponent<Rigidbody>().MovePosition(transform.position+(new Vector3(axis.x,0,axis.y))*Time.deltaTime);
        }
    }
}
