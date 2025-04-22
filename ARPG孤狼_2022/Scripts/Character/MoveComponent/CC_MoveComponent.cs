using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CC_MoveComponent : MoveComponent
{
    private CharacterController characterController;

    protected override Vector3 _mVelocity { get => characterController.velocity; set  { } }

    public override void Init()
    {
        characterController = GetComponent<CharacterController>();
    }
    public override void CalcuateMove()
    {
        var velo = (deltaPosition/*可能需要修改*/ + transform.forward * baseMoveSpeed * speedMult + increaseVelo);
        if (characterController.isGrounded == false)
        {
            velo -= CalcuateGravityVelocity();
        }
        characterController.Move(velo);
        deltaPosition = Vector3.zero;
        ClearIncreaseVelo();
    }

    public override void EnableGravity(bool isGravity)
    {
        base.EnableGravity(isGravity);
    }
    public override Vector3 CalcuateGravityVelocity()
    {
        if (useCustomGravity==false && ignoreGravity == false)
        {
            return -Physics.gravity*Time.deltaTime;
        }
        return CalcuateCustomGravityVelocity();
    }
    public override void AddImpluseForce(Vector3 force)
    {
        base.AddImpluseForce(force);
        
    }

    public override void MotionClipTick()
    {
        
    }
    public override bool IsOnGround()
    {
        return characterController.isGrounded||JudgeOnGroundHandler?.Invoke()==true;
    }

}

