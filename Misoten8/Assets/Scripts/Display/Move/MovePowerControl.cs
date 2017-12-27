using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 勢力図
/// </summary>
public class MovePowerControl : UIBase
{
    private BattleSceneCache _cache;
    private float _drawValue;

    [SerializeField]
    int No = 1;
    public Slider UIobj;

    public override void OnAwake(ISceneCache cache, IEvents displayEvents)
    {
        base.OnAwake(cache, displayEvents);
        _cache = cache as BattleSceneCache;
    }

    public override bool IsDrawUpdate()
    {
		//if (_drawValue != _cache.powercontrol.GetPlayerPower(No))
		//{
		//    _drawValue = _cache.powercontrol.GetPlayerPower(No);
		//    return true;
		//}
		//return false;
		return true;
    }

    public override void OnDrawUpdate()
    {
        //UIobj.value = _cache.powercontrol.GetPlayerPower(No);
    }
}
