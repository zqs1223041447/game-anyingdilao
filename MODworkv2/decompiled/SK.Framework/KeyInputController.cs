using System;
using System.Collections.Generic;
using UnityEngine;

namespace SK.Framework;

public sealed class KeyInputController
{
	private class KeyInputInfo
	{
		public readonly KeyCode keyCode;

		public readonly List<KeyInput> keyInputs;

		public InputState state;

		public KeyInputInfo(KeyCode keyCode)
		{
			this.keyCode = keyCode;
			keyInputs = new List<KeyInput>();
		}

		public void Reset()
		{
			state = InputState.None;
		}
	}

	private readonly List<KeyInput> keyInputs;

	private readonly List<KeyInputInfo> infos;

	private readonly Dictionary<KeyCode, InputTrigger> triggers;

	public KeyInputController()
	{
		keyInputs = new List<KeyInput>();
		infos = new List<KeyInputInfo>();
		triggers = new Dictionary<KeyCode, InputTrigger>();
	}

	public void Update()
	{
		for (int i = 0; i < keyInputs.Count; i++)
		{
			keyInputs[i].Value = Input.GetKey(keyInputs[i].Key);
		}
		for (int j = 0; j < infos.Count; j++)
		{
			KeyInputInfo keyInputInfo = infos[j];
			bool flag = false;
			for (int num = keyInputInfo.keyInputs.Count - 1; num >= 0; num--)
			{
				if (keyInputInfo.keyInputs[num].Value)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				keyInputInfo.state = ((keyInputInfo.state == InputState.None || keyInputInfo.state == InputState.Released) ? InputState.Pressed : InputState.Held);
				triggers.Clear();
			}
			else if (triggers.ContainsKey(keyInputInfo.keyCode))
			{
				switch (triggers[keyInputInfo.keyCode].type)
				{
				case InputTriggerType.Pressed:
					keyInputInfo.state = InputState.Pressed;
					triggers.Remove(keyInputInfo.keyCode);
					break;
				case InputTriggerType.Held:
					keyInputInfo.state = InputState.Held;
					if (triggers[keyInputInfo.keyCode].disposeWhen())
					{
						triggers.Remove(keyInputInfo.keyCode);
					}
					break;
				case InputTriggerType.Released:
					keyInputInfo.state = InputState.Released;
					triggers.Remove(keyInputInfo.keyCode);
					break;
				}
			}
			else
			{
				keyInputInfo.state = ((keyInputInfo.state == InputState.Pressed || keyInputInfo.state == InputState.Held) ? InputState.Released : InputState.None);
			}
		}
	}

	public void Reset()
	{
		for (int i = 0; i < keyInputs.Count; i++)
		{
			keyInputs[i].Reset();
		}
	}

	public bool Register(KeyInput keyInput)
	{
		if (keyInputs.Contains(keyInput))
		{
			return false;
		}
		keyInputs.Add(keyInput);
		KeyInputInfo keyInputInfo = infos.Find((KeyInputInfo m) => m.keyCode == keyInput.Key);
		if (keyInputInfo == null)
		{
			keyInputInfo = new KeyInputInfo(keyInput.Key);
			infos.Add(keyInputInfo);
		}
		keyInputInfo.keyInputs.Add(keyInput);
		Log.Info("<color=cyan><b>[SKFramework.Input.Info]</b></color> 注册键盘按键[{0}]输入监听", keyInput.Key);
		return true;
	}

	public bool Unregister(KeyInput keyInput)
	{
		if (!keyInputs.Contains(keyInput))
		{
			return false;
		}
		keyInputs.Remove(keyInput);
		KeyInputInfo keyInputInfo = infos.Find((KeyInputInfo m) => m.keyCode == keyInput.Key);
		if (keyInputInfo != null)
		{
			keyInputInfo.keyInputs.Remove(keyInput);
			if (keyInputInfo.keyInputs.Count == 0)
			{
				infos.Remove(keyInputInfo);
			}
		}
		Log.Info("<color=cyan><b>[SKFramework.Input.Info]</b></color> 注销键盘按键[{0}]输入监听", keyInput.Key);
		return true;
	}

	public bool GetKeyDown(KeyInput keyInput)
	{
		KeyInputInfo keyInputInfo = infos.Find((KeyInputInfo m) => m.keyCode == keyInput.Key);
		if (keyInputInfo != null)
		{
			return keyInputInfo.state == InputState.Pressed;
		}
		return false;
	}

	public bool GetKey(KeyInput keyInput)
	{
		KeyInputInfo keyInputInfo = infos.Find((KeyInputInfo m) => m.keyCode == keyInput.Key);
		if (keyInputInfo != null)
		{
			if (keyInputInfo.state != InputState.Held)
			{
				return keyInputInfo.state == InputState.Pressed;
			}
			return true;
		}
		return false;
	}

	public bool GetKeyUp(KeyInput keyInput)
	{
		KeyInputInfo keyInputInfo = infos.Find((KeyInputInfo m) => m.keyCode == keyInput.Key);
		if (keyInputInfo != null)
		{
			return keyInputInfo.state == InputState.Released;
		}
		return false;
	}

	public void Trigger(KeyCode keyCode, InputTriggerType type)
	{
		if (keyCode != 0 && !triggers.ContainsKey(keyCode) && type != InputTriggerType.Held)
		{
			triggers.Add(keyCode, new InputTrigger(type));
			Log.Info("<color=cyan><b>[SKFramework.Input.Info]</b></color> 触发键盘按键[{0}]{1}", keyCode, (type == InputTriggerType.Pressed) ? "按下" : "抬起");
		}
	}

	public void Trigger(KeyCode keyCode, InputTriggerType type, Func<bool> disposeWhen)
	{
		if (keyCode != 0 && !triggers.ContainsKey(keyCode) && type == InputTriggerType.Held)
		{
			triggers.Add(keyCode, new InputTrigger(type, disposeWhen));
			Log.Info("<color=cyan><b>[SKFramework.Input.Info]</b></color> 触发键盘按键[{0}]持续按下", keyCode);
		}
	}
}
