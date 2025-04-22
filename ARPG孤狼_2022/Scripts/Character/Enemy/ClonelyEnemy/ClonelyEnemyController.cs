using UnityEngine;
using System.Collections;

public class ClonelyEnemyController : EnemyController
{   
    public float targetForward=0;
    public  void Update()
    {
        if (TimeLineManager.Instance.TimeLineIsPlay)
        {
            return;
        }

        UpdateBlend();
    }

    private EnemyFSMMachine<ClonelyEnemyController> m_EnemyFSMMachine;
    public override void Awake()
    {
        base.Awake();
        InitiaizeFSM();
    }
    public override void CalcuateMove()
    {
        moveComponent.CalcuateMove();
    }

    public override void SetBlend(float tar)
    {
        targetForward = tar;
    }
    public override void UpdateBlend()
    {
        float current = anim.GetFloat("Forward");
        if (current == targetForward || targetForward == 1)
        {
            SetBlend(targetForward);
            return;
        }
        if (Mathf.Abs(current - targetForward) < 0.02f)
        {
            current = targetForward;
            SetBlend(current);
        }
        else
        {
            current = Mathf.MoveTowards(current, targetForward, Time.deltaTime * forwardBlendSpeed);
            SetBlend(current);
        }
    }
    public void InitiaizeFSM()
    {
        attackDeltaTimer = new MyTimer();
        
    }
    public void LateUpdate()
    {
        m_EnemyFSMMachine?.Tick();
    }
    
    public float attackDeltaTime=2;
    public MyTimer attackDeltaTimer;

    public int comboAttackId;
    public override void Attack()
    {
        SkillSystem.Instance.AttackSkillHandler(this, comboAttackId);
    }
    public override void Turn() 
    {
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(GameObject.FindGameObjectWithTag("Player").transform.position - transform.position, Vector3.up));
    }

    public override bool InAttackRange()
    {
        return (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).magnitude < attackDistance;
    }
    public override bool SatisfyAttackCondition()
    {
        bool satisfy= InAttackRange()&& AttackDeltaTimeOut();
        return satisfy;
    }
    public override void AttackEnter()
    {
        
    }
    public override void OpenAttackTime()
    {
        attackDeltaTimer.Go(attackDeltaTime);
    }
    public override bool AttackDeltaTimeOut()
    {
        if (attackDeltaTimer.timerState==MyTimer.TimerState.Finish)
        {
         
            return true;
        }
        return false;
    }
    public override void OnHit(int hitAction, float angle = 0, int defenseAction = 0)
    {
        base.OnHit(hitAction,angle,defenseAction);
        anim.speed = 0.1f;
        GameRoot.Instance.AddTask(100, () => { anim.speed = 1; });
    }


   
    public override void UpPickAttacked()
    {
        base.UpPickAttacked();
        //anim.SetInteger(AnimatorParameter.HitAction.ToString(), 22);
        characterController.Move(Vector3.up * 60 * Time.deltaTime);
        //timeScale
    }

    public override void AirComboAttacked()
    {

    }


}

