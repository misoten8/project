using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>  
/// ルーム  
/// </summary>  
public class RoomObj : MonoBehaviour
{
	// 部屋名  
	[SerializeField] private Text _name;
	// 人数  
	[SerializeField] private Text _count;


	// Use this for initialization  
	void Start()
	{
		// ボタンにコールバック登録  
		GetComponent<Button>().onClick.AddListener(OnTapped);
	}

	// Update is called once per frame  
	void Update()
	{
		// 部屋一覧からこの部屋の情報を取得  
		var room = PhotonNetwork.GetRoomList().ToList().Find(r => r.Name == _name.text);
		if (room != null)
		{
			// 取得した情報から人数を取得  
			_count.text = room.PlayerCount + "/" + room.MaxPlayers;
		}
		else
		{
			// 一覧にない = 0人、0人の部屋は削除される  
			_count.text = 0 + "/" + 2;
		}

	}

	/// <summary>  
	/// タップ時  
	/// </summary>  
	public void OnTapped()
	{
		// 部屋設定  
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.IsOpen = true;     // 部屋を開くか  
		roomOptions.IsVisible = true;  // 一覧に表示するか  
		roomOptions.MaxPlayers = 2;    // 最大参加人数  
		//PhotonNetwork.JoinRoom("Battle Room");
		// 部屋に参加、存在しない時作成して参加  
		PhotonNetwork.JoinOrCreateRoom(_name.text, roomOptions, new TypedLobby());
	}
}