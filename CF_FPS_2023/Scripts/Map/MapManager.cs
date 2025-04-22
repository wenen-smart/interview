using Assets.Resolution.Scripts.Map;
using Assets.Resolution.Scripts.Region;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Pathfinding;

public class MapManager : MonoSingleTon<MapManager>, I_Init
{
    [HideInInspector] public AIRegion[] regions;
    public RoadEntity ReachOppositeMainRoads;
    public SpawnArea[] TeamASpawn;
    public SpawnArea[] TeamBSpawn;
    public RoadEntity teamAHomeNearly;
    public RoadEntity teamBHomeNearly;
    public Dictionary<UnitCamp, TeamHome> teamHomes=new Dictionary<UnitCamp, TeamHome>();
    private List<SpawnArea> noFillArea_TeamA = new List<SpawnArea>();
    private List<SpawnArea> noFillArea_TeamB = new List<SpawnArea>();
    public float minSpawnPadding = 1;//在每个出生区域里每个实体间的最小的生成间距。
    public int maxIterationCount = 10;
    //public Dictionary<string, EnemyController> enemyDics;
    public Dictionary<string, List<RoleController>> campDics = new Dictionary<string, List<RoleController>>();
    public Dictionary<Transform, RoadPointNode> roadNodeDics = new Dictionary<Transform, RoadPointNode>(); //树中节点。
    [HideInInspector] public RoadPointNode root => ReachOppositeMainRoads.root;
    [HideInInspector]public RoadPointNode last => ReachOppositeMainRoads.last;
    public bool drawNextDir=true;
    public bool drawPriorDir=false;
    public Seeker aiSample;
    public IPathModifier[] pathModifiers;
    //[HideInInspector]public bool isUpdateActiveActors=false;
    public void Init()
    {
        noFillArea_TeamA = TeamASpawn.ToList();
        noFillArea_TeamB = TeamBSpawn.ToList();
        AstarPath.active.logPathResults = PathLog.OnlyErrors;
        regions = FindObjectsOfType<AIRegion>();
        pathModifiers = aiSample.GetComponents<IPathModifier>();
        ReachOppositeMainRoads.ConstructRoadTree();

        teamAHomeNearly.ConstructRoadTree();
        teamBHomeNearly.ConstructRoadTree();
        InitHomeInfo(UnitCamp.A);
        InitHomeInfo(UnitCamp.B);
    }

    public void InitHomeInfo(UnitCamp unitCamp)
    {
        TeamHome teamHome = new TeamHome();
        teamHome.homeEnd = GetMyHome(unitCamp);
        teamHome.homeNearlyPath = GetHomeNearlyPath(unitCamp);
        teamHomes.Add(unitCamp,teamHome);
    }

    

    public RoadEntity GetMainRoadEntity()
    {
        return ReachOppositeMainRoads;
    }

    public RoadPointNode FindNearbyPathPointInMainRoad(RobotController robotController,float limitDis,SearchNearbyMethod searchNearbyMethod, bool randomOrMustNotMin = false, Transform ignoreTransform = null)
    {
        return ReachOppositeMainRoads.FindNearbyPathPoint(robotController,limitDis,searchNearbyMethod,randomOrMustNotMin,ignoreTransform);
    }

    public RoadPointNode FindBestClosetPathPointInMainRoad(RobotController robotController,SearchNearbyMethod searchNearbyMethod,Transform ignoreTransform=null)
    {
        return ReachOppositeMainRoads.FindBestClosetPathPoint(robotController,searchNearbyMethod,ignoreTransform);
    }
    public RoadPointNode FindBestClosetPathPointInMainRoadDir(RobotController robotController,RoadPointNode targetNode,SearchNearbyMethod searchNearbyMethod,Transform ignoreTransform=null)
    {
        return ReachOppositeMainRoads.FindBestClosetPathPointInDir(robotController,targetNode,searchNearbyMethod,ignoreTransform);
    }
    //public void NextPathPoint()
    //{

    //}

