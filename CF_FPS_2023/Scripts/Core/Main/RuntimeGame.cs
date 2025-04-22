using Assets.Resolution.Scripts.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.U2D.Sprites;
using UnityEngine;


public class RuntimeGame : MonoSingleTon<RuntimeGame>, I_Init
{
    public GameMode gameMode=GameMode.TeamFight;
    
    public GameObject playerEntity;
    public TeamCommunication[] teams;
    public float respawnTime = 2;
    
    public void Init()
    {

    }
    public override void Awake()
    {
        base.Awake();
        StartGame();

        //TODO
        GameRoot.GameState_ = GameState.PREGAMEING;
        MapManager.Instance.Init();
        
        
        switch (gameMode)
        {
            case GameMode.TeamFight:
                GameRoot.Instance.TeamFightModeSO.StartGame(this);
                break;
            case GameMode.Zombie:
                GameRoot.Instance.ZombieModeSO.StartGame(this);
                break;
            default:
                break;
        }
        
    }
    public void SpawnPlayer(UnitCamp playerCamp)
    {
        TeamCommunication team = GetTeam(playerCamp);
        RoleController player = MapManager.Instance.Spawn(playerCamp, playerEntity.transform,true);
        if (player)
        {
			team.AddTeam(player);
			player.team = team;
			player.unit = team.unit;
			player.Init();
        }
    }
    public TeamCommunication GetTeam(UnitCamp unit)
    {
        return teams.First((t)=> { return t.unit == unit; });
    }
    public void SpawnTeam_TeamFightMode()
    {
        var TeamFightModeSO = GameRoot.Instance.TeamFightModeSO;
        int teamCount = TeamFightModeSO.memberCount;
        var soliderPrefab = TeamFightModeSO.soliderPrefab;
        teams = new TeamCommunication[2];
        teams[0] = new TeamCommunication();
        teams[1] = new TeamCommunication();
        teams[0].unit = UnitCamp.A;
        teams[1].unit = UnitCamp.B;
        TeamCommunication teamA = teams[0];
        int teamACount = teamCount;
        int teamBCount = teamCount;
        if (TeamFightModeSO.playerCamp == UnitCamp.A)
        {
            teamACount -= 1;
        }
        else if (TeamFightModeSO.playerCamp == UnitCamp.B)
        {
            teamBCount -= 1;
        }

        for (int i = 0; i < teamACount; i++)
        {
            var team = teamA;
            teams[0] = team;
            team.unit = UnitCamp.A;
            RoleController robot = MapManager.Instance.SpawnAndInstantiate(UnitCamp.A, soliderPrefab);
            if (robot)
            {
				team.AddTeam(robot);
				robot.team = team;
				robot.unit = team.unit;
				robot.Init();
            }
        }
        TeamCommunication teamB = teams[1];
        for (int i = 0; i < teamBCount; i++)
        {
            var team = teamB;
            teams[1] = team;
            team.unit = UnitCamp.B;
            RoleController robot = MapManager.Instance.SpawnAndInstantiate(UnitCamp.B, soliderPrefab);
            if (robot)
            {
				team.AddTeam(robot);
				robot.team = team;
				robot.unit = team.unit;
				robot.Init();
            }
        }
    }
    public void SpawnTeam_ZombieMode()
    {
		var ModeSO = GameRoot.Instance.ZombieModeSO;
        int allMemberCount = ModeSO.allMemberCount-1;//排除 Player
        //僵尸模式默认 开局都是人类阵营。
		teams = new TeamCommunication[2];
		teams[0] = new TeamCommunication();
		teams[1] = new TeamCommunication();
		teams[0].unit = UnitCamp.A;//认为A为人类
		teams[1].unit = UnitCamp.B;//认为B为生化

		//人类阵营初始化
        TeamCommunication teamA = teams[0];
		for (int i = 0; i < allMemberCount; i++)
		{
			//随机在任意高台
			var team = teamA;
			RoleController robot = MapManager.Instance.SpawnAndInstantiate(UnitCamp.A, ModeSO.soliderPrefab);
            
			if (robot)
			{
				team.AddTeam(robot);
				robot.team = team;
				robot.unit = team.unit;
				robot.Init();

				//MapManager.Instance.RegisterCamp(robot.unit.ToString(), robot);
			}
		}
	}
    public void StartGame(bool isContinue)
    {
        UIManager.Instance.PopPanel();
        if (isContinue==false)
        {
            GameLoader.LoadScene(SCENE_NAME.Map);
        }
    }
    public void StartGame()
    {
        SpawnArea[] spawnAreas = MapManager.Instance.TeamBSpawn;
    }
    public List<RoleController> FilterUnits(string[] tagList)
    {
        List<RoleController> roles = new List<RoleController>();
        foreach (var tagStr in tagList)
        {
            List<RoleController> camp = MapManager.Instance.FindActors(tagStr);
            if (camp != null)
            {
                roles.AddRange(camp);
            }
        }
        return roles;
    }
    
    public void ActorDie(RoleController roleController)
    {
        TimeSystem.Instance.AddTimeTask(respawnTime,()=> { MapManager.Instance.Spawn(roleController.unit,roleController.transform);roleController.ReSpawn(); },PETime.PETimeUnit.Seconds);
    }

}

