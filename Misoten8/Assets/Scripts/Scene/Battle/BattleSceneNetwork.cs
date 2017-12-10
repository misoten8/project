using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// バトルシーン管理クラスで利用する通信処理
/// </summary>
/// <remarks>
/// シーン管理クラスと通信処理は分離させます
/// </remarks>
public class BattleSceneNetwork : Photon.MonoBehaviour
{
	[SerializeField]
	private BattleScene _battleScene;

	/// <summary>
	/// シーン遷移する
	/// </summary>
	[PunRPC]
	public void CallBackSwitch(byte nextScene)
	{
		_battleScene.CallBackSwitch((BattleScene.SceneType)nextScene);
	}

	/// <summary>
	/// 生成クラスをアクティブにする
	/// </summary>
	[PunRPC]
	private void StartupGenerator()
	{
		_battleScene.StartupGenerator();
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }

	public void OnPhotonPlayerPropertiesChanged(object[] i_playerAndUpdatedProps)
	{
		var player = i_playerAndUpdatedProps[0] as PhotonPlayer;
		var properties = i_playerAndUpdatedProps[1] as ExitGames.Client.Photon.Hashtable;

		PhotonNetwork.playerList
			.Where(e => e.ID == player.ID)
			.Select(e =>
			{

				e.SetCustomProperties(properties);
				return default(IEnumerable);
			});
	}
}