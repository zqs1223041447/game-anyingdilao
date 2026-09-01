using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Zone_Comp : MonoBehaviour
{
	public GameObject FX;

	public float size;

	public float DotMulti;

	public bool Body;

	[HideInInspector]
	public Dicform dic;

	private bool CanAT;

	private float timeA;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		CanAT = false;
		initialized = false;
	}

	private void Update()
	{
		if (CanAT)
		{
			timeA += Time.deltaTime;
			if (timeA >= 0.5f)
			{
				Fashe();
				timeA = 0f;
			}
		}
	}

	private void LateUpdate()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!initialized && CanInitialize())
		{
			initialized = true;
			SetStart();
		}
	}

	private bool CanInitialize()
	{
		Dicform component = GetComponent<Dicform>();
		if (component != null && component.sp == null)
		{
			return false;
		}
		return true;
	}

	public void SetStart()
	{
		CanAT = true;
	}

	public void Fashe()
	{
		EmptyCOL component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol, base.transform.position, Quaternion.identity).GetComponent<EmptyCOL>();
		Dicform component2 = component.GetComponent<Dicform>();
		component2.sp = dic.sp;
		component2.SetCount(dic.sp.ZY);
		component2.SubType = 2;
		float num = ((dic != null && dic.sp != null) ? ((float)dic.sp.Field_Range) : 0f);
		component.size = size * Mathf.Max(0f, 1f + num / 100f);
		component.Body = Body;
		component.DotMulti = DotMulti;
		component.lifeTime = 0.1f;
		component.IsGround = false;
		if (FX != null)
		{
			component.FX = FX;
		}
	}
}
