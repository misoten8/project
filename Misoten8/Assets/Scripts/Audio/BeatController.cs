using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BeatController クラス
/// </summary>
public class BeatController : MonoBehaviour
{
	public BeatControllerSections.Section this[int index]
	{
		get
		{
			if (0 <= index && index < SectionCount_)
			{
				return Sections.SectionList[index];
			}
			else
			{
				Debug.LogWarning("Section index out of range! index = " + index + ", SectionCount = " + SectionCount_);
				return null;
			}
		}
	}

	#region editor params
	public BeatControllerSections Sections;

	/// <summary>
	/// Just for music that doesn't start with the first timesample.
	/// if so, specify time samples before music goes into first timing = (0,0,0).
	/// </summary>
	public int EntryPointSample = 0;

	/// <summary>
	/// put your debug GUIText to see current musical time & section info.
	/// </summary>
	public TextMesh DebugText;

	public bool CreateSectionClips;
	#endregion

	#region public properties
	public bool IsPlaying
	{
		get { return IsPlaying_; }
	}
	/// <summary>
	/// last timing.
	/// </summary>
	public Timing Just
	{
		get { return just_; }
	}
	/// <summary>
	/// nearest timing.
	/// </summary>
	public Timing Near
	{
		get { return near_; }
	}
	/// <summary>
	/// is Just changed in this frame or not.
	/// </summary>
	public bool IsJustChanged
	{
		get { return isJustChanged_; }
	}
	/// <summary>
	/// is Near changed in this frame or not.
	/// </summary>
	public bool IsNearChanged
	{
		get { return isNearChanged_; }
	}
	/// <summary>
	/// is currently former half in a MusicTimeUnit, or last half.
	/// </summary>
	public bool IsFormerHalf
	{
		get { return isFormerHalf_; }
	}
	/// <summary>
	/// delta time from JustChanged.
	/// </summary>
	public double TimeSecFromJust
	{
		get { return timeSecFromJust_; }
	}
	/// <summary>
	/// how many times you repeat current music/block.
	/// </summary>
	public int NumRepeat
	{
		get { return numRepeat_; }
	}
	/// <summary>
	/// returns how long from nearest Just timing with sign.
	/// </summary>
	public double Lag
	{
		get { return Lag_; }
	}
	/// <summary>
	/// returns how long from nearest Just timing absolutely.
	/// </summary>
	public double LagAbs
	{
		get { return LagAbs_; }
	}
	/// <summary>
	/// returns normalized lag.
	/// </summary>
	public double LagUnit
	{
		get { return LagUnit_; }
	}
	/// <summary>
	/// sec / musicalUnit
	/// </summary>
	public double MusicTimeUnit
	{
		get { return musicTimeUnit_; }
	}
	/// <summary>
	/// current musical time based on MusicalTimeUnit
	/// **warning** if CurrentSection.UnitPerBar changed, this is not continuous.
	/// </summary>
	public float MusicalTime
	{
		get { return MusicalTime_; }
	}
	/// <summary>
	/// current musical time based on MusicalBar.
	/// This is always continuous(MusicalTime is not).
	/// </summary>
	public float MusicalTimeBar
	{
		get { return MusicalTimeBar_; }
	}
	/// <summary>
	/// dif from timing to Just on musical time unit.
	/// </summary>
	/// <param name="timing"></param>
	/// <returns></returns>
	public float MusicalTimeFrom(Timing timing)
	{
		int index = 0;
		for (int i = 0; i < SectionCount; ++i)
		{
			if (i + 1 < SectionCount)
			{
				if (timing.Bar < this[i + 1].startBar)
				{
					index = i;
					break;
				}
			}
			else
			{
				index = i;
			}
		}
		int startIndex = Mathf.Min(index, sectionIndex_);
		int endIndex = Mathf.Max(index, sectionIndex_);
		Timing currentTiming = new Timing(this, timing < Just ? timing : Just);
		Timing endTiming = (timing > Just ? timing : Just);
		int musicalTime = 0;
		for (int i = startIndex; i <= endIndex; ++i)
		{
			if (i < endIndex)
			{
				musicalTime += this[i + 1].startBar * this[i].unitPerBar - currentTiming.GetMusicalTime(this.Sections.SectionList[i]);
				currentTiming.Set(this[i + 1].startBar);
			}
			else
			{
				musicalTime += endTiming.GetMusicalTime(this[i]) - currentTiming.GetMusicalTime(this[i]);
			}
		}
		return (float)((timing > Just ? -1 : 1) * musicalTime + TimeSecFromJust / MusicTimeUnit);
	}
	/// <summary>
	/// current audio play time in sec.
	/// </summary>
	public float AudioTimeSec
	{
		get { return AudioTimeSec_; }
	}
	/// <summary>
	/// current audio play sample
	/// </summary>
	public int TimeSamples
	{
		get { return TimeSamples_; }
	}
	/// <summary>
	/// returns musically synced cos wave.
	/// if default( MusicalCos(16,0,0,1),
	/// starts from max=1,
	/// reaches min=0 on MusicalTime = cycle/2 = 8,
	/// back to max=1 on MusicalTIme = cycle = 16.
	/// </summary>
	/// <param name="cycle">wave cycle in musical unit</param>
	/// <param name="offset">wave offset in musical unit</param>
	/// <param name="min"></param>
	/// <param name="max"></param>
	/// <returns></returns>
	public float MusicalCos(float cycle = 16, float offset = 0, float min = 0, float max = 1)
	{
		return Mathf.Lerp(min, max, ((float)Math.Cos(Math.PI * 2 * (MusicalTime + offset) / cycle) + 1.0f) / 2.0f);
	}

