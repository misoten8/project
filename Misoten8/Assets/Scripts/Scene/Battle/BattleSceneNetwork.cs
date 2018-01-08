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
	/// 生成クラス有効化を通知
	/// </summary>
	[PunRPC]
	private void StartupGeneratorBattleScene()
	{
		_battleScene.StartupGenerator();
	}

	/// <summary>
	/// バトル開始を通知
	/// </summary>
	[PunRPC]
	private void BeginGameBattleScene()
	{
		_battleScene.Begin();
	}

	/// <summary>
	/// タイムアップを通知
	/// </summary>
	[PunRPC]
	private void FinishBattleScene()
	{
		_battleScene.Finish();
	}

	/// <summary>
	/// バトル終了処理を通知
	/// </summary>
	[PunRPC]
	public void EndBattleScene(int joinBattlePlayerNum, int[] scoreArray)
	{
		_battleScene.End(joinBattlePlayerNum, scoreArray);
	}

	/// <summary>
	/// バトルシーンの読み込みが終わった事を一定間隔で通知する
	/// </summary>
	/// <remarks>
	/// 一般クライアントが送信する
	/// </remarks>
	[PunRPC]
	private void LoadedBattleSceneSendToMasterClient(byte playerId)
	{
		// マスタークライアントのみが処理を実行する
		if (!PhotonNetwork.isMasterClient)
			return;

		_battleScene.LoadedBattleSceneSendToMasterClient(playerId);
	}

	/// <summary>
	/// 定義のみ
	/// </summary>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
}