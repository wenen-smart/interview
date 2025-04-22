using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DrawGraph:MonoBehaviour
{
    public LineRenderer lineRenderer;
    private float vaildDis;
    public float drawSpeed = 0.1f;
    private Vector3[] points;
    private Gradient gradient;
    private bool isLoopGraph = false;
    private int pointIndex;
    private int nextPointIndex;
    private bool startDraw = false;
    private float betweenPrg = 0;
    private Vector3 lastPoint;
    public Action finsihAction;
    private Color materialColor;
    public void Awake()
    {
        materialColor = lineRenderer.material.color;
    }
    public void SetInfo(GraphItemData graphData,float vaildDis)
    {
        points = graphData.graphPoints;
        this.gradient = graphData.colorGradient;
        this.isLoopGraph = graphData.isLoopGraph;
        this.vaildDis = vaildDis;
    }
    public void StartDraw()
    {
        ResetData();
        gameObject.SetActive(true);
        pointIndex = 0;
        nextPointIndex = 1;
        startDraw = true;
        AddPoint(points[pointIndex]);
        lineRenderer.colorGradient = gradient;
        lineRenderer.loop = false;
        lineRenderer.material.color = materialColor;
    }
    public void Update()
    {
        if (startDraw)
        {
            Vector3 point1 = points[pointIndex];
            Vector3 point2 = points[nextPointIndex];
            betweenPrg += Mathf.Clamp01(Time.deltaTime * drawSpeed);
            Vector3 currentPos = Vector3.Lerp(point1,point2,betweenPrg);
            if (Vector3.Distance(currentPos,lastPoint)>=vaildDis)
            {
                AddPoint(currentPos);
            }
            else
            {
                if (betweenPrg >= 1)
                {
                    ArrivePoint();
                    AddPoint(point2);
                }
            }
        }
    }
    public void ArrivePoint()
    {
        if (pointIndex >= points.Length - 2)
        {
            startDraw = false;
            lineRenderer.loop = isLoopGraph;
            pointIndex = nextPointIndex;
            //Finsih
            finsihAction?.Invoke();
            return;
        }
        betweenPrg = 0;
        pointIndex = nextPointIndex;
        nextPointIndex += 1;
    }
    public void AddPoint(Vector3 point)
    {
        lastPoint = point;
        lineRenderer.positionCount += 1;
        lineRenderer.SetPosition(lineRenderer.positionCount-1,point);
    }
    public void ResetData()
    {
        lineRenderer.positionCount = 0;
        
        betweenPrg = 0;
        startDraw = false;
        pointIndex = 0;
        nextPointIndex = 0;
    }
}

