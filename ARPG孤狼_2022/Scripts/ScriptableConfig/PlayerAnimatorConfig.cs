using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAnimatorConfig", menuName = "CreateConfig/CreatePlayerAnimator", order = 1)]
public class PlayerAnimatorConfig : ScriptableConfigTon<PlayerAnimatorConfig>
{

    new  public static void SetScriptablePath()
    {
        scriptablePath = PathDefine.playerAnimatorConfigPath;
    }

    [Header("---------DeltaMult------------")]
    public float runToStopMult = 2.5f;
    public float runRushMult = 5;
    public float jump1Mult = 10;
    public float turnVeloMult = 2;
    public float land2Mult = 1.2f;
    public float coverToStandMult = 2f;
    public float jumpToHangMult = 2f;
    public float canelShimmyMult = 2;
    public float hangToClimbMult = 1;
    public float standtoHangMult = 100;
    public float lockLocomotionMult = 100f;
    public float comboAttackMotionMult = 35f;

    [Header("------Animation_Curve-------")]
    public AnimationCurve jump1MotionCurve;
    public AnimationCurve jump2MotionCurve;
    public AnimationCurve fallIdleMotionCurve;
    public AnimationCurve canelShimmyMotionCurve;

    [Header("------Test-------")]
    public float enemyHitMotionMult=40;
    public float test = 1;
    public float lockStateJumpMult = 30;

}

