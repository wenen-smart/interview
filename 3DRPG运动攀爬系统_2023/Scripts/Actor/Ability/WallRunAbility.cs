using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunAbility : ActorComponent
{
    [Header("±º≈‹œ¬µƒ¿Î«Ωæ‡¿Î")]
    [SerializeField] public float run_characterEndOffset = 0.32f;
    [Header("Ω˚÷πœ¬µƒ¿Î«Ωæ‡¿Î")]
    [SerializeField] public float hold_characterEndOffset = 0.32f;
    [Header("π˝∂…ÀŸ∂»")]
    [SerializeField] public float c_EndOffset_TransitionSpeed = 2;
    public float current_characterEndOffset { get; set; }
    [HideInInspector] public float headPointEndOffset = 0.9f;
    [SerializeField] public float headPointHeightOffset = 0.5f;
    public float hold_alignCastHeight = 1.7f;
    public float run_alignCastHeight = 1f;
    [SerializeField] public Transform HeadPoint;
    [SerializeField] public RaycastHit towardHitInfo;
    [SerializeField] public RaycastHit climbUpRootHitInfo;
    public BaseRaycastHit readyClimbForecastHitinfo;//stand to climb
    public BaseRaycastHit climbForecastHitinfo;//update-Œ¥¿¥µƒ≈ ≈¿µ„
    public float highObstacleMaxHeight_Climb = 3;
    [SerializeField] public float climbTowardOffset = 0.25f;
    [SerializeField] public float climbTowardSafeDistance = 0.5f;
    [SerializeField] public float climbToTopOffset = 0.9f;
    [SerializeField] public float climbtoTop_EndOffset = 0.9f;
    [SerializeField] public float maxAngle = 45f;
    public bool openDebug = false;
    public Collider lastWall { get;  set; }
    [HideInInspector] public CapsuleCollider capsuleCollider;
    public bool autofall = true;
    public override void ActorComponentAwake()
    {
        base.ActorComponentAwake();
        
    }
    public override void Init()
    {
        base.Init();
        headPointEndOffset = GetForwardcastLen(run_alignCastHeight);
    }

    void OnValidate()
    {
        if (isInitialize)
        {
            headPointEndOffset = GetForwardcastLen(run_alignCastHeight);
            DebugTool.DebugPrint("headPointEndOffset:" + headPointEndOffset);
        }
    }
    public bool CheckIsCanWallRun(bool ignoreLastWall=false,float castMultiply=1)
    {
        //TODO environment check
        RaycastHit hitInfo;
        //≈ ≈¿
        if (Physics.Raycast(transform.position + transform.up * /*0.45f*/ run_alignCastHeight, actorSystem.transform.forward, out hitInfo, headPointEndOffset*castMultiply, ~(1 << LayerMask.NameToLayer("Character"))))
        {
            bool horizontalAxis_high = (Physics.Raycast(actorSystem.transform.position + Vector3.up * highObstacleMaxHeight_Climb, actorSystem.transform.forward, out RaycastHit hitInfo_3, GetForwardcastLen(highObstacleMaxHeight_Climb) * castMultiply, ~(1 << LayerMask.NameToLayer("Character"))));

            if (horizontalAxis_high)
            {
                DebugTool.DrawLine(transform.position + transform.up * /*0.45f*/ run_alignCastHeight, hitInfo.point, Color.green, 0);
                if (Physics.BoxCast(transform.position + transform.up * /*0.45f*/ run_alignCastHeight + actorSystem.transform.up * headPointHeightOffset - transform.forward * GetForwardcastLen(headPointHeightOffset+run_alignCastHeight) / 2, new Vector3(0.4f, 0.2f, GetForwardcastLen(headPointHeightOffset+run_alignCastHeight) / 2), actorSystem.transform.forward, out RaycastHit readyHitinfo, Quaternion.LookRotation(actorSystem.transform.forward), headPointEndOffset*castMultiply, ~(1 << LayerMask.NameToLayer("Character"))) & readyClimbForecastHitinfo.ConvertTo(readyHitinfo))
                {
                    DebugTool.DrawBox(transform.position + transform.up * /*0.45f*/ run_alignCastHeight + actorSystem.transform.forward * GetForwardcastLen(headPointHeightOffset+run_alignCastHeight) / 2 + actorSystem.transform.up * headPointHeightOffset, new Vector3(0.4f, 0.2f, GetForwardcastLen(headPointHeightOffset+run_alignCastHeight) / 2), Color.green, 0);
                    if (ignoreLastWall&&readyClimbForecastHitinfo.collider==lastWall)
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    DebugTool.DrawBox(transform.position + transform.up * /*0.45f*/ run_alignCastHeight + actorSystem.transform.forward * GetForwardcastLen(headPointHeightOffset+run_alignCastHeight) / 2 + actorSystem.transform.up * headPointHeightOffset, new Vector3(0.4f, 0.2f, GetForwardcastLen(headPointHeightOffset+run_alignCastHeight) / 2), Color.red, 0);
                }
            }
        }
        else
        {
            DebugTool.DrawLine(transform.position + transform.up * /*0.45f*/ run_alignCastHeight, HeadPoint.position + actorSystem.transform.forward * headPointEndOffset, Color.red, 0);
        }

        return false;
    }
    public void Update()
    {
        if (openDebug)
        {
            DebugTool.DrawBox(transform.position + transform.up * /*0.45f*/ run_alignCastHeight + actorSystem.transform.forward * headPointEndOffset / 2 + actorSystem.transform.up * headPointHeightOffset, new Vector3(0.4f, 0.2f, headPointEndOffset / 2), Color.yellow, 0);
            DebugTool.DrawLine(transform.position + transform.up * /*0.45f*/ run_alignCastHeight, HeadPoint.position + actorSystem.transform.forward * headPointEndOffset, Color.yellow, 0);
            DebugTool.DrawBox(transform.position + transform.up * /*0.45f*/ run_alignCastHeight + actorSystem.transform.forward * GetForwardcastLen(headPointHeightOffset+run_alignCastHeight) / 2 + actorSystem.transform.up * headPointHeightOffset, new Vector3(0.4f, 0.2f, GetForwardcastLen(headPointHeightOffset+run_alignCastHeight) / 2), Color.blue, 0);
        }
    }
    public float GetForwardcastLen(float height)
    {
        return Mathf.Tan(Mathf.Deg2Rad*maxAngle)*height+capsuleCollider.radius;
    }
}
