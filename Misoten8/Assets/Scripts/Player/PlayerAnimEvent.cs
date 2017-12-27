using UnityEngine;

/// <summary>
/// PlayerAnimEvent クラス
/// </summary>
public class PlayerAnimEvent : MonoBehaviour 
{
	public Player Player
	{
		set { isMine = value.IsMine; }
	}
	/// <summary>
	/// このプレイヤーがクライアント自身かどうか
	/// </summary>
	bool? isMine = false;

	/// <summary>
	/// 右足接地時実行イベント
	/// </summary>
	public void FootR()
	{
		if(isMine ?? false)
		{
            //足音再生
            AudioManager.PlaySE("足音");
        }
	}

	/// <summary>
	/// 左足接地時実行イベント
	/// </summary>
	public void FootL()
	{
		if (isMine ?? false)
		{
            //足音再生
            AudioManager.PlaySE("足音");
        }
	}
}