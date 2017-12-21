using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BeatControllerSection クラス
/// </summary>
public class BeatControllerSections : MonoBehaviour 
{
	/// <summary>
	/// セクションリスト
	/// </summary>
	public List<Section> SectionList
	{
		get { return _sectionsList; }
	}

	[SerializeField]
	private List<Section> _sectionsList = new List<Section>();

	public enum ClipType
	{
		None,
		Loop,
		Through,
		End,
	}

	/// <summary>
	/// セクション単体
	/// </summary>
	[System.Serializable]
	public class Section
	{
		public string Name = "";
		public int unitPerBeat = 4;
		public int unitPerBar = 16;
		public double tempo = 120;
		public int startBar = 0;
		// this will be automatically setted on validate.
		public int startTimeSamples = 0;
		// this will only work when CreateSectionClips == true.
		public ClipType loopType = ClipType.None;
	}

	public override string ToString()
	{
		return string.Format("\"{0}\" StartBar:{1}, Tempo:{2}", _sectionsList[0]?.Name, _sectionsList[0]?.startBar, _sectionsList[0]?.tempo);
	}
}