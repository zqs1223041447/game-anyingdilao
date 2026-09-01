using FinkFramework.Runtime.Singleton;
using UnityEngine;

public class Dicform : MonoBehaviour
{
	[HideInInspector]
	public Vector2 dic;

	[HideInInspector]
	public float speed;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public SkillOBJ_DT_CP cp;

	[HideInInspector]
	public int SubType;

	[HideInInspector]
	public int Index;

	[HideInInspector]
	public PlayerManager PL;

	public int type;

	[HideInInspector]
	public bool ZY;

	[HideInInspector]
	public bool CutSpeed;

	[HideInInspector]
	public float UPDamage;

	[HideInInspector]
	public bool ChangeFL;

	private void Awake()
	{
		PL = SingletonMonoScope<PlayerManager>.Instance;
		CutSpeed = false;
		UPDamage = 0f;
		ChangeFL = false;
	}

	private void OnEnable()
	{
	}

	public void SetCount(bool zy)
	{
		ZY = zy;
		if (ZY)
		{
			PL.PrefabCount(type, add: true);
		}
	}

	private void OnDisable()
	{
		if (ZY)
		{
			PL.PrefabCount(type, add: false);
		}
		CutSpeed = false;
		UPDamage = 0f;
		ChangeFL = false;
	}
}
