using System;
using System.Collections.Generic;
using UnityEngine;

namespace SK.Framework;

public sealed class MouseInputController
{
	private class MouseButtonInputInfo
	{
		public readonly int mouseButton;

		public readonly List<MouseButtonInput> mouseButtonInputs;

		public InputState state;

		public MouseButtonInputInfo(int mouseButton)
		{
			this.mouseButton = mouseButton;
			mouseButtonInputs = new List<MouseButtonInput>();
		}

		public void Reset()
		{
			state = InputState.None;
		}
	}

	private readonly List<MouseButtonInput> mouseButtonInputs;

	private readonly List<MouseButtonInputInfo> infos;

	private readonly Dictionary<int, InputTrigger> triggers;

	private Vector2 cacheMousePosition;

	public Vector2 Delta { get; private set; }

	public MouseInputController()
	{
		mouseButtonInputs = new List<MouseButtonInput>();
		infos = new List<MouseButtonInputInfo>();
		triggers = new Dictionary<int, InputTrigger>();
	}

	public void Update()
	{
		Vector2 vector = Input.mousePosition;
		Delta = vector - cacheMousePosition;
		for (int i = 0; i < mouseButtonInputs.Count; i++)
		{
			mouseButtonInputs[i].Value = Input.GetMouseButton(mouseButtonInputs[i].Key);
		}
		for (int j = 0; j < infos.Count; j++)
		{
			MouseButtonInputInfo mouseButtonInputInfo = infos[j];
			bool flag = false;
			for (int num = mouseButtonInputInfo.mouseButtonInputs.Count - 1; num >= 0; num--)
			{
				if (mouseButtonInputInfo.mouseButtonInputs[num].Value)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				mouseButtonInputInfo.state = ((mouseButtonInputInfo.state == InputState.None || mouseButtonInputInfo.state == InputState.Released) ? InputState.Pressed : InputState.Held);
				triggers.Clear();
			}
			else if (triggers.ContainsKey(mouseButtonInputInfo.mouseButton))
			{
				switch (triggers[mouseButtonInputInfo.mouseButton].type)
				{
				case InputTriggerType.Pressed:
					mouseButtonInputInfo.state = InputState.Pressed;
					triggers.Remove(mouseButtonInputInfo.mouseButton);
					break;
				case InputTriggerType.Held:
					mouseButtonInputInfo.state = InputState.Held;
					if (triggers[mouseButtonInputInfo.mouseButton].disposeWhen())
					{
						triggers.Remove(mouseButtonInputInfo.mouseButton);
					}
					break;
				case InputTriggerType.Released:
					mouseButtonInputInfo.state = InputState.Released;
					triggers.Remove(mouseButtonInputInfo.mouseButton);
					break;
				}
			}
			else
			{
				mouseButtonInputInfo.state = ((mouseButtonInputInfo.state == InputState.Pressed || mouseButtonInputInfo.state == InputState.Held) ? InputState.Released : InputState.None);
			}
		}
		cacheMousePosition = Input.mousePosition;
	}

	public void Reset()
	{
		for (int i = 0; i < mouseButtonInputs.Count; i++)
		{
			mouseButtonInputs[i].Reset();
		}
		cacheMousePosition = Input.mousePosition;
		Delta = Vector2.zero;
	}

	public bool Register(MouseButtonInput mouseButtonInput)
	{
		if (mouseButtonInputs.Contains(mouseButtonInput))
		{
			return false;
		}
		mouseButtonInputs.Add(mouseButtonInput);
		int key = mouseButtonInput.Key;
		MouseButtonInputInfo mouseButtonInputInfo = infos.Find((MouseButtonInputInfo m) => m.mouseButton == key);
		if (mouseButtonInputInfo == null)
		{
			mouseButtonInputInfo = new MouseButtonInputInfo(key);
			infos.Add(mouseButtonInputInfo);
		}
		mouseButtonInputInfo.mouseButtonInputs.Add(mouseButtonInput);
		Log.Info("<color=cyan><b>[SKFramework.Input.Info]</b></color> 注册鼠标{0}输入监听", (key == 0) ? "左键" : ((key == 1) ? "右键" : "滚轮"));
		return true;
	}

