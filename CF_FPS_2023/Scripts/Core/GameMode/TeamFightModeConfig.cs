using UnityEngine;

[CreateAssetMenu(fileName = "TeamFightModeSO", menuName = "CreateGameModeSO/CreateTeamFightModeConfig")]
public class TeamFightModeConfig : ScriptableConfig
{
    [Header("ÿ����Ӫ�ĳ�Ա��")]
    public int memberCount;
    [Header("����������Ӫ")]
    public UnitCamp playerCamp;
    public int secondsAfterStart=2;
    public GameObject soliderPrefab;
    public float respawnTime = 2;

    public void StartGame(RuntimeGame runtime)
    {
		runtime.SpawnTeam_TeamFightMode();
        runtime.SpawnPlayer(playerCamp);
        TimeSystem.Instance.AddTimeTask(secondsAfterStart, () => { GameRoot.GameState_ = GameState.GAMEING; MapManager.Instance.PreGameFinishAndStartGame(); },PETime.PETimeUnit.Seconds);
	}
}

