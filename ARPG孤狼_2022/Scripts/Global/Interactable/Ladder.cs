using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public Transform[] stagePoint;
    public float slideDis;//两边点距离中间的间隔 局部坐标

    public LadderStagePointData GetUpStartPointData()
    {
        return GetStagePointData(0);
    }
    public LadderStagePointData GetDownStartPointData()
    {
        return GetStagePointData(stagePoint.Length-1);
    }
    public LadderStagePointData GetStagePointData(int index)
    {
        if (index>=stagePoint.Length)
        {
            var nullData = new LadderStagePointData();
            nullData.stageIndex = -1;
            return nullData;
        }
        Vector3 center = stagePoint[index].position;
        float worldSlideDis = transform.localScale.x* slideDis;
        Vector3 right = center + transform.right * worldSlideDis;
        Vector3 left = center - transform.right * worldSlideDis;
        LadderStagePointData ladderStagePointData=new LadderStagePointData(center,left,right,index,stagePoint.Length);
        return ladderStagePointData;
    }

    
}
public struct LadderStagePointData
{
    public Vector3 LeftPoint;
    public Vector3 rightPoint;
    public Vector3 centerPoint;
    public int stageIndex;
    public int allStageCount;
    public LadderStagePointData(Vector3 centerPoint,Vector3 left,Vector3 right,int stageIndex,int stageCount)
    {
        LeftPoint = left;
        rightPoint = right;
        this.centerPoint = centerPoint;
        this.stageIndex = stageIndex;
        this.allStageCount = stageCount;
    }
}
