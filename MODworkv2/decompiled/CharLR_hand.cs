using FinkFramework.Runtime.Singleton;
using UnityEngine;
using UnityEngine.UI;

public class CharLR_hand : MonoBehaviour
{
	public Image icon;

	public Sprite[] spr;

	private void Awake()
	{
		icon = GetComponent<Image>();
	}

	private void Start()
	{
		icon.sprite = spr[SingletonMonoScope<PlayerManager>.Instance.PLType];
		icon.SetNativeSize();
	}
}
