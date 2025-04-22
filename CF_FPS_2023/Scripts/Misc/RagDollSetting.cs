using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RagDollSetting:ActorComponent
{
    public Transform Pelvis;
    public Transform LeftHips;
    public Transform LeftKnee;
    public Transform LeftFoot;
    public Transform RightHips;
    public Transform RightKnee;
    public Transform RightFoot;
    public Transform LeftArm;
    public Transform LeftElbow;
    public Transform RightArm;
    public Transform RightElbow;
    public Transform MiddleSpine;
    public Transform Head;
    //public float TotalMass;
    //public float Strength;
    //public bool FlipForward;
    private bool isGeneatedRagDoll;
    protected Dictionary<RagDollBone, Transform> boneTrans=new Dictionary<RagDollBone, Transform>();
    public bool IsGeneatedRagDoll { get => isGeneatedRagDoll; set => isGeneatedRagDoll = value; }

    public override void  Awake()
    {
        base.Awake();
        boneTrans.Add(RagDollBone.Pelvis,Pelvis);
        boneTrans.Add(RagDollBone.LeftHips,LeftHips);
        boneTrans.Add(RagDollBone.LeftKnee,LeftKnee);
        boneTrans.Add(RagDollBone.LeftFoot,LeftFoot);
        boneTrans.Add(RagDollBone.RightHips,RightHips);
        boneTrans.Add(RagDollBone.RightKnee,RightKnee);
        boneTrans.Add(RagDollBone.RightFoot,RightFoot);
        boneTrans.Add(RagDollBone.LeftArm,LeftArm);
        boneTrans.Add(RagDollBone.LeftElbow,LeftElbow);
        boneTrans.Add(RagDollBone.RightArm,RightArm);
        boneTrans.Add(RagDollBone.RightElbow,RightElbow);
        boneTrans.Add(RagDollBone.MiddleSpine,MiddleSpine);
        boneTrans.Add(RagDollBone.Head,Head);
    }
    public  void OnGeneric()
    {
        isGeneatedRagDoll = true;
    }
    public void DeleteRagDoll()
    {
        if (isGeneatedRagDoll == false)
        {
            return;
        }
        isGeneatedRagDoll = false;
        Transform[] list = GetALL();
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == null)
            {
                continue;
            }
            
            CharacterJoint joint = list[i].GetComponent<CharacterJoint>();
            if (joint != null)
            {
                DestroyImmediate(joint);
            }
        }
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == null)
            {
                continue;
            }
            Rigidbody rigidbody = list[i].GetComponent<Rigidbody>();
           
            Collider capsuleCollider = list[i].GetComponent<Collider>();
            if (rigidbody != null)
            {
                DestroyImmediate(rigidbody);
            }
            if (capsuleCollider != null)
            {
                DestroyImmediate(capsuleCollider);
            }

        }
        Pelvis = null;
        LeftHips = null;
        LeftKnee = null;
        LeftFoot = null;
        RightHips = null;
        RightKnee = null;
        RightFoot = null;
        LeftArm = null;
        LeftElbow = null;
        RightArm = null;
        RightElbow = null;
        MiddleSpine = null;
        Head = null;
        DestroyImmediate(this);
    }
    public void DisableRagDoll()
    {
        Transform[] list = GetALL();
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == null)
            {
                continue;
            }
            Rigidbody rigidbody = list[i].GetComponent<Rigidbody>();
            CharacterJoint joint = list[i].GetComponent<CharacterJoint>();
            Collider capsuleCollider = list[i].GetComponent<Collider>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = true;
            }
            if (joint != null)
            {
                //暂时不知道如何禁用
            }
            if (capsuleCollider != null)
            {
                capsuleCollider.enabled = false;
            }

        }
    }
    public void AddForceOrTorque(RagDollBone ragDollBone,Vector3 force,Vector3 torque=default(Vector3))
    {
        Transform bone;
        boneTrans.TryGetValue(ragDollBone,out bone);
        if (bone!=null)
        {
            Rigidbody rigidbody = bone.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                if (force!=Vector3.zero)
                {
                    rigidbody.AddForce(force,ForceMode.Impulse);
                }
                if (torque!=Vector3.zero)
                {
                    rigidbody.AddTorque(force,ForceMode.Impulse);
                }
            }
        }
    }
    public void EnableRagDoll()
    {
        Transform[] list = GetALL();
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == null)
            {
                continue;
            }
            Rigidbody rigidbody = list[i].GetComponent<Rigidbody>();
            CharacterJoint joint = list[i].GetComponent<CharacterJoint>();
            Collider capsuleCollider = list[i].GetComponent<Collider>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
            }
            if (joint != null)
            {
                //暂时不知道如何禁用
            }
            if (capsuleCollider != null)
            {
                capsuleCollider.enabled = true;
            }

        }
    }
    public void CtrlAllColliderTrigger(bool isTrigger)
    {
        Transform[] list = GetALL();
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == null)
            {
                continue;
            }
            Collider col =  list[i].GetComponent<Collider>();
            if (col)
            {
                col.isTrigger = isTrigger;
            }
        }

    }
    public void OnlySaveCollider()
    {
        Transform[] list = GetALL();
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == null)
            {
                continue;
            }

            CharacterJoint joint = list[i].GetComponent<CharacterJoint>();
            if (joint != null)
            {
                DestroyImmediate(joint);
            }
        }
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == null)
            {
                continue;
            }
            Rigidbody rigidbody = list[i].GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                DestroyImmediate(rigidbody);
            }
        }
    }
    public Transform[] GetALL() 
    {
        Transform[] list = new Transform[] {
            Pelvis,
            LeftHips,
            LeftKnee,
            LeftFoot,
            RightHips,
            RightKnee,
            RightFoot,
            LeftArm,
            LeftElbow,
            RightArm,
            RightElbow,
            MiddleSpine,
            Head
        };
        return list;
    }
}

