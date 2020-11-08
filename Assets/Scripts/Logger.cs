using UnityEngine;

public static class Logger
{
	public static void Log(string message)
	{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
		Debug.Log(message);
#endif
	}

	public static void LogWarning(string message)
	{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
		Debug.LogWarning(message);
#endif
	}

	public static void LogError(string message)
	{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
		Debug.LogError(message);
#endif
	}
}