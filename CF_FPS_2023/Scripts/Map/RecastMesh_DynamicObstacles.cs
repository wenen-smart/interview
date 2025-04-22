using Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RecastMesh_DynamicObstacles:MonoBehaviour
{
    public float restoreNodeWhenOverDis = 0.2f;
    public float checkTime = 0.2f;
    public bool toggle;//仅测试用。
    private Vector3 pos1;
    private Queue<GraphNode> queue=new Queue<GraphNode>();
    private bool isHandled = false;//可能会发生禁用的点，正好把路堵住了。导致自己想向前走，无法走，那需要关闭这个。
    public MyTimer myTimer;
    public bool IsHandled { get { return isHandled; } 
        set {
            if (isHandled&&value==false)
            {
                RecoverNodeWalkable();
            }
            isHandled = value;
        } 
    }
    void Start()
    {
        myTimer = TimeSystem.Instance.CreateTimer();
        UpdateStatus();
    }
    public void UpdateStatus()
    {
        pos1 = transform.position;
    }
    void RecoverNodeWalkable()
    {
        myTimer.timerState = MyTimer.TimerState.Idle;
        while (queue.Count != 0)
        {
			queue.Peek().Walkable = true;
			queue.Dequeue();
		}
    }
    public void Update()
    {
        if (toggle)
        {
            if (Vector3.Distance(transform.position, pos1) < restoreNodeWhenOverDis)
            {
                if (IsHandled == false)
                {
                    if (myTimer.timerState==MyTimer.TimerState.Idle)
                    {
                        myTimer.Go(checkTime);
                    }
                    if (myTimer.timerState==MyTimer.TimerState.Finish)
                    {
                        IsHandled = true;
                        NNInfoInternal nNInfoInternal = AstarPath.active.data.recastGraph.GetNearest(transform.position, NNConstraint.None);
                        var node = nNInfoInternal.node;
                        if (node.Walkable)
                        {
                            node.Walkable = false;
                            queue.Enqueue(node);
                        }
                    }
                }
            }
            else
            {
                pos1 = transform.position;
                myTimer.timerState = MyTimer.TimerState.Idle;
                if (IsHandled)
                {
                    //恢复原先禁用的点。
                    IsHandled = false;
                }
            }

        }
    }
	public void OnDisable()
	{
        IsHandled = false;
	}
	public void OnDestroy()
	{
		if (Application.isPlaying)
		{
            IsHandled = false;
		}
        
	}
}

public class RecastMesh_DynamicObstaclesManager : ScriptSingleTon<RecastMesh_DynamicObstaclesManager>, I_Init
{
    //public HashSet<int,>
    public void Init()
    {

    }
}