	public int UnitPerBar { get { return UnitPerBar_; } }
	public int UnitPerBeat { get { return UnitPerBeat_; } }
	public AudioSource CurrentSource { get { return musicSource_; } }
	public BeatControllerSections.Section CurrentSection { get { return CurrentSection_; } }
	public int CurrentSectionIndex { get { return sectionIndex_; } }
	public int SectionCount { get { return SectionCount_; } }
	public string CurrentMusicName { get { return name; } }
	public BeatControllerSections.Section GetSection(int index)
	{
		return this[index];
	}
	public BeatControllerSections.Section GetSection(string sectionName)
	{
		return Sections.SectionList.Find((BeatControllerSections.Section s) => s.Name == sectionName);
	}
	/// <summary>
	/// this will only work when CreateSectionClips == true.
	/// </summary>
	public bool IsTransitioning { get { return isTransitioning_; } }
	#endregion

	#region public predicates
	public bool IsJustChangedWhen(System.Predicate<Timing> pred)
	{
		return IsJustChangedWhen_(pred);
	}
	public bool IsJustChangedBar()
	{
		return IsJustChangedBar_();
	}
	public bool IsJustChangedBeat()
	{
		return IsJustChangedBeat_();
	}
	public bool IsJustChangedAt(int bar = 0, int beat = 0, int unit = 0)
	{
		return IsJustChangedAt_(bar, beat, unit);
	}
	public bool IsJustChangedAt(Timing t)
	{
		return IsJustChangedAt_(t.Bar, t.Beat, t.Unit);
	}
	public bool IsJustChangedSection(string sectionName = "")
	{
		BeatControllerSections.Section targetSection = (sectionName == "" ? CurrentSection : GetSection(sectionName));
		if (targetSection != null)
		{
			return IsJustChangedAt(targetSection.startBar);
		}
		else
		{
			Debug.LogWarning("Can't find section name: " + sectionName);
			return false;
		}
	}

	public bool IsNearChangedWhen(System.Predicate<Timing> pred)
	{
		return IsNearChangedWhen_(pred);
	}
	public bool IsNearChangedBar()
	{
		return IsNearChangedBar_();
	}
	public bool IsNearChangedBeat()
	{
		return IsNearChangedBeat_();
	}
	public bool IsNearChangedAt(int bar, int beat = 0, int unit = 0)
	{
		return IsNearChangedAt_(bar, beat, unit);
	}
	public bool IsNearChangedAt(Timing t)
	{
		return IsNearChangedAt_(t.Bar, t.Beat, t.Unit);
	}
	#endregion

