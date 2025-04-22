using UnityEngine;

[CreateAssetMenu(fileName = "ZombieModeSO", menuName = "CreateGameModeSO/CreateZombieModeConfig")]
public class ZombieModeConfig : ScriptableConfig
{
	public int allMemberCount=8;
	public GameObject soliderPrefab;//TODO: 后期这块要随机 
	[Header("主角所在阵营")]
	public UnitCamp playerCamp;
	public int secondsAfterStart=1;
	public void StartGame(RuntimeGame runtime)
	{
		runtime.SpawnTeam_ZombieMode();
		runtime.SpawnPlayer(playerCamp);
		TimeSystem.Instance.AddTimeTask(secondsAfterStart, () => { GameRoot.GameState_ = GameState.GAMEING; MapManager.Instance.PreGameFinishAndStartGame(); }, PETime.PETimeUnit.Seconds);
	}
}