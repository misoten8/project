using UnityEngine;
using System.Diagnostics;

public class ScriptDebug : MonoBehaviour
{
	static public void Log(string msg)
	{
#if UNITY_EDITOR
		StackFrame sf = new StackFrame(1, true);
		UnityEngine.Debug.Log(msg + "\n関数名:" + sf.GetMethod().ToString() + "\nファイル名:" + sf.GetFileName() + "\n行番号:" + sf.GetFileLineNumber());
#endif
	}

	static public void LogErrorEx(string msg)
	{
#if UNITY_EDITOR
		StackFrame sf = new StackFrame(1, true);
		UnityEngine.Debug.LogError(msg + "\n関数名:" + sf.GetMethod().ToString() + "\nファイル名:" + sf.GetFileName() + "\n行番号:" + sf.GetFileLineNumber());
#endif
	}
}