	#region public functions
	/// <summary>
	/// Quantize to musical time.
	/// </summary>
	public void QuantizePlay(AudioSource source, int transpose = 0, float allowRange = 0.3f)
	{
		source.pitch = Mathf.Pow(PITCH_UNIT, transpose);
		if (IsFormerHalf && LagUnit < allowRange)
		{
			source.Play();
		}
		else
		{
			quantizedCue_.Add(source);
		}
	}
	public void Pause() { musicSource_.Pause(); }
	public void Resume() { musicSource_.Play(); }
	public void Stop()
	{
		musicSource_.Stop();
		if (isTransitioning_)
		{
			foreach (AudioSource source in sectionSources_)
			{
				source.Stop();
			}
			isTransitioning_ = false;
		}
	}
	public void Seek(Timing timing)
	{
		BeatControllerSections.Section section = null;
		for (int i = 0; i < SectionCount; ++i)
		{
			if (i + 1 < SectionCount)
			{
				if (timing.Bar < this[i + 1].startBar)
				{
					section = this[i];
				}
			}
			else
			{
				section = this[i];
			}
		}
		int deltaMT = (timing.GetMusicalTime(section) - section.startBar * section.unitPerBar);
		musicSource_.timeSamples = section.startTimeSamples + (int)(deltaMT * MusicTimeUnit * samplingRate_);
	}
	public void SeekToSection(string sectionName)
	{
		SeekToSection_(sectionName);
	}
	public void SetVolume(float volume)
	{
		musicSource_.volume = volume;
		if (CreateSectionClips)
		{
			foreach (AudioSource source in sectionSources_)
			{
				source.volume = volume;
			}
		}
	}

	public enum SyncType
	{
		NextBeat,
		Next2Beat,
		NextBar,
		Next2Bar,
		Next4Bar,
		Next8Bar,
		SectionEnd,
	}

	public void SetNextSection(int sectionIndex, SyncType syncType = SyncType.NextBar)
	{
		SetNextSection_(sectionIndex, syncType);
	}
	public void SetNextSection(string name, SyncType syncType = SyncType.NextBar)
	{
		SetNextSection(Sections.SectionList.FindIndex((BeatControllerSections.Section s) => s.Name == name), syncType);
	}
	#endregion

	#region private params
	private Timing just_;
	private Timing near_;
	private bool isJustChanged_;
	private bool isNearChanged_;
	private bool isFormerHalf_;
	private double timeSecFromJust_;
	private int numRepeat_;
	private double musicTimeUnit_;

	[SerializeField]
	private AudioSource musicSource_;
	private int sectionIndex_;
	private int currentSample_;

	private int samplingRate_;
	private int samplesPerUnit_;
	private int samplesPerBeat_;
	private int samplesPerBar_;
	private int samplesInLoop_;

	private Timing oldNear_, oldJust_;
	private int numLoopBar_ = -1;
	private List<AudioSource> quantizedCue_ = new List<AudioSource>();
	private readonly float PITCH_UNIT = Mathf.Pow(2.0f, 1.0f / 12.0f);

	private List<AudioSource> sectionSources_ = new List<AudioSource>();
	private bool isTransitioning_ = false;
	private Timing transitionTiming_ = null;
	#endregion

	#region private properties
	private double Lag_
	{
		get
		{
			if (isFormerHalf_)
				return timeSecFromJust_;
			else
				return timeSecFromJust_ - musicTimeUnit_;
		}
	}
	private double LagAbs_
	{
		get
		{
			if (isFormerHalf_)
				return timeSecFromJust_;
			else
				return musicTimeUnit_ - timeSecFromJust_;
		}
	}
	private double LagUnit_ { get { return Lag / musicTimeUnit_; } }
	private float MusicalTime_ { get { return (float)(just_.GetMusicalTime(CurrentSection_) + timeSecFromJust_ / musicTimeUnit_); } }
	private float MusicalTimeBar_ { get { return MusicalTime_ / CurrentSection_.unitPerBar; } }
	private float AudioTimeSec_ { get { return musicSource_.time; } }
	private int TimeSamples_ { get { return musicSource_.timeSamples; } }
	private int SectionCount_ { get { return Sections.SectionList.Count; } }
	private int UnitPerBar_ { get { return CurrentSection.unitPerBar; } }
	private int UnitPerBeat_ { get { return CurrentSection.unitPerBeat; } }
	private bool IsPlaying_ { get { return (musicSource_ != null && musicSource_.isPlaying); } }
	private BeatControllerSections.Section CurrentSection_ { get { return Sections.SectionList[sectionIndex_]; } }
	#endregion

