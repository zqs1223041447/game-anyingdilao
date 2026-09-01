using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Trans_S : MonoBehaviour
{
	[HideInInspector]
	public Dicform dic;

	private bool canAT;

	private float timeA;

	private int IndexA;

	private int IndexB;

	private int type;

	private float ATtime;

	private int EXPcount;

	private int EXPcountTmp;

	private GameDataManager _gameDataManager;

	private bool initialized;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		canAT = false;
		EXPcountTmp = 0;
		initialized = false;
	}

	private void Update()
	{
		if (!canAT)
		{
			return;
		}
		timeA += Time.deltaTime;
		if (!(timeA >= ATtime))
		{
			return;
		}
		timeA = 0f;
		if (EXPcountTmp < EXPcount)
		{
			switch (type)
			{
			case 0:
			{
				Dicform component4 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity, base.transform.parent.gameObject.GetComponent<Transform>().transform).GetComponent<Dicform>();
				component4.sp = dic.sp;
				component4.SetCount(dic.sp.ZY);
				component4.SubType = dic.SubType;
				component4.Index = dic.Index;
				break;
			}
			case 1:
			{
				Vector3 right2 = base.transform.right;
				float z2 = Mathf.Atan2(right2.y, right2.x) * 57.29578f;
				Dicform component3 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, z2), base.transform.parent.gameObject.GetComponent<Transform>().transform).GetComponent<Dicform>();
				component3.sp = dic.sp;
				component3.SetCount(dic.sp.ZY);
				component3.SubType = dic.SubType;
				component3.Index = dic.Index;
				break;
			}
			case 2:
			{
				Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component2.sp = dic.sp;
				component2.SetCount(dic.sp.ZY);
				component2.SubType = dic.SubType;
				component2.Index = dic.Index;
				break;
			}
			case 3:
			{
				Vector3 right = base.transform.right;
				float z = Mathf.Atan2(right.y, right.x) * 57.29578f;
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, z)).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index;
				break;
			}
			}
			EXPcountTmp++;
		}
		else
		{
			Stop();
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
		canAT = true;
		ATtime = dic.sp.EXP_time;
		EXPcount = dic.sp.CountEXP;
		switch (dic.SubType)
		{
		case 0:
			IndexA = dic.sp.EXP_S;
			IndexB = dic.sp.MainEL;
			type = dic.sp.TypeEXP_S;
			break;
		case 1:
		case 2:
			IndexA = dic.sp.EXP_AB;
			IndexB = dic.sp.MainEL;
			type = dic.sp.TypeEXP_AB;
			break;
		}
		switch (type)
		{
		case 0:
		{
			Dicform component4 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity, base.transform.parent.transform).GetComponent<Dicform>();
			component4.sp = dic.sp;
			component4.SubType = dic.SubType;
			component4.Index = dic.Index;
			break;
		}
		case 1:
		{
			Vector3 right2 = base.transform.right;
			float z2 = Mathf.Atan2(right2.y, right2.x) * 57.29578f;
			Dicform component3 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, z2), base.transform.parent.transform).GetComponent<Dicform>();
			component3.sp = dic.sp;
			component3.SubType = dic.SubType;
			component3.Index = dic.Index;
			break;
		}
		case 2:
		{
			Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component2.sp = dic.sp;
			component2.SubType = dic.SubType;
			component2.Index = dic.Index;
			break;
		}
		case 3:
		{
			Vector3 right = base.transform.right;
			float z = Mathf.Atan2(right.y, right.x) * 57.29578f;
			Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, z)).GetComponent<Dicform>();
			component.sp = dic.sp;
			component.SubType = dic.SubType;
			component.Index = dic.Index;
			break;
		}
		}
		EXPcountTmp = 1;
	}

	public void Stop()
	{
		timeA = 0f;
		canAT = false;
		this.wait(0.5f, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
