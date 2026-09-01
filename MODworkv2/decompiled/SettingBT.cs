using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class SettingBT : MonoBehaviour
{
	private static readonly int light1 = Animator.StringToHash("light");

	[SerializeField]
	private Animator ani;

	[SerializeField]
	private GameObject san;

	[SerializeField]
	private CanvasGroup canvas;

	public GameObject WZ;

	private int lastPlayerLevel = -1;

	private void OnEnable()
	{
		RefreshWZ();
	}

	private void Start()
	{
		RefreshWZ();
	}

	private void Update()
	{
		RefreshWZ();
	}

	private void RefreshWZ()
	{
		if ((bool)WZ && SingletonMonoScope<PlayerManager>.HasInstance)
		{
			int level = SingletonMonoScope<PlayerManager>.Instance.Level;
			if (level != lastPlayerLevel)
			{
				lastPlayerLevel = level;
				WZ.SetActive(level <= 30);
			}
		}
	}

	public void lightBT()
	{
		ani.SetInteger(light1, 1);
		san.SetActive(value: true);
	}

	public void UNlightBT()
	{
		ani.SetInteger(light1, 0);
		san.SetActive(value: false);
	}
}
