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
            MyDebug.DebugError($"GraphItemData��ȡ���� id��{graphID}");
        }
        this.drawFailed = drawFailed;
        this.drawSuccess = drawSuccess;
    }
    public GraphRecognitionPopupDefinition(GraphItemData graphItemData, Action<double> drawFailed = null, Action<double> drawSuccess = null) : base(UIPanelIdentity.GraphRecognitionPanel)
    {
        this.graphItemData = graphItemData;
        if (graphItemData == null)
        {
            MyDebug.DebugError($"GraphItemData��ȡ����");
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
    private Vector3[] filterVaildDisPoint;//��Ч����ɸѡ��
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
    /// ��ʼ����
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
    /// ��ͣ������������
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
    /// ���Ƶ�
    /// </summary>

    /// <summary>
    /// �����ͼ
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
        Debug.Log("�����ɰ�ť"+Time.frameCount);
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
            if (Vector3.Distance(lastPoint, point) < screenDisCoverNGUI)//����ľ�����ڱ���������ţ�������NGUI�����ĵ�λ
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
            MyDebug.DebugWarning("������Ϊ�ջ��һ����");
            return "";
        }
        List<Vector3> filterAfterPointDatas = FilerPointData(pointData);
        List<int> dirtyFields = new List<int>();
        dirtyFields.Capacity=filterAfterPointDatas.Count-1;//��ʱû���� ��β��ӣ���Ϊ�о�����Ӱ��
        for (int i = 0; i < filterAfterPointDatas.Count-1; i++)
        {
            Vector3 currentPoint = filterAfterPointDatas[i];
            Vector3 nextPoint =  filterAfterPointDatas[i+1];
            //�õ�����
            Vector3 dir = nextPoint - currentPoint;
            float distance = (nextPoint - currentPoint).magnitude;
            float yDisance = Mathf.Abs(nextPoint.y - currentPoint.y);
            //��֪����֮��ľ��� �� y������ͨ����������Ƕ�
            float betweenYRad = Mathf.Acos(yDisance/distance);//�ڱ߱�б�� //����󵽵���Զ������[0��90]
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
            Debug.Log("ûƥ��ɹ������ڽ����ݲ��");
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
    /// �õ������ֵ�������
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
    /// point1��Ϊԭ��
    /// </summary>
    /// <param name="point1"></param>
    /// <param name="point2"></param>
    /// <returns></returns>
    public int GetVectorInQuadrant(Vector3 point1,Vector3 point2)
    {
        //1 4����
        if (point2.x>=point1.x)//���޳���XY����ͬ�ĵ�
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
            //2 3����
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
    /// �����Ͳ��ÿ��Ǳ��������ˣ�
    /// </summary>
    public FilterPointReturnData FilterDirtyDirField(List<int> direField,List<Vector3> _pointData)
    {
        List<int> resultField=new List<int>();
        List<Vector3> resultPointData=new List<Vector3>();
        List<int> dirtyFields = direField;
        List<Vector3> pointData = _pointData;
        int lastDirField = 0;
        //�����߶�34 ��35  �߶�0ʱ����Ϊ0,1  �߶�1ʱ ��Ϊ1,2 �߶�2ʱ ��Ϊ2,3 ����ֻ��Ҫ��Ӷ�Ӧ�ĵ㼴�� �ڶ����� ����һ���߶����
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
    /// pointData�Ѿ�ɸѡ����������
    /// </summary>
    /// <param name="pointData"></param>
    /// <param name="dirFieldStr"></param>
    public string  FaultTolerantHandler(List<Vector3> pointData,string dirFieldStr)
    {
        //Ĭ���ݲʽ����������ֱ��ͼ�Ρ�
        //����Բ��������ͼ����˵ ��Ҫ����д-��Ҫ�ж����������ĵ�֮��ķ����������ڵĲ���Բ�������ݲ��

        //Ĭ���ݲ��
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
        //ǰ�������߶����Ƕ��ݲ�Ƚϣ������ݲ��ж�Ϊ�յ�
        //����Դ�����߶����Ƕ��ݲ�Ƚ�

        //ǰ�������߶����Ƕ��ݲ�Ƚ�
        
        List<Vector3> cornerList=new List<Vector3>();
        if (graphItemData.isLoopGraph)
        {
            cornerList.Add(pointData[0]);//ѭ��ͼ�εĻ���ʼ��Ҳ�����յ�
        }
        for (int i = 1; i < pointData.Count-1; i++)
        {
            //ȡ�߶�
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