    public void FindClosetEnemy()
    {

    }

    
    public void RegisterCamp(string unitCamp, RoleController roleController)
    {
        List<RoleController> list = null;
        if (campDics.ContainsKey(unitCamp))
        {
            campDics.TryGetValue(unitCamp, out list);
        }
        if (list != null)
        {
            if (list.Contains(roleController) == false)
            {
                list.Add(roleController);
            }
        }
        else
        {
            campDics.Add(unitCamp, new List<RoleController>() { roleController });
        }
    }
    public List<RoleController> FindActors(string camp)
    {
        if (campDics == null || campDics.Count == 0)
        {
            return null;
        }
        List<RoleController> list;
        campDics.TryGetValue(camp, out list);
        return list;
    }
    public AIRegion GetOneCanStandAiRegion()
    {
        if (regions==null||regions.Length==0)
        {
            return null;
        }
        //TODO
        int len = regions.Length;
        List<int> canOccupyList = new List<int>();
		for (int i = 0; i < len; i++)
		{
			if (regions[i].IsRemainCanStandPoint())
			{
                canOccupyList.Add(i);
			}
		}
        int index = UnityEngine.Random.Range(0,canOccupyList.Count);
        return regions[canOccupyList[index]];
    }
    public RoleController Spawn(UnitCamp unitCamp,Transform entity,bool isAlignForward=false)
    {
        List<SpawnArea> noFillArea_Team = null;
        if (unitCamp.IsSelectThisEnumInMult(UnitCamp.A))
        {
            noFillArea_Team = noFillArea_TeamA;
        }
        else if (unitCamp.IsSelectThisEnumInMult(UnitCamp.B))
        {
            noFillArea_Team = noFillArea_TeamB;
        }
        int areaIndex = UnityEngine.Random.Range(0, noFillArea_Team.Count);
        SpawnArea spawnArea = noFillArea_Team[areaIndex];
        if (spawnArea.IsCanSpawn())
        {
            GameObject go = entity.gameObject;
            Vector3 point;
            go.SetActive(true);
            if ((GameRoot.GameState_ & GameState.PREGAMEING) != 0)
            {
                if (spawnArea.SpawnInTheArea(out point, minSpawnPadding, maxIterationCount))
                {
                    go.transform.position = point;
                }
                if (spawnArea.IsCanSpawn() == false)
                {
                    noFillArea_Team.RemoveAt(areaIndex);
                }
            }
            else
            {
                point = spawnArea.GetSpawnPointInXZ(0, 0);
                go.transform.position = point;
            }
            if (isAlignForward)
            {
                //go.transform.forward = Vector3.ProjectOnPlane(spawnArea.transform.forward, Vector3.up);
            }
            return go.GetComponent<ActorComponent>().GetActorComponent<RoleController>();
        }
        return null;
    }
    public RoleController SpawnAndInstantiate(UnitCamp unitCamp,GameObject prefab,bool isAlignForward=false)
    {
        GameObject go = GameObjectFactory.Instance.PopItem(prefab);
        return Spawn(unitCamp,go.transform);
    }
    public void PreGameFinishAndStartGame()
    {
        foreach (var item in TeamASpawn)
        {
            item.ClearFillCount();
        }
        foreach (var item in TeamBSpawn)
        {
            item.ClearFillCount();
        }
    }

    public RoadPointNode GetEnemyHome(UnitCamp myUnit)
    {
        if (myUnit==UnitCamp.A)
        {
            return last;
        }
        else 
        {
            return root;
        }
    }
    public RoadPointNode GetMyHome(UnitCamp myUnit)
    {
        if (myUnit == UnitCamp.A)
        {
            return root;
        }
        else
        {
            return last;
        }
    }
    public NextRoadTreeType ToEnemyHomeDir(UnitCamp myUnit)
    {
        if (myUnit == UnitCamp.A)
        {
            return NextRoadTreeType.NextTree;
        }
        else
        {
            return NextRoadTreeType.PriorTree;
        }
    }

    public RoadEntity GetHomeNearlyPath(UnitCamp myUnit)
    {
        if (myUnit == UnitCamp.A)
        {
            return teamAHomeNearly;
        }
        else
        {
            return teamBHomeNearly;
        }
    }
    public bool IsArriveEnemyHome(UnitCamp myUnit,RoadPointNode pointNode)
    {
        if (GetEnemyHome(myUnit)==pointNode)
        {
            return true;
        }
        return false;
    }
    public TeamHome GetTeamHomeInfo(UnitCamp unitCamp)
    {
        return teamHomes[unitCamp];
    }
    public RoleController FilterTheClosestEnemy(RoleController role,UnitCamp camp)
	{
        List<RoleController> enemys =  FindActors(camp.ToString());
		if (enemys!=null&&enemys.Count>0)
		{
			float minDis = Vector3.Distance(enemys[0].transform.position,role.transform.position);
            int len = enemys.Count;
            RoleController target=enemys[0];
			for (int i = 1; i < len; i++)
			{
                float dis = Vector3.Distance(enemys[i].transform.position,role.transform.position);
				if (minDis>dis)
				{
                    minDis = dis;
                    target = enemys[i];
				}
			}
            return target;
		}
        return null;
	}

    public RoleController RandomGetUnitInCamp(UnitCamp camp)
	{
        List<RoleController> enemys =  FindActors(camp.ToString());
        if (enemys==null||enemys.Count==0)
		{
            return null;
		}
        return enemys[UnityEngine.Random.Range(0,enemys.Count)];
	}
	public RoleController FilterTheClosestEnemy(RoleController role, List<RoleController> targets)
	{
		if (targets != null && targets.Count > 0)
		{
			float minDis = Vector3.Distance(targets[0].transform.position, role.transform.position);
			int len = targets.Count;
			RoleController target = targets[0];
			for (int i = 1; i < len; i++)
			{
				float dis = Vector3.Distance(targets[i].transform.position, role.transform.position);
				if (minDis > dis)
				{
					minDis = dis;
					target = targets[i];
				}
			}
			return target;
		}
		return null;
	}
}
public enum SearchNearbyMethod
{
    StraightLine,
    SeekPath,
    BothThink
}

public struct TeamHome
{
    public RoadPointNode homeEnd;
    public RoadEntity homeNearlyPath;
}