	public bool Unregister(MouseButtonInput mouseButtonInput)
	{
		if (!mouseButtonInputs.Contains(mouseButtonInput))
		{
			return false;
		}
		mouseButtonInputs.Remove(mouseButtonInput);
		int key = mouseButtonInput.Key;
		MouseButtonInputInfo mouseButtonInputInfo = infos.Find((MouseButtonInputInfo m) => m.mouseButton == key);
		if (mouseButtonInputInfo != null)
		{
			mouseButtonInputInfo.mouseButtonInputs.Remove(mouseButtonInput);
			if (mouseButtonInputInfo.mouseButtonInputs.Count == 0)
			{
				infos.Remove(mouseButtonInputInfo);
			}
		}
		Log.Info("<color=cyan><b>[SKFramework.Input.Info]</b></color> 注销鼠标{0}输入监听", (key == 0) ? "左键" : ((key == 1) ? "右键" : "滚轮"));
		return true;
	}

	public bool GetKeyDown(MouseButtonInput mouseButtonInput)
	{
		MouseButtonInputInfo mouseButtonInputInfo = infos.Find((MouseButtonInputInfo m) => m.mouseButton == mouseButtonInput.Key);
		if (mouseButtonInputInfo != null)
		{
			return mouseButtonInputInfo.state == InputState.Pressed;
		}
		return false;
	}

	public bool GetKey(MouseButtonInput mouseButtonInput)
	{
		MouseButtonInputInfo mouseButtonInputInfo = infos.Find((MouseButtonInputInfo m) => m.mouseButton == mouseButtonInput.Key);
		if (mouseButtonInputInfo != null)
		{
			if (mouseButtonInputInfo.state != InputState.Held)
			{
				return mouseButtonInputInfo.state == InputState.Pressed;
			}
			return true;
		}
		return false;
	}

	public bool GetKeyUp(MouseButtonInput mouseButtonInput)
	{
		MouseButtonInputInfo mouseButtonInputInfo = infos.Find((MouseButtonInputInfo m) => m.mouseButton == mouseButtonInput.Key);
		if (mouseButtonInputInfo != null)
		{
			return mouseButtonInputInfo.state == InputState.Released;
		}
		return false;
	}

	public void Trigger(int mouseButton, InputTriggerType type)
	{
		if ((mouseButton == 0 || mouseButton == 1 || mouseButton == 2) && !triggers.ContainsKey(mouseButton) && type != InputTriggerType.Held)
		{
			triggers.Add(mouseButton, new InputTrigger(type));
			string text = mouseButton switch
			{
				1 => "右键", 
				0 => "左键", 
				_ => "滚轮", 
			};
			Log.Info("<color=cyan><b>[SKFramework.Input.Info]</b></color> 触发鼠标{0}{1}", text, (type == InputTriggerType.Pressed) ? "按下" : "抬起");
		}
	}

	public void Trigger(int mouseButton, InputTriggerType type, Func<bool> disposeWhen)
	{
		if ((mouseButton == 0 || mouseButton == 1 || mouseButton == 2) && !triggers.ContainsKey(mouseButton) && type == InputTriggerType.Held)
		{
			triggers.Add(mouseButton, new InputTrigger(type, disposeWhen));
			string text = mouseButton switch
			{
				1 => "右键", 
				0 => "左键", 
				_ => "滚轮", 
			};
			Log.Info("<color=cyan><b>[SKFramework.Input.Info]</b></color> 触发鼠标{0}持续按下", text);
		}
	}

	public void Trigger(MouseButtonCode mouseButtonCode, InputTriggerType type)
	{
		Trigger((int)mouseButtonCode, type);
	}

	public void Trigger(MouseButtonCode mouseButtonCode, InputTriggerType type, Func<bool> disposeWhen)
	{
		Trigger((int)mouseButtonCode, type, disposeWhen);
	}
}