	#region private predicates
	private bool IsNearChangedWhen_(System.Predicate<Timing> pred)
	{
		return isNearChanged_ && pred(near_);
	}
	private bool IsNearChangedBar_()
	{
		return isNearChanged_ && near_.Beat == 0 && near_.Unit == 0;
	}
	private bool IsNearChangedBeat_()
	{
		return isNearChanged_ && near_.Unit == 0;
	}
	private bool IsNearChangedAt_(int bar, int beat = 0, int unit = 0)
	{
		return isNearChanged_ &&
			near_.Bar == bar && near_.Beat == beat && near_.Unit == unit;
	}
	private bool IsJustChangedWhen_(System.Predicate<Timing> pred)
	{
		return isJustChanged_ && pred(just_);
	}
	private bool IsJustChangedBar_()
	{
		return isJustChanged_ && just_.Beat == 0 && just_.Unit == 0;
	}
	private bool IsJustChangedBeat_()
	{
		return isJustChanged_ && just_.Unit == 0;
	}
	private bool IsJustChangedAt_(int bar = 0, int beat = 0, int unit = 0)
	{
		return isJustChanged_ &&
			just_.Bar == bar && just_.Beat == beat && just_.Unit == unit;
	}
	#endregion

	#region private functions
	void Awake()
	{
		samplingRate_ = musicSource_.clip.frequency;
		if (musicSource_.loop)
		{
			samplesInLoop_ = musicSource_.clip.samples;
			BeatControllerSections.Section lastSection = Sections.SectionList[Sections.SectionList.Count - 1];
			double beatSec = (60.0 / lastSection.tempo);
			int samplesPerBar = (int)(samplingRate_ * lastSection.unitPerBar * (beatSec / lastSection.unitPerBeat));
			numLoopBar_ = lastSection.startBar + Mathf.RoundToInt((float)(samplesInLoop_ - lastSection.startTimeSamples) / (float)samplesPerBar);
		}

		if (CreateSectionClips)
		{
			int listCount = Sections.SectionList.Count;
			AudioClip[] clips = new AudioClip[listCount];
			int previousSectionSample = 0;
			for (int i = 0; i < listCount; ++i)
			{
				int nextSectionSample = (i + 1 < listCount ? Sections.SectionList[i + 1].startTimeSamples : musicSource_.clip.samples);
				clips[i] = AudioClip.Create(Sections.SectionList[i].Name + "_clip", nextSectionSample - previousSectionSample, musicSource_.clip.channels, musicSource_.clip.frequency, false);
				previousSectionSample = nextSectionSample;
				float[] waveData = new float[clips[i].samples * clips[i].channels];
				musicSource_.clip.GetData(waveData, Sections.SectionList[i].startTimeSamples);
				clips[i].SetData(waveData, 0);
				AudioSource sectionSource = new GameObject("section_" + Sections.SectionList[i].Name, typeof(AudioSource)).GetComponent<AudioSource>();
				sectionSource.transform.parent = this.transform;
				sectionSource.clip = clips[i];
				sectionSource.loop = Sections.SectionList[i].loopType == BeatControllerSections.ClipType.Loop;
				sectionSource.outputAudioMixerGroup = musicSource_.outputAudioMixerGroup;
				sectionSource.volume = musicSource_.volume;
				sectionSource.pitch = musicSource_.pitch;
				sectionSource.playOnAwake = false;
				sectionSources_.Add(sectionSource);
			}
			musicSource_.Stop();
			musicSource_.enabled = false;
			musicSource_ = sectionSources_[0];
		}

		Initialize();

		OnSectionChanged();
	}

	void Update()
	{
		if (IsPlaying_ || (CreateSectionClips && CheckClipChange()))
		{
			UpdateTiming();
		}
	}

	void Initialize()
	{
		isJustChanged_ = false;
		isNearChanged_ = false;
		near_ = new Timing(this, 0, 0, -1);
		just_ = new Timing(this, near_);
		oldNear_ = new Timing(this, near_);
		oldJust_ = new Timing(this, just_);
		timeSecFromJust_ = 0;
		isFormerHalf_ = true;
		numRepeat_ = 0;
		sectionIndex_ = 0;
	}

