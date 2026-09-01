using System.Collections.Generic;
using Data.AutoGen.DataClass.Settings;
using FinkFramework.Runtime.ResLoad;
using FinkFramework.Runtime.Singleton;
using Inputs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindUI : MonoBehaviour
{
	[Header("Skill Slots")]
	public GameObject skill1;

	public GameObject skill2;

	public GameObject skill3;

	public GameObject skill4;

	public GameObject skill5;

	public GameObject skill6;

	public GameObject skill7;

	public GameObject skill8;

	[Header("Item Slots")]
	public GameObject item1;

	public GameObject item2;

	private readonly Dictionary<ControlAction, GameObject> actionTargets = new Dictionary<ControlAction, GameObject>();

	private readonly List<GameObject> _spawnedThisBuild = new List<GameObject>();

	private void Awake()
	{
		BuildActionTargets();
	}

	private void BuildActionTargets()
	{
		actionTargets.Clear();
		actionTargets[ControlAction.Skill1] = ResolveSkillTarget(ControlAction.Skill1, skill1);
		actionTargets[ControlAction.Skill2] = ResolveSkillTarget(ControlAction.Skill2, skill2);
		actionTargets[ControlAction.Skill3] = ResolveSkillTarget(ControlAction.Skill3, skill3);
		actionTargets[ControlAction.Skill4] = ResolveSkillTarget(ControlAction.Skill4, skill4);
		actionTargets[ControlAction.Skill5] = ResolveSkillTarget(ControlAction.Skill5, skill5);
		actionTargets[ControlAction.Skill6] = ResolveSkillTarget(ControlAction.Skill6, skill6);
		actionTargets[ControlAction.Skill7] = ResolveSkillTarget(ControlAction.Skill7, skill7);
		actionTargets[ControlAction.Skill8] = ResolveSkillTarget(ControlAction.Skill8, skill8);
		actionTargets[ControlAction.Item1] = ResolveUseTarget(0, item1);
		actionTargets[ControlAction.Item2] = ResolveUseTarget(1, item2);
	}

	private GameObject ResolveSkillTarget(ControlAction action, GameObject serializedTarget)
	{
		int num = ActionToSkillSlotIndex(action);
		if (num >= 0 && SingletonMonoScope<ACTbar>.HasInstance && SingletonMonoScope<ACTbar>.Instance.skillBT != null && num < SingletonMonoScope<ACTbar>.Instance.skillBT.Length)
		{
			ACT_skillBT aCT_skillBT = SingletonMonoScope<ACTbar>.Instance.skillBT[num];
			if ((bool)aCT_skillBT)
			{
				return aCT_skillBT.gameObject;
			}
		}
		if ((bool)serializedTarget)
		{
			return serializedTarget;
		}
		return FindChildGameObject(action.ToString(), action.ToString().ToLowerInvariant());
	}

	private GameObject ResolveUseTarget(int index, GameObject serializedTarget)
	{
		if (SingletonMonoScope<ACTbar>.HasInstance && SingletonMonoScope<ACTbar>.Instance.useBT != null && index >= 0 && index < SingletonMonoScope<ACTbar>.Instance.useBT.Length)
		{
			ACT_UseBT aCT_UseBT = SingletonMonoScope<ACTbar>.Instance.useBT[index];
			if ((bool)aCT_UseBT)
			{
				return aCT_UseBT.gameObject;
			}
		}
		return serializedTarget;
	}

	private GameObject FindChildGameObject(params string[] names)
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>(includeInactive: true);
		foreach (Transform transform in componentsInChildren)
		{
			foreach (string text in names)
			{
				if (transform.name == text)
				{
					return transform.gameObject;
				}
			}
		}
		return null;
	}

	private static int ActionToSkillSlotIndex(ControlAction action)
	{
		int num = ((SingletonMonoScope<ACTbar>.HasInstance && SingletonMonoScope<ACTbar>.Instance.skillBT != null) ? SingletonMonoScope<ACTbar>.Instance.skillBT.Length : 0);
		switch (action)
		{
		case ControlAction.Skill3:
			return 0;
		case ControlAction.Skill4:
			return 1;
		case ControlAction.Skill5:
			return 2;
		case ControlAction.Skill6:
			return 3;
		case ControlAction.Skill7:
			if (num < 8)
			{
				return -1;
			}
			return 4;
		case ControlAction.Skill8:
			if (num < 8)
			{
				return 4;
			}
			return 5;
		case ControlAction.Skill1:
			if (num < 8)
			{
				return 5;
			}
			return 6;
		case ControlAction.Skill2:
			if (num < 8)
			{
				return 6;
			}
			return 7;
		default:
			return -1;
		}
	}

	private void OnEnable()
	{
		CurrentInputManager.OnCurrentInputDeviceChanged += HandleInputDeviceChanged;
	}

	private void OnDisable()
	{
		CurrentInputManager.OnCurrentInputDeviceChanged -= HandleInputDeviceChanged;
	}

	private void HandleInputDeviceChanged(InputDeviceType deviceType)
	{
		RefreshUI();
	}

	private void Start()
	{
		this.wait(0.001f, BuildAll);
	}

	private void ClearLastBuild()
	{
		foreach (GameObject item in _spawnedThisBuild)
		{
			if ((bool)item)
			{
				Object.Destroy(item);
			}
		}
		_spawnedThisBuild.Clear();
	}

	private void BuildAll()
	{
		BuildActionTargets();
		ControlsSettingData currentControl = Singleton<SettingDataManager>.Instance.GetCurrentControl();
		if (currentControl == null)
		{
			return;
		}
		ClearLastBuild();
		foreach (KeyValuePair<ControlAction, GameObject> actionTarget in actionTargets)
		{
			ControlAction key = actionTarget.Key;
			GameObject value = actionTarget.Value;
			if (!value)
			{
				continue;
			}
			string bind = currentControl.GetBind(key);
			if (bind != null && !string.IsNullOrEmpty(bind))
			{
				GameObject gameObject = CreateHint(bind, value.transform);
				if ((bool)gameObject)
				{
					_spawnedThisBuild.Add(gameObject);
				}
			}
		}
	}

	private static GameObject CreateHint(string rawKey, Transform parent)
	{
		GameObject gameObject = Singleton<ResManager>.Instance.Load<GameObject>("res://UI/Icons/Input_Text");
		if (!gameObject)
		{
			return null;
		}
		GameObject gameObject2 = Object.Instantiate(gameObject, parent);
		TextMeshProUGUI componentInChildren = gameObject2.GetComponentInChildren<TextMeshProUGUI>();
		Text componentInChildren2 = gameObject2.GetComponentInChildren<Text>();
		if (KeyDisplayUtil.TryGetSpriteRichText(rawKey, out var richText))
		{
			if ((bool)componentInChildren)
			{
				if ((bool)componentInChildren2)
				{
					componentInChildren2.gameObject.SetActive(value: false);
				}
				componentInChildren.gameObject.SetActive(value: true);
				componentInChildren.text = richText;
				return gameObject2;
			}
		}
		else
		{
			string text = KeyDisplayUtil.ToDisplayName(rawKey);
			if ((bool)componentInChildren)
			{
				if ((bool)componentInChildren2)
				{
					componentInChildren2.gameObject.SetActive(value: false);
				}
				componentInChildren.gameObject.SetActive(value: true);
				componentInChildren.text = text;
				return gameObject2;
			}
			if ((bool)componentInChildren2)
			{
				componentInChildren2.gameObject.SetActive(value: true);
				componentInChildren2.text = text;
			}
		}
		return gameObject2;
	}

	public void RefreshUI()
	{
		BuildAll();
	}
}
