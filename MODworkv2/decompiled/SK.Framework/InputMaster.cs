using UnityEngine;

namespace SK.Framework;

public sealed class InputMaster : MonoBehaviour
{
	private static InputMaster instance;

	private KeyInputController keyInputController;

	private MouseInputController mouseButtonInputController;

	private AxisInputController axisInputController;

	private bool toggle = true;

	public static InputMaster Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameObject("[SKFramework.Input]").AddComponent<InputMaster>();
				instance.keyInputController = new KeyInputController();
				instance.mouseButtonInputController = new MouseInputController();
				instance.axisInputController = new AxisInputController();
				Object.DontDestroyOnLoad(instance);
			}
			return instance;
		}
	}

	public static KeyInputController Key => Instance.keyInputController;

	public static MouseInputController Mouse => Instance.mouseButtonInputController;

	public static AxisInputController Axis => Instance.axisInputController;

	public static bool Toggle
	{
		get
		{
			return Instance.toggle;
		}
		set
		{
			if (Instance.toggle != value)
			{
				Instance.toggle = value;
				if (!Instance.toggle)
				{
					Key.Reset();
					Mouse.Reset();
					Axis.Reset();
				}
				Log.Info("<color=cyan><b>[SKFramework.Input.Info]</b></color> {0}输入监听", value ? "打开" : "关闭");
			}
		}
	}

	public static bool IsAnyKey
	{
		get
		{
			if (Toggle)
			{
				return Input.anyKey;
			}
			return false;
		}
	}

	public static bool IsAnyKeyDown
	{
		get
		{
			if (Toggle)
			{
				return Input.anyKeyDown;
			}
			return false;
		}
	}

	private void Update()
	{
		if (toggle)
		{
			keyInputController.Update();
			mouseButtonInputController.Update();
			axisInputController.Update();
		}
	}

	public bool GetKeyDown(KeyCode keyCode)
	{
		if (toggle)
		{
			return Input.GetKeyDown(keyCode);
		}
		return false;
	}

	public bool GetKey(KeyCode keyCode)
	{
		if (toggle)
		{
			return Input.GetKey(keyCode);
		}
		return false;
	}

	public bool GetKeyUp(KeyCode keyCode)
	{
		if (toggle)
		{
			return Input.GetKeyUp(keyCode);
		}
		return false;
	}

	public bool GetMouseButtonDown(int mouseButton)
	{
		if (toggle)
		{
			return Input.GetMouseButtonDown(mouseButton);
		}
		return false;
	}

	public bool GetMouseButton(int mouseButton)
	{
		if (toggle)
		{
			return Input.GetMouseButton(mouseButton);
		}
		return false;
	}

	public bool GetMouseButtonUp(int mouseButton)
	{
		if (toggle)
		{
			return Input.GetMouseButtonUp(mouseButton);
		}
		return false;
	}

	public float GetAxis(string axisName)
	{
		if (!toggle)
		{
			return 0f;
		}
		return Input.GetAxis(axisName);
	}

	public float GetAxisRaw(string axisName)
	{
		if (!toggle)
		{
			return 0f;
		}
		return Input.GetAxisRaw(axisName);
	}
}
