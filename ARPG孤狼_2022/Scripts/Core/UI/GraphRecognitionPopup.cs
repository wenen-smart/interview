using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System.Text;
using System;

public class GraphRecognitionPopupDefinition : PopupDefinition
{
    public GraphItemData graphItemData;
    public Action<double> drawFailed;
    public Action<double> drawSuccess;
    public GraphRecognitionPopupDefinition(int graphID,Action<double> drawFailed=null,Action<double> drawSuccess=null) : base(UIPanelIdentity.GraphRecognitionPanel)
    {
        graphItemData = GraphDataConfig.Instance.GetGraphItemData(graphID);
        if (graphItemData==null)
        {
            MyDebug.DebugError($"GraphItemData读取不到 id：{graphID}");
        }
        this.drawFailed = drawFailed;
        this.drawSuccess = drawSuccess;
    }
    public GraphRecognitionPopupDefinition(GraphItemData graphItemData, Action<double> drawFailed = null, Action<double> drawSuccess = null) : base(UIPanelIdentity.GraphRecognitionPanel)
    {
        this.graphItemData = graphItemData;
        if (graphItemData == null)
        {
            MyDebug.DebugError($"GraphItemData读取不到");
        }
        this.drawFailed = drawFailed;
        this.drawSuccess = drawSuccess;
    }
}
public class GraphRecognitionPopup : BasePanel
{
    [SerializeField]
    private UILabel tipLabel;
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private LineRenderer tipLineRenderer;
    private GraphItemData graphItemData;
    [SerializeField]
    private UIButton completeButton;
    // Start is called before the first frame update
    public PaintBurshSetting paintBurshSetting;
    [SerializeField]
    private bool canPaint;
    private bool isPainted;
    private bool firstPaint = true;
    private Vector3 startPaintMousePosition;
    private Vector3 startPaintPosition;
    private Vector3 lastPaintMousePosition;
    public bool CanPaint { get { return canPaint; } set { canPaint = value;} }
    private Queue<Vector3> drawPointQueue=new Queue<Vector3>();
    [SerializeField]
    private DrawGraph drawGraph;
    private Vector3[] filterVaildDisPoint;//有效距离筛选；
    [SerializeField]
    private UIButton exchangeBtn;
    [SerializeField]
    private UIButton RefreshBtn;
    private Tweener graphFinishTween;
    public Action<double> drawFailed;
    public Action<double> drawSuccess;

