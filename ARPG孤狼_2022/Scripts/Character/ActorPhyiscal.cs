using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using System.Collections;

public enum CheckPointType
{
    HeadTop,
    Head,
    Chest,//胸腔
    Waist,//腰
    Leg,//大腿
    Feet,//脚
    LeftOffsetPoint,//腰部左侧的偏移点。在人物外的点
    RightOffsetPoint,
}
public class ActorPhyiscal:ActorComponent
    {
    public SphereBodyCast[] sphereBodyCasts= { 
    new SphereBodyCast(0.4f,0.4f,true,false), new SphereBodyCast(1.1f,0.4f,false,false),
    new SphereBodyCast(1.7f,.4f,false,true)
    };
    
    private CapsuleCollider ownCollider;
    public LayerMask walkableLayer= ~(1 << 0);
    //public LayerMask IgnoreLayer;
    [TagField]
    public string ignoreTag;
    private SphereBodyCast feet;
    private SphereBodyCast head;
    public Dictionary<CheckPointType, Transform> bodyPointTrans = new Dictionary<CheckPointType, Transform>();
    [SerializeField]private Collider touchCollider;
    [SerializeField]private Collider walkableCollider;
    public Vector3 checkTargetBoxCastCenterFromActor = Vector3.one*10;
    protected Vector3? lastOnGroundPosition;
    [SerializeField]protected float checkInGroundMaxDis = 0.32f;
    [HideInInspector] public static string[] stairTags;
    public void Awake()
    {
        if (stairTags==null)
        {
            stairTags=TagManager.GameObjectTagList.Where((_name) => { return (PlayerableConfig.Instance.StairsTag).IsSelectThisEnumInMult((GameObjectTag)Enum.Parse(typeof(GameObjectTag), _name)); }).ToArray();
        }
        ownCollider = GetComponent<CapsuleCollider>();
        if (ownCollider==null)
        {
            Debug.Log("-------ownCollider Reference is Null--------");
        }
        foreach (var item in sphereBodyCasts)
        {
            if (item.isFeet)
            {
                feet = item;
            }
            if (item.isHead)
            {
                head = item;
            }
        }
        if (feet==null)
        {
            Debug.Log("-------feet Reference is Null--------");
        }

        if (head==null)
        {
            Debug.Log("-------head Reference is Null--------");
        }
       
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (TouchCollider!=collision.collider)
        {
            TouchCollider = collision.collider;
            JudgeEnterGround();
        }
    }
    public void OnTouchObjectEnter(Collider collision)
    {
        if (collision==null)
        {
            return;
        }
        //TODO
        MyDebug.DebugPrint("TouchObject:"+collision.gameObject.name);
    }
    public void OnCollisionExit(Collision collision)
    {
        TouchCollider = null;
        JudgeEnterGround();
    }
    public Collider GetCurrentTouchCollider()
    {
        return TouchCollider;
    }


    [Serializable]
    public class SphereBodyCast
    {
        public float offset;
        public float radius;
        public bool isFeet;
        public bool isHead;
        public Color drawColor = Color.red;
        public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal;

        public SphereBodyCast(float offset, float radius, bool isFeet, bool isHead)
        {
            this.offset = offset;
            this.radius = radius;
            this.isFeet = isFeet;
            this.isHead = isHead;
        }
    }

    public Vector3 CalcuateBodySphereVerticalPos(int index)
    {
        return this.CalcuateLocalToWorld(Vector3.up* sphereBodyCasts[index].offset);
    }
    public Vector3 CalcuateBodySphereVerticalPos(SphereBodyCast index)
    {
        return this.CalcuateLocalToWorld(Vector3.up * index.offset);
    }
    
    public void FixedUpdate()
    {
        IsOnGround();  
    }
    private bool isGround = false;


    public bool IsGround { get => isGround; protected set => isGround = value; }
    public Collider TouchCollider
    {
        get => touchCollider;
        protected set
        {
            if (value != touchCollider)
            {
                touchCollider = value; OnTouchObjectEnter(touchCollider);//在这里处理当前碰到的物体，因为下面方法-》如果踩在地面上会将地面也赋值给touch，而用Stay可能造成循环一直更新touchcollider，所以采用这种方式了。
            }
        }
    }

    private void JudgeEnterGround()
    {
        if (TouchCollider == null)
        {
            walkableCollider = null;
        }
        else
        {
            if ((walkableLayer.value & (1 << TouchCollider.gameObject.layer)) != 0 && IsIgnoreChecking(TouchCollider) == false)
            {
                walkableCollider = TouchCollider;
            }
        }
        
        if (walkableCollider==null)
        {
            isGround = false;
        }
        TryUpdateGroundState();
    }
    private void IsOnGround()
    {
        if (lastOnGroundPosition.HasValue==false||(Vector3.Distance(transform.position,lastOnGroundPosition.Value)>=0.1f))
        {
            TryUpdateGroundState();
        }
    }
    private void TryUpdateGroundState()
    {
        walkableCollider = CastGroundReturnCollider(checkInGroundMaxDis);
        if (walkableCollider == null)
        {
            var tempCollider = CastGroundReturnCollider(checkInGroundMaxDis*1.5f);
            if (tempCollider!=null&&string.IsNullOrEmpty(tempCollider.tag)==false)
            {
                if (stairTags.Contains(tempCollider.tag))
                {
                    walkableCollider = tempCollider;
                }
            }
        }
        TouchCollider = walkableCollider;
        IsGround=walkableCollider!=null;
        if (IsGround)
        {
            lastOnGroundPosition = transform.position;
        }
    }
    public bool CastGround(float maxDis, bool drawDebug = false)
    {
        return CastGroundReturnCollider(maxDis,drawDebug)!=null;
    }
    private Collider CastGroundReturnCollider(float maxDis, bool drawDebug = false)
    {
        if (feet != null)
        {
            var origin = CalcuateBodySphereVerticalPos(feet);
            var dir = -transform.up;
            RaycastHit hitInfo;
            if (drawDebug)
            {
                Debug.DrawLine(origin, origin + dir * maxDis);

            }

            if (Physics.SphereCast(origin, 0.1f, dir, out hitInfo, maxDis, walkableLayer.value, feet.triggerInteraction))
            {
                if (IsIgnoreChecking(hitInfo) == false)
                {
                    return hitInfo.collider;
                }
            }
        }
        else
        {
            Debug.Log("-------feet Reference is Null-------- So The CastGround noVaild");
        }
        return null;
    }

    public bool IsIgnoreChecking(RaycastHit raycastHit)
    {
        return raycastHit.collider.tag == ignoreTag || raycastHit.collider == ownCollider;
    }
    public bool IsIgnoreChecking(Collider collider)
    {
        return collider.tag == ignoreTag ||collider == ownCollider;
    }

    public void UpdateSphereBodyCast()
    {
       
    }

    

    public bool SpherecastFromBodySphere(bool isFeet,bool isHead, Vector3 dir, float maxDis,LayerMask layerMask, out RaycastHit raycastHit, float radius = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,bool drawLine=true,Color color=default(Color))
    {
        SphereBodyCast sphereBodyCast=null;
        if (isFeet)
        {
            sphereBodyCast = feet;
        }
        else if (isHead)
        {
            sphereBodyCast = head;
        }else
        {
            foreach (var item in sphereBodyCasts)
            {
                if (item!=feet&&item!=head)
                {
                    sphereBodyCast = item;
                }
            }
        }
        if (sphereBodyCast==null)
        {
            Debug.Log("---------No Find the BodySphere From bodySpheres -----------");
            raycastHit = new RaycastHit();
            return false;
        }

        var origin = CalcuateBodySphereVerticalPos(sphereBodyCast);


        if (radius==-1)
        {
            radius = sphereBodyCast.radius;
        }
        if (drawLine)
        {
            if (color==default(Color))
            {
                color = Color.red;
            }
            
            Debug.DrawLine(origin, origin + dir * maxDis,color,2);

        }

        if (Physics.SphereCast(origin, radius, dir,out raycastHit, maxDis, layerMask.value,queryTriggerInteraction))
        {
            return true;
        }

        return false;
    }
    public bool IsOwnCollider(RaycastHit raycastHit)
    {
        return raycastHit.collider!=null&&raycastHit.collider == ownCollider;
    }

    public void AddBodyPoint(CheckPointType bodyPoint,Transform trans)
    {
        if (bodyPointTrans!=null&&bodyPointTrans.ContainsKey(bodyPoint)==false)
        {
            bodyPointTrans.Add(bodyPoint,trans);
        }
    }

    public Transform GetCheckPoint(CheckPointType bodyPoint)
    {
        if (bodyPointTrans!=null&& bodyPointTrans.ContainsKey(bodyPoint))
        {
            return bodyPointTrans[bodyPoint];
        }
        return null;
    }
    public bool BoxCast(CheckPointType checkPointType,Vector3 halfAxisSize,Vector3 direction,Quaternion quaternion, LayerMask layerMask,out RaycastHit raycastHit,float maxDistancce=1000,QueryTriggerInteraction queryTriggerInteraction=QueryTriggerInteraction.UseGlobal, Vector3 offsetVec = default(Vector3))
    {
        var pointTrans=GetCheckPoint(checkPointType);
        if (pointTrans == null)
        {
            raycastHit = new RaycastHit();
            Debug.LogWarning($"--------从CheckPoint字典中获取不到对应的{checkPointType.ToString()} 键值 pointTrans为null ---------");
            return false;
        }
        Vector3 center = pointTrans.position+ offsetVec;
        
        if (Physics.BoxCast(center, halfAxisSize, direction,out raycastHit, quaternion, maxDistancce, layerMask.value, queryTriggerInteraction))
        {
            return true;
        }
        return false;
    }
    public bool BoxCast(Vector3 center, Vector3 halfAxisSize, Vector3 direction, Quaternion quaternion, LayerMask layerMask, out RaycastHit raycastHit , float maxDistancce = 1000, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (Physics.BoxCast(center, halfAxisSize, direction, out raycastHit, quaternion, maxDistancce, layerMask.value, queryTriggerInteraction))
        {
            return true;
        }
        return false;
    }

    public Transform CheckLockTargetIsExist()
    {
        Transform lockTarget=null;
        var bodymiddleOrigin = Vector3.up * transform.GetComponent<RoleController>().characterCollider.center.y;
        var offsetOrigin = new Vector3(0, 0, -0.2f);
        var boxCenter = transform.TransformPoint(Vector3.forward * checkTargetBoxCastCenterFromActor.z + offsetOrigin+bodymiddleOrigin);
        var halfExents = checkTargetBoxCastCenterFromActor; 
        var colliders = Physics.OverlapBox(boxCenter, halfExents,Quaternion.LookRotation(transform.forward),PlayerableConfig.Instance.enemyLayerMask.value);
        var minDistance = Mathf.Infinity;
        foreach (var item in colliders)
        {
            if (item == ownCollider)
            {
                continue;
            }
            Debug.Log(item.transform.name);
            var actorManager = item.transform.GetComponentInChildren<EnemyController>();
            if (!actorManager|| actorManager.damageable.IsDie)
            {
                Debug.Log(item.transform.name+ "   false Actor");
                continue;
            }
            
         var offvec=(transform.position - actorManager.transform.position);
            offvec.y = 0;
            var offsetDis = offvec.sqrMagnitude;
            if (offsetDis<minDistance)
            {
                minDistance = offsetDis;
                lockTarget = actorManager.transform;
            }
        }
        return lockTarget;
    }
    /// <summary>
    /// 获得人物的真实比例的身高大小
    /// </summary>
    /// <returns></returns>
    public float GetTrulyRigBodyHeight()
    {
        return ownCollider.transform.localScale.y * ownCollider.height;
    }
    /// <summary>
    /// 获取人物真实比例的身体的中心点
    /// 前提保证人物的pivot的位置 即坐标点 处于人物模型的脚底
    /// </summary>
    /// <returns></returns>
   
    #region Draw

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        DrawSphereBody();
        DrawCheckPoint();
    }
    public void DrawSphereBody()
    {
        if (sphereBodyCasts != null)
        {
            for (int i = 0; i < sphereBodyCasts.Length; i++)
            {
                var pos = CalcuateBodySphereVerticalPos(i);
                Gizmos.color = sphereBodyCasts[i].drawColor;
                Gizmos.DrawWireSphere(pos, sphereBodyCasts[i].radius);
            }
        }
    }
    public void DrawCheckPoint()
    {
        Gizmos.color = Color.green;
        foreach (var checkPointType in bodyPointTrans.Keys)
        {
            
            Gizmos.DrawWireCube(bodyPointTrans[checkPointType].position,Vector3.one*0.35f);
        }
        if (Application.isPlaying)
        {
            var bodymiddleOrigin = Vector3.up * GetComponent<RoleController>().characterCollider.center.y;
            var offsetOrigin = new Vector3(0, 0,-0.2f);
            var boxCenter= transform.TransformPoint(Vector3.forward * checkTargetBoxCastCenterFromActor.z + offsetOrigin+bodymiddleOrigin);
            var halfExents = checkTargetBoxCastCenterFromActor;

            
           Color color= Color.black;
            color.a = 0.2f;

            Gizmos.color = color;
            Gizmos.DrawWireCube(boxCenter,halfExents*2);
        }
    }
#endif
    #endregion

}

