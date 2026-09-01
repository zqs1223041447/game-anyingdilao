using Lean.Pool;
using UnityEngine;

public class SK_Drop_Dic : MonoBehaviour
{
	public Skill_PB_List[] OBJ;

	[HideInInspector]
	public Dicform dic;

	private bool CanAT;

	private float timeA;

	private int IndexA;

	private int IndexB;

	private int Count;

	private float spMin;

	private float spMax;

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
			if (timeA >= 0.2f)
			{
				timeA = 0f;
				LeanPool.Despawn(this);
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
		spMin = dic.sp.Speed3;
		spMax = dic.sp.Speed4;
		IndexA = dic.sp.Dic_S;
		IndexB = dic.sp.MainEL;
		Count = dic.sp.Count_AB;
		for (int i = 0; i < Count; i++)
		{
			Dicform component = LeanPool.Spawn(OBJ[IndexA].PB[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SetCount(dic.sp.ZY);
			component.SubType = dic.SubType;
			component.Index = dic.Index;
			component.dic = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			component.speed = Random.Range(spMin, spMax);
		}
	}
}
