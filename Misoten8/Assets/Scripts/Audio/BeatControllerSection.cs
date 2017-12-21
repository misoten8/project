using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BeatControllerSection クラス
/// </summary>
public class BeatControllerSections : MonoBehaviour 
{
	public enum ClipType
	{
		None,
		Loop,
		Through,
		End,
	}
	public string Name;
	public int UnitPerBeat;
	public int UnitPerBar;
	public double Tempo;
	public int StartBar;
	// this will be automatically setted on validate.
	public int StartTimeSamples;
	// this will only work when CreateSectionClips == true.
	public ClipType LoopType;

	public void OnAwake(int startBar, int mtBeat = 4, int mtBar = 16, double tempo = 120)
	{
		StartBar = startBar;
		UnitPerBeat = mtBeat;
		UnitPerBar = mtBar;
		Tempo = tempo;
	}

	public override string ToString()
	{
		return string.Format("\"{0}\" StartBar:{1}, Tempo:{2}", Name, StartBar, Tempo);
	}
}