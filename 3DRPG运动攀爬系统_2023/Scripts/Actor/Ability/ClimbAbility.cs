using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public enum ClimbType
{
    None = 0,
    LowerClimbOver = 1,//低位爬上
    HigherClimbOver = 1<<1,//高位爬上
    ContinueClimb = 1<<2,//持续攀爬-攀爬移动
}
public enum ClimbProcessState//贴墙跑算是特殊的一种攀爬
{
    None,
    InProcess,//攀爬动作持续中
    CancelClimb,//攀爬取消 
    ClimbWholeFinish,//攀爬完整结束
}
public class ClimbAbility:ActorComponent
{
    [SerializeField] public float characterEndOffset = 0.32f;
    [SerializeField] public float headPointEndOffset = 0.9f;
    [SerializeField] public float headPointHeightOffset = 0.5f;
    [SerializeField] public float climbtoTop_EndOffset = 0.9f;

    public BaseRaycastHit readyClimbForecastHitinfo;
    public BaseRaycastHit climbForecastHitinfo;
    [SerializeField] public bool ToggleClimbAbility = true;

    [SerializeField] public float climbTowardOffset = 0.25f;
    [SerializeField] public float climbToTopOffset = 0.3f;
    [SerializeField] public RaycastHit towardHitInfo;
    [SerializeField] public RaycastHit climbUpRootHitInfo;
    public RaycastHit ledge_Hit;


    [SerializeField, Range(-1, 1)]public  float climbToTop_hand_delta = 0.3f;

    [SerializeField, Range(-1, 1)] public float leftHandDis = 0f;
    [SerializeField, Range(-1, 1)] public float rightHandDis = 0.2f;

  

    public float lowObstacleMinHeight_Climb = 1f;
    public float lowObstacleMaxHeight_Climb = 2f;
    public float highObstacleMaxHeight_Climb = 3;
    [SerializeField] public Transform HeadPoint;
    //private RoleController roleController;
    //private Movement movement;
    private float characterHeight=0;
    [MultSelectTags]public ClimbType climbAbilityType;
    public void SetCharacterHeight(float h)
    {
        characterHeight = h;
    }

    public  bool CheckContinueClimb(bool haveJump)
    {
        if (IsHaveTheClimbAbility(ClimbType.ContinueClimb) == false)
        {
            return false;
        }
        RaycastHit hitInfo;
        //攀爬
        if (Physics.Raycast(HeadPoint.position, actorSystem.transform.forward, out hitInfo, headPointEndOffset, ~(1 << LayerMask.NameToLayer("Character"))))
        {
            Debug.Log("check1");
            if (haveJump||(Physics.Raycast(actorSystem.transform.position + Vector3.up * highObstacleMaxHeight_Climb, actorSystem.transform.forward, out RaycastHit hitInfo_3, headPointEndOffset, ~(1 << LayerMask.NameToLayer("Character")))))
            {
                Debug.Log("check2");
                DebugTool.DrawLine(HeadPoint.position, hitInfo.point, Color.green, 0);
                if (Physics.BoxCast(HeadPoint.position + actorSystem.transform.up * headPointHeightOffset - transform.forward * headPointEndOffset / 2, new Vector3(0.4f, 0.2f, headPointEndOffset / 2), actorSystem.transform.forward, out RaycastHit readyHitinfo, Quaternion.LookRotation(actorSystem.transform.forward), headPointEndOffset, ~(1 << LayerMask.NameToLayer("Character")))&readyClimbForecastHitinfo.ConvertTo(readyHitinfo))
                {
                    Debug.Log("check3");
                    DebugTool.DrawBox(HeadPoint.position + actorSystem.transform.forward * headPointEndOffset / 2 + actorSystem.transform.up * headPointHeightOffset, new Vector3(0.4f, 0.2f, headPointEndOffset / 2), Color.green, 0);
                    return true;
                }
                else
                {
                    DebugTool.DrawBox(HeadPoint.position + actorSystem.transform.forward * headPointEndOffset / 2 + actorSystem.transform.up * headPointHeightOffset, new Vector3(0.4f, 0.2f, headPointEndOffset / 2), Color.red, 0);
                }
            }
        }
        else
        {
            
            Debug.DrawLine(HeadPoint.position, HeadPoint.position + actorSystem.transform.forward * headPointEndOffset, Color.red, 0);
        }
        return false;
    }
    public ClimbType CheckLowOrHighClimb()
    {
        if (IsHaveTheClimbAbility(ClimbType.LowerClimbOver|ClimbType.HigherClimbOver) == false)
        {
            return ClimbType.None;
        }
        bool isHitMinHeightlowObstacle;
        bool isHitMaxHeightlowObstacle;
        bool isHitMaxHeightHigherObstacle;
        isHitMinHeightlowObstacle = (Physics.Raycast(actorSystem.transform.position + Vector3.up * lowObstacleMinHeight_Climb, actorSystem.transform.forward, out RaycastHit hitinfo_1, headPointEndOffset, ~(1 << LayerMask.NameToLayer("Character"))));
        if (isHitMinHeightlowObstacle)
        {
            isHitMaxHeightlowObstacle = (Physics.Raycast(actorSystem.transform.position + Vector3.up * lowObstacleMaxHeight_Climb, actorSystem.transform.forward, out RaycastHit hitInfo_2, headPointEndOffset, ~(1 << LayerMask.NameToLayer("Character"))));
            if (isHitMaxHeightlowObstacle)
            {
                isHitMaxHeightHigherObstacle = (Physics.Raycast(actorSystem.transform.position + Vector3.up * highObstacleMaxHeight_Climb, actorSystem.transform.forward, out RaycastHit hitInfo_3, headPointEndOffset, ~(1 << LayerMask.NameToLayer("Character"))));
                if (isHitMaxHeightHigherObstacle == false)
                {
                    if (Physics.Raycast(hitInfo_2.point + Vector3.up * characterHeight, Vector3.down, out ledge_Hit, characterHeight, ~(1 << LayerMask.NameToLayer("Character"))))
                    {
                        readyClimbForecastHitinfo.ConvertTo(hitInfo_2);
                        return ClimbType.HigherClimbOver;
                    }
                }
            }
            else
            {
                if (isHitMinHeightlowObstacle)
                {
                    if (Physics.Raycast(hitinfo_1.point + Vector3.up * characterHeight, Vector3.down, out ledge_Hit, characterHeight, ~(1 << LayerMask.NameToLayer("Character"))))
                    {
                        readyClimbForecastHitinfo.ConvertTo(hitinfo_1);
                        return ClimbType.LowerClimbOver;
                    }
                }
            }

        }
        return ClimbType.None;
    }

    public bool IsHaveTheClimbAbility(ClimbType offer)
    {
        return climbAbilityType.IsSelectThisEnumInMult(offer);
    }
}
