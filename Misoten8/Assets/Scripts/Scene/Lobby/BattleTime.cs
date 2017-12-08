using UnityEngine;

/// <summary>
/// バトルのタイマークラス
/// 値の更新はマスタークライアントのみが行う
/// </summary>
public class BattleTime : MonoBehaviour
{
	public float CurrentTime
	{
		get { return _currentTime; }
	}

	[SerializeField]
	private Battle _battle;

	[SerializeField]
	private float _limitTime;

	[SerializeField]
	private float _currentTime = 0.0f;

	private void Start()
	{
		_currentTime = _limitTime;
	}

	void Update()
	{
		if (!PhotonNetwork.isMasterClient)
			return;

		_currentTime -= Time.deltaTime;

		if (_currentTime > 0.0f)
			return;

		_currentTime = 0.0f;

		_battle.photonView.RPC("TransScene", PhotonTargets.AllViaServer);
	}

	void OnPhotonSerializeView(PhotonStream i_stream, PhotonMessageInfo i_info)
	{
		if (PhotonNetwork.isMasterClient)
		{
			//データの送信
			i_stream.SendNext(_currentTime);
		}
		else
		{
			//データの受信
			_currentTime = (float)i_stream.ReceiveNext();
		}
	}
}
