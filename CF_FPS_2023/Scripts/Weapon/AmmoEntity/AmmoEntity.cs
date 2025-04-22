using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoEntity : MonoBehaviour
{
    protected RoleController user;
    protected void RegisterUser(RoleController roleController)
    {
        user = roleController;
    }
}
