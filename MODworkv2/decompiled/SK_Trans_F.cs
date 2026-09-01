using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Trans_F : MonoBehaviour
{
	[HideInInspector]
	public SkillOBJ_DT_SP sp;

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
		sp = GetComponent<SkillOBJ_DT_SP>();
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
				Dicform component4 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity, base.transform.parent.transform).GetComponent<Dicform>();
				component4.sp = sp;
				component4.SetCount(sp.ZY);
				component4.SubType = 0;
				component4.Index = 0;
				break;
			}
			case 1:
			{
				Vector3 right2 = base.transform.right;
				float z2 = Mathf.Atan2(right2.y, right2.x) * 57.29578f;
				Dicform component3 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, z2), base.transform.parent.transform).GetComponent<Dicform>();
				component3.sp = sp;
				component3.SetCount(sp.ZY);
				component3.SubType = 0;
				component3.Index = 0;
				break;
			}
			case 2:
			{
				Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component2.sp = sp;
				component2.SetCount(sp.ZY);
				component2.SubType = 0;
				component2.Index = 0;
				break;
			}
			case 3:
			{
				Vector3 right = base.transform.right;
				float z = Mathf.Atan2(right.y, right.x) * 57.29578f;
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, z)).GetComponent<Dicform>();
				component.sp = sp;
				component.SetCount(sp.ZY);
				component.SubType = 0;
				component.Index = 0;
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
		IndexA = sp.EXP_F;
		IndexB = sp.MainEL;
		type = sp.TypeEXP_F;
		ATtime = sp.EXP_time;
		EXPcount = sp.CountEXP;
		switch (type)
		{
		case 0:
		{
			Dicform component4 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity, base.transform.parent.gameObject.GetComponent<Transform>().transform).GetComponent<Dicform>();
			component4.sp = sp;
			component4.SubType = 0;
			component4.Index = 0;
			break;
		}
		case 1:
		{
			Vector3 right2 = base.transform.right;
			float z2 = Mathf.Atan2(right2.y, right2.x) * 57.29578f;
			Dicform component3 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, z2), base.transform.parent.gameObject.GetComponent<Transform>().transform).GetComponent<Dicform>();
			component3.sp = sp;
			component3.SubType = 0;
			component3.Index = 0;
			break;
		}
		case 2:
		{
			Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component2.sp = sp;
			component2.SubType = 0;
			component2.Index = 0;
			break;
		}
		case 3:
		{
			Vector3 right = base.transform.right;
			float z = Mathf.Atan2(right.y, right.x) * 57.29578f;
			Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[IndexA].OBJ[IndexB], base.transform.position, Quaternion.Euler(0f, 0f, z)).GetComponent<Dicform>();
			component.sp = sp;
			component.SubType = 0;
			component.Index = 0;
			break;
		}
		}
		EXPcountTmp = 1;
	}

	public void Stop()
	{
		timeA = 0f;
		canAT = false;
		this.wait(2f, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