	void OnSectionChanged()
	{
		if (Sections == null || Sections.SectionList.Count == 0)
			return;
		if (CurrentSection_.tempo > 0.0f)
		{
			double beatSec = (60.0 / CurrentSection_.tempo);
			samplesPerUnit_ = (int)(samplingRate_ * (beatSec / CurrentSection_.unitPerBeat));
			samplesPerBeat_ = (int)(samplingRate_ * beatSec);
			samplesPerBar_ = (int)(samplingRate_ * CurrentSection_.unitPerBar * (beatSec / CurrentSection_.unitPerBeat));
			musicTimeUnit_ = (double)samplesPerUnit_ / (double)samplingRate_;
			if (CreateSectionClips)
			{
				samplesInLoop_ = musicSource_.clip.samples;
				numLoopBar_ = Mathf.RoundToInt(samplesInLoop_ / (float)samplesPerBar_);
				if (CurrentSection_.loopType == BeatControllerSections.ClipType.Through)
				{
					SetNextSection_(sectionIndex_ + 1, SyncType.SectionEnd);
				}
			}
		}
		else
		{
			samplesPerUnit_ = 0;
			samplesPerBeat_ = 0;
			samplesPerBar_ = 0;
			musicTimeUnit_ = 0;
		}
	}

	public void PlayStart(string sectionName = "")
	{
		if (IsPlaying)
		{
			Stop();
		}

		Initialize();
		if (sectionName != "")
		{
			if (CreateSectionClips)
			{
				int index = Sections.SectionList.FindIndex((BeatControllerSections.Section s) => s.Name == sectionName);
				if (index >= 0)
				{
					musicSource_ = sectionSources_[index];
					sectionIndex_ = index;
					OnSectionChanged();
				}
				else
				{
					Debug.LogWarning("Can't find section name: " + sectionName);
				}
			}
			else
			{
				SeekToSection_(sectionName);
			}
		}
		musicSource_.Play();
	}

	void SeekToSection_(string sectionName)
	{
		BeatControllerSections.Section targetSection = GetSection(sectionName);
		if (targetSection != null)
		{
			musicSource_.timeSamples = targetSection.startTimeSamples;
			sectionIndex_ = Sections.SectionList.IndexOf(targetSection);
			OnSectionChanged();
		}
		else
		{
			Debug.LogWarning("Can't find section name: " + sectionName);
		}
	}

	void SetNextSection_(int sectionIndex, SyncType syncType = SyncType.NextBar)
	{
		if (CreateSectionClips == false || isTransitioning_)
			return;

		if (sectionIndex < 0 || SectionCount <= sectionIndex || sectionIndex == sectionIndex_)
			return;

		int syncUnit = 0;

		if(transitionTiming_ == null)
		{
			transitionTiming_ = new Timing(this, 0);
		}
		transitionTiming_.Copy(just_);
		switch (syncType)
		{
			case SyncType.NextBeat:
				syncUnit = samplesPerBeat_;
				transitionTiming_.Beat += 1;
				transitionTiming_.Unit = 0;
				break;
			case SyncType.Next2Beat:
				syncUnit = samplesPerBeat_ * 2;
				transitionTiming_.Beat += 2;
				transitionTiming_.Unit = 0;
				break;
			case SyncType.NextBar:
				syncUnit = samplesPerBar_;
				transitionTiming_.Bar += 1;
				transitionTiming_.Beat = transitionTiming_.Unit = 0;
				break;
			case SyncType.Next2Bar:
				syncUnit = samplesPerBar_ * 2;
				transitionTiming_.Bar += 2;
				transitionTiming_.Beat = transitionTiming_.Unit = 0;
				break;
			case SyncType.Next4Bar:
				syncUnit = samplesPerBar_ * 4;
				transitionTiming_.Bar += 4;
				transitionTiming_.Beat = transitionTiming_.Unit = 0;
				break;
			case SyncType.Next8Bar:
				syncUnit = samplesPerBar_ * 8;
				transitionTiming_.Bar += 8;
				transitionTiming_.Beat = transitionTiming_.Unit = 0;
				break;
			case SyncType.SectionEnd:
				syncUnit = samplesInLoop_;
				transitionTiming_.Bar = CurrentSection_.startBar + numLoopBar_;
				transitionTiming_.Beat = transitionTiming_.Unit = 0;
				break;
		}
		transitionTiming_.Fix(CurrentSection_);
		if (CurrentSection_.loopType == BeatControllerSections.ClipType.Loop && transitionTiming_.Bar >= CurrentSection_.startBar + numLoopBar_)
		{
			transitionTiming_.Bar -= numLoopBar_;
		}

		if (syncUnit <= 0)
			return;

		double transitionTime = AudioSettings.dspTime + (syncUnit - musicSource_.timeSamples % syncUnit) / (double)samplingRate_ / musicSource_.pitch;
		sectionSources_[sectionIndex].PlayScheduled(transitionTime);
		sectionSources_[sectionIndex_].SetScheduledEndTime(transitionTime);
		isTransitioning_ = true;
	}

