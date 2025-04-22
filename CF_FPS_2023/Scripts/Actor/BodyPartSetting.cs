using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BodyPartSetting:RagDollSetting
{
    public static BodyDamageRateConfig bodyDamageRateConfig=new BodyDamageRateConfig(3.2f,0.8f,1,0.7f,0.7f) { };
    public override void Awake()
    {
        base.Awake();
        foreach (var pair in boneTrans)
        {
            AllowScansBodyPart allowScansBodyPart = pair.Value.gameObject.AddComponent<AllowScansBodyPart>();
            allowScansBodyPart.setting = this;
            allowScansBodyPart.bodyPartType = pair.Key;
        }
    }
    public bool IsContain(Transform target)
    {
        return boneTrans.ContainsValue(target);
    }
    public RagDollBone GetBodyPartType(Transform target)
    {
        RagDollBone ragDollBone = boneTrans.First((p) => { return target == p.Value; }).Key;
        return ragDollBone;
    }

    public static BodyPartMainType GetBodyPartMainType(RagDollBone ragDollBone)
    {
        switch (ragDollBone)
        {
            case RagDollBone.LeftHips:
            case RagDollBone.RightHips:
                return BodyPartMainType.Thigh;

            case RagDollBone.LeftKnee:
            case RagDollBone.LeftFoot:
            case RagDollBone.RightKnee:
            case RagDollBone.RightFoot:
                return BodyPartMainType.LowerLeg;

            case RagDollBone.LeftArm:
            case RagDollBone.LeftElbow:
            case RagDollBone.RightArm:
            case RagDollBone.RightElbow:
                return BodyPartMainType.Arm;

            case RagDollBone.MiddleSpine:
            case RagDollBone.Pelvis:
                return BodyPartMainType.Body;
            case RagDollBone.Head:
            default:
                return BodyPartMainType.Head;
        }
    }


    public Dictionary<int, BodyDamageRateConfig> BodyDamageConfigList=new Dictionary<int, BodyDamageRateConfig>();
    string _Data_KAssetName = "_Data_BodyDamageRateConfig";
    public void GetConfig()
    {
        ExcelDataObject excelDataObject = ConfigResFactory.LoadConifg<ExcelDataObject>(_Data_KAssetName);
        //excelDataObject._Rows;
    }
}
public enum BodyPartMainType
{
    Head,
    Body,//胸/腹/背
    Arm,//腕/臂
    Thigh,//大腿
    LowerLeg//小腿
}
public struct BodyDamageRateConfig
{
    public float Head;
    public float Body;//胸/腹/背
    public float Arm;//腕/臂
    public float Thigh;//大腿
    public float LowerLeg;//小腿

    public BodyDamageRateConfig(float head, float body, float arm, float thigh, float lowerLeg)
    {
        Head = head;
        Body = body;
        Arm = arm;
        Thigh = thigh;
        LowerLeg = lowerLeg;
    }
    public float GetRate(BodyPartMainType bodyPartType)
    {
        switch (bodyPartType)
        {
            case BodyPartMainType.Head:
                return Head;
            case BodyPartMainType.Body:
                return Body;
            case BodyPartMainType.Arm:
                return Arm;
            case BodyPartMainType.Thigh:
                return Thigh;
            case BodyPartMainType.LowerLeg:
                return LowerLeg;
            default:
                return 0;
        }
    }
}

