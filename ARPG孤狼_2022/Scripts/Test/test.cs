using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Presets;
using UnityEngine;
using RayFire;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    [MultSelectTagsAttribute]
    public Tagg matchAttackerTags;
    public Transform transDir;
    public AudioSourceConfig AudioSourceConfig;
    public AudioSource audioSource;
    public RayfireRigid rayfireRigid;
    public enum Tagg
    {
        T1=1<<0,
        T2=1<<1,
        T3=1<<2,
    }
    void Start()
    {
        int i = 10;
        i.StringToInt("100");
        Debug.Log(i);
        Debug.Log("Tags:"+matchAttackerTags);
        Debug.Log("LayermaskValue:"+(int)PlayerableConfig.Instance.pickItemLayerMask);
        Debug.Log("LayermaskValue:"+PlayerableConfig.Instance.pickItemLayerMask.value);

    }

    public void Update()
    {
        
        //if (Input.GetKey(KeyCode.Alpha1))
        //{
        //    PlayerFacade.Instance.characterAnim.anim.Play("Air_Attack01_2");
        //}
        //if (Input.GetKey(KeyCode.Alpha2))
        //{
        //    PlayerFacade.Instance.characterAnim.anim.Play("Air_Attack02");
        //}
        //if (Input.GetKey(KeyCode.Alpha3))
        //{
        //    PlayerFacade.Instance.characterAnim.anim.Play("Air_Attack03_Start");
        //}

        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    FindObjectOfType<ClonelyEnemyController>().moveComponent.AddImpluseForce(Vector3.up*8);
        //}
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    QuaternionRotation();
        //}
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    PlayerFacade.Instance.roleController.moveComponent.AddImpluseForce(PlayerFacade.Instance.transform.forward*50000);
        //}
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    MyDebug.DebugPrint(Time.realtimeSinceStartup.ToString());
        //    GameTimeScaleCtrl.Instance.SetTimeSacle(0.1f, 500, 1, () => { MyDebug.DebugPrint(Time.timeScale.ToString()); MyDebug.DebugPrint(Time.realtimeSinceStartup.ToString()); });
        //     MyDebug.DebugPrint(Time.timeScale.ToString());
        //}
        if (Input.GetKeyDown(KeyCode.U))
        {
            PlayerFacade.Instance.roleController.EquipWeaponHandler(PlayerFacade.Instance.weaponManager.currentWeapon);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            PlayerFacade.Instance.roleController.EquipWeaponHandler(PlayerFacade.Instance.weaponManager.currentWeapon);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            

        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            var enemyActor = GameObject.Find("ClonelyEnemy (1)").GetComponent<ActorSystem>();
            var asset = EndKillTimeLineConfig.Instance.GetTimeLineAsset(4, PlayerFacade.Instance.actorSystem.roleDefinition, enemyActor.roleDefinition); 
        EndSkillTimeLineManager.Instance.SetEndSkillTimeLine(PlayerFacade.Instance.actorSystem.GetActorComponent<RoleController>(), enemyActor.GetActorComponent<RoleController>(), asset);
    }
    }
    
    // Update is called once per frame
    public void QuaternionRotation()
    {
        Vector3 dir = Quaternion.Euler(transDir.right*-45)*transDir.forward;
        MyDebug.DebugLine(transDir.position,transDir.position+transDir.forward,Color.green,10);
        MyDebug.DebugLine(transDir.position,transDir.position+dir,Color.red,10);
    }
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(1);
    }
    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log(2);
    }
}

namespace ABS
{
    public class AAA
    {

    }
}