	bool CheckClipChange()
	{
		if (musicSource_.isPlaying == false)
		{
			foreach (AudioSource source in sectionSources_)
			{
				if (source.isPlaying)
				{
					musicSource_ = source;
					isTransitioning_ = false;
					return true;
				}
			}
		}
		return false;
	}

	void UpdateTiming()
	{
		// find section index
		int newIndex = sectionIndex_;
		if (CreateSectionClips)
		{
			newIndex = sectionSources_.IndexOf(musicSource_);
			currentSample_ = musicSource_.timeSamples;
		}
		else
		{
			int oldSample = currentSample_;
			currentSample_ = musicSource_.timeSamples;
			if (sectionIndex_ + 1 >= Sections.SectionList.Count)
			{
				if (currentSample_ < oldSample)
				{
					newIndex = 0;
				}
			}
			else
			{
				if (Sections.SectionList[sectionIndex_ + 1].startTimeSamples <= currentSample_)
				{
					newIndex = sectionIndex_ + 1;
				}
			}
		}

		if (newIndex != sectionIndex_)
		{
			sectionIndex_ = newIndex;
			OnSectionChanged();
		}

		// calc current timing
		isNearChanged_ = false;
		isJustChanged_ = false;
		int sectionSample = currentSample_ - (CreateSectionClips ? 0 : CurrentSection_.startTimeSamples);
		if (sectionSample >= 0)
		{
			just_.Bar = (int)(sectionSample / samplesPerBar_) + CurrentSection_.startBar;
			just_.Beat = (int)((sectionSample % samplesPerBar_) / samplesPerBeat_);
			just_.Unit = (int)(((sectionSample % samplesPerBar_) % samplesPerBeat_) / samplesPerUnit_);
			just_.Fix(CurrentSection_);
			if (CreateSectionClips)
			{
				if (CurrentSection_.loopType == BeatControllerSections.ClipType.Loop && numLoopBar_ > 0)
				{
					just_.Bar -= CurrentSection_.startBar;
					while (just_.Bar >= numLoopBar_)
					{
						just_.Decrement(CurrentSection_);
					}
					just_.Bar += CurrentSection_.startBar;
				}

				if (isTransitioning_ && just_.Equals(transitionTiming_))
				{
					if (CurrentSection_.loopType == BeatControllerSections.ClipType.Loop && just_.Bar == CurrentSection_.startBar)
						just_.Bar = CurrentSection_.startBar + numLoopBar_;
					just_.Decrement(CurrentSection_);
				}
			}
			else
			{
				if (sectionIndex_ + 1 >= Sections.SectionList.Count)
				{
					if (numLoopBar_ > 0)
					{
						while (just_.Bar >= numLoopBar_)
						{
							just_.Decrement(CurrentSection_);
						}
					}
				}
				else
				{
					while (just_.Bar >= Sections.SectionList[sectionIndex_ + 1].startBar)
					{
						just_.Decrement(CurrentSection_);
					}
				}
			}

			just_.Bar -= CurrentSection_.startBar;
			timeSecFromJust_ = (double)(sectionSample - just_.Bar * samplesPerBar_ - just_.Beat * samplesPerBeat_ - just_.Unit * samplesPerUnit_) / (double)samplingRate_;
			isFormerHalf_ = (timeSecFromJust_ * samplingRate_) < samplesPerUnit_ / 2;
			just_.Bar += CurrentSection_.startBar;

			near_.Copy(just_);
			if (!isFormerHalf_)
				near_.Increment(CurrentSection_);
			if (samplesInLoop_ != 0 && currentSample_ + samplesPerUnit_ / 2 >= samplesInLoop_)
			{
				near_.Init();
			}

			isNearChanged_ = (near_.Equals(oldNear_) == false);
			isJustChanged_ = (just_.Equals(oldJust_) == false);

			CallEvents();

			oldNear_.Copy(near_);
			oldJust_.Copy(just_);
		}

		if (DebugText != null)
		{
			DebugText.text = "Just = " + Just.ToString() + ", MusicalTime = " + MusicalTime_;
			if (Sections.SectionList.Count > 0)
			{
				DebugText.text += System.Environment.NewLine + "section[" + sectionIndex_ + "] = " + CurrentSection_.ToString();
			}
		}
	}

