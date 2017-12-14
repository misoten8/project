/// <summary>
/// バトルシーンのキャッシュデータ
/// </summary>
public class BattleSceneCache : SceneCacheBase, ISceneCache 
{
	public BattleTime battleTime;
	public InfluencePower influencePower;
	public Score score;
	public PlayerManager playerManager;
	public MobManager mobManager;
}