using System.Diagnostics;
using FinkFramework.Runtime.Utils;
using UI.DebugUI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Obsoletes;

public class PlayerTeleportDetector : MonoBehaviour
{
	[Header("判定为瞬移的距离")]
	public float teleportDistance = 3f;

	private Vector3 lastPos;

	private bool initialized;

	private void Start()
	{
		lastPos = base.transform.position;
		initialized = true;
	}

	private void LateUpdate()
	{
		if (initialized)
		{
			Vector3 position = base.transform.position;
			float num = Vector3.Distance(position, lastPos);
			if (num > teleportDistance)
			{
				PrintTeleportInfo(lastPos, position, num);
			}
			lastPos = position;
		}
	}

	private void PrintTeleportInfo(Vector3 from, Vector3 to, float dis)
	{
		UILog.W("玩家发生瞬移！");
		LogUtil.Error("===== 玩家发生瞬移 =====\n" + $"距离: {dis}\n" + $"From: {from}\n" + $"To: {to}\n" + $"Frame: {Time.frameCount}\n" + $"Time: {Time.time}\n" + "Parent: " + (base.transform.parent ? base.transform.parent.name : "null") + "\nActive Scene: " + SceneManager.GetActiveScene().name + "\n");
		UnityEngine.Debug.LogError("调用栈:\n" + new StackTrace(fNeedFileInfo: true).ToString());
	}
}