	void CallEvents()
	{
		if (isJustChanged_)
			OnJustChanged();
		if (isJustChanged_ && just_.Unit == 0)
			OnBeat();
		if (isJustChanged_ && just_.Unit == 0 && just_.Beat == 0)
			OnBar();
		if (isJustChanged_ && oldJust_ > just_)
		{
			OnRepeated();
		}
	}

	//On events (when isJustChanged)
	void OnJustChanged()
	{
		foreach (AudioSource cue in quantizedCue_)
		{
			cue.Play();
		}
		quantizedCue_.Clear();
	}

	void OnBeat()
	{
	}

	void OnBar()
	{
	}

	void OnRepeated()
	{
		++numRepeat_;
	}
	#endregion
}

[Serializable]
public class Timing : IComparable<Timing>, IEquatable<Timing>
{
	public Timing(BeatController beatController, int bar = 0, int beat = 0, int unit = 0)
	{
		_beatController = beatController;
		Bar = bar;
		Beat = beat;
		Unit = unit;
	}

	public Timing(BeatController beatController, Timing copy)
	{
		_beatController = beatController;
		Copy(copy);
	}

	public Timing(BeatController beatController)
	{
		_beatController = beatController;
		Init();
	}

	public void Init()
	{
		Bar = 0;
		Beat = 0;
		Unit = 0;
	}

	public void Copy(Timing copy)
	{
		Bar = copy.Bar;
		Beat = copy.Beat;
		Unit = copy.Unit;
	}

	public void Set(int bar, int beat = 0, int unit = 0)
	{
		Bar = bar;
		Beat = beat;
		Unit = unit;
	}

	public int Bar, Beat, Unit;

	//TODO:後で修正する
	//public int CurrentMusicalTime
	//{
	//	get { return GetMusicalTime(_beatController.CurrentSection.Sections); }
	//}
	public int GetMusicalTime(BeatControllerSections.Section section)
	{
		return Bar * section.unitPerBar + Beat * section.unitPerBeat + Unit;
	}
	public void Fix(BeatControllerSections.Section section)
	{
		int totalUnit = Bar * section.unitPerBar + Beat * section.unitPerBeat + Unit;
		Bar = totalUnit / section.unitPerBar;
		Beat = (totalUnit - Bar * section.unitPerBar) / section.unitPerBeat;
		Unit = (totalUnit - Bar * section.unitPerBar - Beat * section.unitPerBeat);
	}
	public void Increment(BeatControllerSections.Section section)
	{
		Unit++;
		Fix(section);
	}
	public void Decrement(BeatControllerSections.Section section)
	{
		Unit--;
		Fix(section);
	}
	public void IncrementBeat(BeatControllerSections.Section section)
	{
		Beat++;
		Fix(section);
	}
	public void Add(Timing t, BeatControllerSections.Section section)
	{
		Bar += t.Bar;
		Beat += t.Beat;
		Unit += t.Unit;
		Fix(section);
	}
	public void Subtract(Timing t, BeatControllerSections.Section section)
	{
		Bar -= t.Bar;
		Beat -= t.Beat;
		Unit -= t.Unit;
		Fix(section);
	}

	public static bool operator >(Timing t, Timing t2) { return t.Bar > t2.Bar || (t.Bar == t2.Bar && t.Beat > t2.Beat) || (t.Bar == t2.Bar && t.Beat == t2.Beat && t.Unit > t2.Unit); }
	public static bool operator <(Timing t, Timing t2) { return !(t > t2) && !(t.Equals(t2)); }
	public static bool operator <=(Timing t, Timing t2) { return !(t > t2); }
	public static bool operator >=(Timing t, Timing t2) { return t > t2 || t.Equals(t2); }

	public override bool Equals(object obj)
	{
		if (object.ReferenceEquals(obj, null))
		{
			return false;
		}
		if (object.ReferenceEquals(obj, this))
		{
			return true;
		}
		if (this.GetType() != obj.GetType())
		{
			return false;
		}
		return this.Equals(obj as Timing);
	}

	public bool Equals(Timing other)
	{
		return (this.Bar == other.Bar && this.Beat == other.Beat && this.Unit == other.Unit);
	}

	public int CompareTo(Timing tother)
	{
		if (this.Equals(tother))
			return 0;
		else if (this > tother)
			return 1;
		else
			return -1;
	}

	public override string ToString()
	{
		return Bar + " " + Beat + " " + Unit;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	private BeatController _beatController = null;
}