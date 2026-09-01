using System.Collections.Generic;
using UnityEngine;

namespace SK.Framework;

public sealed class AxisInputController
{
	private class AxisInputInfo
	{
		public readonly string axisName;

		public readonly List<AxisInput> axisInputs;

		public float Value;

		public float ValueRaw;

		public AxisInputInfo(string axisName)
		{
			this.axisName = axisName;
			axisInputs = new List<AxisInput>();
		}

		public void Reset()
		{
			Value = 0f;
			ValueRaw = 0f;
		}
	}

	private readonly List<AxisInput> axisInputs;

	private readonly List<AxisInputInfo> infos;

	public float lerpModifier = 20f;

	public AxisInputController()
	{
		axisInputs = new List<AxisInput>();
		infos = new List<AxisInputInfo>();
	}

	public void Update()
	{
		for (int i = 0; i < axisInputs.Count; i++)
		{
			axisInputs[i].Value = Input.GetAxisRaw(axisInputs[i].Key);
		}
		for (int j = 0; j < infos.Count; j++)
		{
			AxisInputInfo axisInputInfo = infos[j];
			axisInputInfo.ValueRaw = 0f;
			for (int k = 0; k < axisInputInfo.axisInputs.Count; k++)
			{
				AxisInput axisInput = axisInputInfo.axisInputs[k];
				if (axisInput.Value != 0f)
				{
					axisInputInfo.ValueRaw = axisInput.Value;
					break;
				}
			}
			axisInputInfo.Value = Mathf.Lerp(axisInputInfo.Value, axisInputInfo.ValueRaw, lerpModifier * Time.deltaTime);
			if (axisInputInfo.ValueRaw == 0f && axisInputInfo.Value != 0f && Mathf.Abs(axisInputInfo.Value) < 0.025f)
			{
				axisInputInfo.Value = 0f;
			}
		}
	}

	public void Reset()
	{
		for (int i = 0; i < axisInputs.Count; i++)
		{
			axisInputs[i].Reset();
		}
	}

	public bool Register(AxisInput axisInput)
	{
		if (axisInputs.Contains(axisInput))
		{
			return false;
		}
		axisInputs.Add(axisInput);
		AxisInputInfo axisInputInfo = infos.Find((AxisInputInfo m) => m.axisName == axisInput.Key);
		if (axisInputInfo == null)
		{
			axisInputInfo = new AxisInputInfo(axisInput.Key);
			infos.Add(axisInputInfo);
		}
		axisInputInfo.axisInputs.Add(axisInput);
		Log.Info("<color=cyan><b>[SKFramework.Input.Info]</b></color> 注册轴[{0}]输入监听", axisInput.Key);
		return true;
	}

	public bool Unregister(AxisInput axisInput)
	{
		if (!axisInputs.Contains(axisInput))
		{
			return false;
		}
		axisInputs.Remove(axisInput);
		AxisInputInfo axisInputInfo = infos.Find((AxisInputInfo m) => m.axisName == axisInput.Key);
		if (axisInputInfo != null)
		{
			axisInputInfo.axisInputs.Remove(axisInput);
			if (axisInputInfo.axisInputs.Count == 0)
			{
				infos.Remove(axisInputInfo);
			}
		}
		Log.Info("<color=cyan><b>[SKFramework.Input.Info]</b></color> 注册轴[{0}]输入监听", axisInput.Key);
		return true;
	}

	public float GetAxis(AxisInput axisInput)
	{
		return infos.Find((AxisInputInfo m) => m.axisName == axisInput.Key)?.Value ?? 0f;
	}

	public float GetAxisRaw(AxisInput axisInput)
	{
		return infos.Find((AxisInputInfo m) => m.axisName == axisInput.Key)?.ValueRaw ?? 0f;
	}
}