    public void Awake()
    {
        lineRenderer.startWidth = paintBurshSetting.burshWidth;
        lineRenderer.startColor = paintBurshSetting.burshColor;
        lineRenderer.useWorldSpace = false;
        completeButton.onClick.Add(new EventDelegate(OnClickCompleteBtn));
    }
    public override void Init()
    {
        base.Init();
        drawGraph.finsihAction = FinishShowTipGraph;
    }
    public override void Enable()
    {
        base.Enable();
        CanPaint = true;
        ResetOperatorData();
        lineRenderer.positionCount = 0;
        var def = Definition as GraphRecognitionPopupDefinition;
        graphItemData = def.graphItemData;
        drawGraph.SetInfo(graphItemData,paintBurshSetting.vaildIntervalDis);
        drawGraph.StartDraw();
        this.drawFailed = def.drawFailed;
        this.drawSuccess = def.drawSuccess;
    }
    public override void Resume()
    {
        base.Resume();
        CanPaint = true;
    }
    public override void Pause()
    {
        base.Pause();
        CanPaint = false;
    }
    public override void Exit()
    {
        base.Exit();
        CanPaint = false;
    }
    private void Update()
    {
        if (CanPaint == false || (GameRoot.Instance.currentClickUIGO!=null&& GameRoot.Instance.currentClickUIGO!= gameObject))
        {
            if (isPainted==false&&GameRoot.Instance.currentClickUIGO== completeButton.gameObject)
            {
                CanPaint = false;
            }
            return;
        }
        if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0)&&isPainted==false)
            {
                StartPaint();
                return;
            }
            if (isPainted)
            {
                bool operateVaild = true;
                Vector3 currentPaintMousePos = Input.mousePosition;
                float betweenPointDis = (currentPaintMousePos - lastPaintMousePosition).magnitude;
                if (betweenPointDis<=paintBurshSetting.vaildIntervalDis)
                {
                    operateVaild = false;
                }
                if (operateVaild)
                {
                    PaintPoint(currentPaintMousePos);
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ReleasePointPaint();
        }
    }
    /// <summary>
    /// 开始绘制
    /// </summary>
    private void StartPaint()
    {
        if (firstPaint)
        {
            firstPaint = false;
            drawPointQueue.Clear();
            lineRenderer.positionCount = 0;
        }
        isPainted = true;
        startPaintMousePosition = Input.mousePosition;
    }
    /// <summary>
    /// 暂停（结束）绘制
    /// </summary>
    private void ReleasePointPaint()
    {
        isPainted = false;
    }
    public void PaintPoint(Vector3 screenPos)
    {
        lastPaintMousePosition = screenPos;
        lineRenderer.positionCount += 1;
        Vector3 localPos = CameraMgr.ScreentPointToNGUILocalPoint(lastPaintMousePosition, transform);
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, localPos);
    }
    /// <summary>
    /// 绘制点
    /// </summary>

    /// <summary>
    /// 完成作图
    /// </summary>
    private void EndPaint()
    {
        isPainted = false;
        drawPointQueue.Clear();
        filterVaildDisPoint = GetLineRenderPositions(lineRenderer);
        string result = PointDataFormat(filterVaildDisPoint);
        MatchGraphPerResult(result);
    }
    public Vector3[] GetLineRenderPositions(LineRenderer lineRenderer)
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        return positions;
    }
    private void OnClickCompleteBtn()
    {
        EndPaint();
        CanPaint = false;
        Debug.Log("点击完成按钮"+Time.frameCount);
    }
    public List<Vector3>  FilerPointData(Vector3[] pointData)
    {
        List<Vector3> filterAfterPoint = new List<Vector3>();
        Vector3 lastPoint=pointData[0];
        float screenDisCoverNGUI = (Mathf.Sqrt(Mathf.Pow(Screen.width*1.0f,2)+Mathf.Pow(Screen.height*1.0f,2))/Mathf.Sqrt(Mathf.Pow(GameRoot.Instance.UIROOT.manualWidth*1.0f,2)+Mathf.Pow(GameRoot.Instance.UIROOT.manualHeight*1.0f,2)))*paintBurshSetting.vaildIntervalDis;
        for (int i = 0; i < pointData.Length; i++)
        {
            if (i==0)
            {
                filterAfterPoint.Add(lastPoint);
                continue;
            }
            Vector3 point = pointData[i];
            if (point==lastPoint)
            {
                continue;
            }
            if (Vector3.Distance(lastPoint, point) < screenDisCoverNGUI)//这里的距离基于本界面的缩放，即基于NGUI画布的单位
            {
                continue;
            }
            lastPoint = point;
            filterAfterPoint.Add(point);
        }
        return filterAfterPoint;
    }
    public string PointDataFormat(Vector3[] pointData)
    {
        if (pointData==null||pointData.Length-1<=0)
        {
            MyDebug.DebugWarning("点数据为空或仅一个点");
            return "";
        }
        List<Vector3> filterAfterPointDatas = FilerPointData(pointData);
        List<int> dirtyFields = new List<int>();
        dirtyFields.Capacity=filterAfterPointDatas.Count-1;//暂时没考虑 首尾相接，因为感觉不会影响
        for (int i = 0; i < filterAfterPointDatas.Count-1; i++)
        {
            Vector3 currentPoint = filterAfterPointDatas[i];
            Vector3 nextPoint =  filterAfterPointDatas[i+1];
            //得到方向
            Vector3 dir = nextPoint - currentPoint;
            float distance = (nextPoint - currentPoint).magnitude;
            float yDisance = Mathf.Abs(nextPoint.y - currentPoint.y);
            //已知两点之间的距离 与 y轴差。可以通过反函数求角度
            float betweenYRad = Mathf.Acos(yDisance/distance);//邻边比斜边 //最后求到的永远都是在[0，90]
            float betweenYAngle = Mathf.Rad2Deg*betweenYRad;
            //float angle=Vector3.Angle(dir,Vector3.down); 
            int quad = GetVectorInQuadrant(currentPoint,nextPoint);
            int dirField = GetDirField(betweenYAngle,quad);
            dirtyFields.Add(dirField);
        }
        FilterPointReturnData filterPointReturnData = FilterDirtyDirField(dirtyFields,filterAfterPointDatas);
        List<int> fields = filterPointReturnData.dirField;
        string result = SumDirFieldResult(fields);
        return result;
    }
    public double MatchGraphPerResult(string result)
    {
        string dbResult = PointDataFormat(GetLineRenderPositions(drawGraph.lineRenderer));
        int cost = StringExtral.LevenshteinDistance(dbResult,result);
        double percent = StringExtral.LevenshteinDistanceMacthPercent(cost,dbResult,result);
        string cornerDirFieldStr = FaultTolerantHandler(filterVaildDisPoint.ToList<Vector3>(),result);
        //double cornerMatch = corner*1.0d / graphItemData.Corner;
        Debug.Log("data:"+dbResult);
        Debug.Log("result:"+result);
        Debug.Log("DirmatchPercent:"+percent);
        Debug.Log("DistanceCost:"+cost);
        if (percent<graphItemData.MinimumMatch)
        {
            Debug.Log("没匹配成功，现在进行容差处理。");
            int faultToleranCost = StringExtral.LevenshteinDistance(dbResult, cornerDirFieldStr);
            Debug.Log("DistancefaultToleranCost:"+faultToleranCost);
            double faultToleranPercent = StringExtral.LevenshteinDistanceMacthPercent(faultToleranCost, dbResult, cornerDirFieldStr);
            Debug.Log("faultToleranDirMatchPercent:"+faultToleranPercent);
            percent = faultToleranPercent;
        }
        if (percent>=graphItemData.MinimumMatch)
        {
            drawSuccess?.Invoke(percent);
        }
        else
        {
            drawFailed?.Invoke(percent);
        }
        return percent;
    }

    /// <summary>
    /// 得到所划分的象限域
    /// </summary>
    public int GetDirField(float angle,int quad)
    {
        int dirField = 0;
        switch (quad)
        {
            case 1:
                if (angle<=45)
                {
                    dirField = 1;
                }
                else if(angle>45&&angle<90)
                {
                    dirField = 2;
                }
                else
                {
                    dirField = 3;
                }
                break;
            case 2:
                if (angle <= 45)
                {
                    dirField = 3;
                }
                else if(angle>45&&angle<90)
                {
                    dirField = 4;
                }
                else
                {
                    dirField = 5;
                }
                break;
            case 3:
                if (angle <= 45)
                {
                    dirField = 5;
                }
                else if(angle>45&&angle<90)
                {
                    dirField = 6;
                }
                else
                {
                    dirField = 7;
                }
                break;
            case 4:
                if (angle <= 45)
                {
                    dirField = 7;
                }
                else if(angle>45&&angle<90)
                {
                    dirField = 8;
                }
                else
                {
                    dirField = 1;
                }
                break;
            default:
                break;
        }
        return dirField;
    }
    /// <summary>
    /// point1作为原点
    /// </summary>
    /// <param name="point1"></param>
    /// <param name="point2"></param>
    /// <returns></returns>
    public int GetVectorInQuadrant(Vector3 point1,Vector3 point2)
    {
        //1 4象限
        if (point2.x>=point1.x)//得剔除掉XY都相同的点
        {
            if (point2.y>point1.y)
            {
                return 1;
            }
            else
            {
                return 4;
            }
        }
        else
        {
            //2 3象限
            if (point2.y>=point1.y)
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }
    }
    /// <summary>
    /// 这样就不用考虑比例问题了？
    /// </summary>
    public FilterPointReturnData FilterDirtyDirField(List<int> direField,List<Vector3> _pointData)
    {
        List<int> resultField=new List<int>();
        List<Vector3> resultPointData=new List<Vector3>();
        List<int> dirtyFields = direField;
        List<Vector3> pointData = _pointData;
        int lastDirField = 0;
        //假设线段34 点35  线段0时，点为0,1  线段1时 点为1,2 线段2时 点为2,3 所以只需要添加对应的点即可 第二个点 由下一个线段添加
        for (int i = 0; i < dirtyFields.Count; i++)
        {
            int dirField = dirtyFields[i];
            if (lastDirField!=dirField)
            {
                lastDirField = dirField;
                resultField.Add(dirField);
                resultPointData.Add(pointData[i]);
            }
        }
        resultPointData.Add(pointData[pointData.Count-1]);
        FilterPointReturnData returnData = new FilterPointReturnData() { dirField = resultField, pointData = resultPointData };
        return returnData;
    }
    public string SumDirFieldResult(List<int> resultFields)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var field in resultFields)
        {
            sb.Append(field);
        }
        return sb.ToString();
    }
    public void ResetOperatorData()
    {
        firstPaint = true;
        isPainted = false;
    }
    public void FinishShowTipGraph()
    {
        if (graphFinishTween!=null)
        {
            graphFinishTween.Kill();
        }
        graphFinishTween=DOTween.To(()=>drawGraph.lineRenderer.material.color,(targetColor)=> { drawGraph.lineRenderer.material.color=targetColor; },Color.clear,2).SetDelay(2).OnComplete(()=> { drawGraph.gameObject.SetActive(false); });
        GameRoot.Instance.ShowFloatDialog(0,4,false);
    }
    /// <summary>
    /// pointData已经筛选掉了连续的
    /// </summary>
    /// <param name="pointData"></param>
    /// <param name="dirFieldStr"></param>
    public string  FaultTolerantHandler(List<Vector3> pointData,string dirFieldStr)
    {
        //默认容差方式处理是适用直线图形。
        //对于圆那种曲线图形来说 需要单独写-需要判断所有连续的点之间的方向域是相邻的才是圆（允许容差）。

        //默认容差处理
        switch (graphItemData.faultTolerantWay)
        {
            case FaultTolerantWay.Default:
                return DefaultFaultTolerant(pointData, dirFieldStr);
                break;
            case FaultTolerantWay.Circle:
                break;
            default:
                break;
        }
        return "";
    }
    public string DefaultFaultTolerant(List<Vector3> pointData,string dirFieldStr)
    {
        //前后两条线段做角度容差比较，大于容差判断为拐点
        //或已源数据线段做角度容差比较

        //前后两条线段做角度容差比较
        
        List<Vector3> cornerList=new List<Vector3>();
        if (graphItemData.isLoopGraph)
        {
            cornerList.Add(pointData[0]);//循环图形的话开始点也记做拐点
        }
        for (int i = 1; i < pointData.Count-1; i++)
        {
            //取线段
            var line1 = pointData[i]-pointData[i - 1];
            var line2 = pointData[i + 1] - pointData[i];
            if (Vector3.Angle(line1,line2)>graphItemData.faultAngle)
            {
                cornerList.Add(pointData[i]);
                continue;
            }
        }
        Debug.Log("GraphCornerReverse:"+cornerList.Count);
        string dirFieldResult = PointDataFormat(cornerList.ToArray());
        return dirFieldResult;
    }
    public void RefreshDrawGraphTip()
    {
        if (graphItemData!=null)
        {
            if (graphFinishTween != null)
            {
                graphFinishTween.Kill();
            }
            drawGraph.SetInfo(graphItemData,paintBurshSetting.vaildIntervalDis);
            drawGraph.StartDraw();
        }
    }
    public void RefreshDrawGraphPopup()
    {
        if (graphItemData != null)
        {
            ResetOperatorData();
            canPaint = true;
            RefreshDrawGraphTip();
        }
    }
    public void ExchangeGraph()
    {
        //int graphCount = GraphDataConfig.Instance.graphItemDataDic.Count;
        //int id = 0;
        //if (graphItemData!=null)
        //{
        //    id = (graphItemData.id+1) % graphCount;
        //}
        //graphItemData=GraphDataConfig.Instance.GetGraphItemData(id);
        RefreshDrawGraphPopup();
        UIControl.Instance.TestGraphRecognition();
        UIControl.Instance.TestGraphRecognition();
    }
}
[System.Serializable]
public class PaintBurshSetting
{
    public float vaildIntervalDis;//InScreenRect
    public float burshWidth;
    public Color burshColor;
}
public struct FilterPointReturnData
{
    public List<int> dirField;
    public List<Vector3> pointData;
}
