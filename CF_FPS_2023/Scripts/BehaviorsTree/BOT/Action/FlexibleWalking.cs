using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Bot")]
public class FlexibleWalking : Action
{
    [Header("横向走位大小区间，Only Positive Number")]
    public Vector2 horizontalWalkPosition;//x is min y is max
    public float speed = 2;
    public float dampTime = 0.2f;
    public SharedTransform target;
    public bool isRandom = false;
    public int minCount = 0;
    public int maxCount = 10;
    private int targetWalkCount;
    private int walkCount = 0;
    private RobotController bot;
    private RichAI agent;
    private int right = 0;
    private List<Vector3> vectors=new List<Vector3>();
    private float startTime;
    private float duration;

    public override void OnAwake()
    {
        base.OnAwake();
        bot = GetComponent<ActorComponent>().GetActorComponent<RobotController>();
        agent = bot.moveAgent;
    }
    public override void OnStart()
    {
        base.OnStart();
        if (isRandom)
        {
            targetWalkCount = Random.Range(minCount, maxCount);
        }
        else
        {
            targetWalkCount = 0;
        }
        walkCount = 0;
        NextWalkPositioin();
    }
    public override TaskStatus OnUpdate()
    {
        var delta = (vectors[1] - vectors[0]).normalized*speed*Time.deltaTime;
        if (startTime + duration <= Time.time)
        {
            walkCount += 1;
            if (walkCount >= targetWalkCount)
            {
                return TaskStatus.Success;
            }
            NextWalkPositioin();
        }
        
        bot.animatorMachine.animator.SetFloat(Const_Animation.Horizontal, right*bot.GetAnimMoveParameterByState(ControllerState.Walking), dampTime, Time.deltaTime);
        agent.Move(delta);
        agent.rotation=agent.SimulateRotationTowards(bot.ProjectOnSelfXZPlane((target.Value.position-bot.transform.position)),Time.deltaTime*360);
        return TaskStatus.Running;
    }
    public void NextWalkPositioin()
    {
        if (right == 0)
        {
            right = Random.Range(-1, 1);
            if (right == 0)
            {
                right = 1;
            }
        }
        else
        {
            right *= -1;
        }


        bot.StopSeeker();
        NNConstraint nNConstraint = NNConstraint.Default;
        nNConstraint.constrainWalkability = true;
        vectors.Clear();
        vectors.Add(bot.transform.position);
        vectors.Add(bot.transform.position + bot.transform.right * right);
        NNInfo nNInfo = AstarPath.active.GetNearest(vectors[1], nNConstraint);
        vectors[1] = nNInfo.position;
        Path path = ABPath.Construct(vectors[0],vectors[1]);
        bot.seeker.CancelCurrentPathRequest();
        startTime = Time.time;
        duration = (vectors[1] - vectors[0]).magnitude / speed;
        DebugTool.DrawWireSphere(vectors[1],0.2f,Color.blue,5,"FlexibleWalking");
    }
    public override void OnConditionalAbort()
    {
        base.OnConditionalAbort();
        bot.StopSeeker();
    }

    public float RandomWalkPositionMagnitude()
    {
        return Random.Range(horizontalWalkPosition.x,horizontalWalkPosition.y);
    }
}
