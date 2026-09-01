using FinkFramework.Runtime.Singleton;
using Inputs.Gamepad;
using Lean.Pool;
using UnityEngine;

public class SK_Angle_Self : MonoBehaviour
{
	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private float timeB;

	private bool CanAT;

	private Gun gun;

	private float AG;

	private GameDataManager _gameDataManager;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		gun = SingletonMonoScope<Gun>.Instance;
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		CanAT = false;
		initialized = false;
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance || !CanAT)
		{
			return;
		}
		timeA += Time.deltaTime;
		if (timeA >= sp.FStime1)
		{
			timeA = 0f;
			Vector3 aimWorldPos = AimProvider.GetAimWorldPos();
			Vector3 vector = aimWorldPos - base.transform.position;
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			for (int i = 0; i < sp.CountMulti; i++)
			{
				AG = Random.Range(sp.AngleA, 0f - sp.AngleA);
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num + AG)).GetComponent<Dicform>();
				component.sp = sp;
				component.SetCount(sp.ZY);
				component.SubType = 0;
				component.Index = 0;
			}
			if (sp.Type_F == 1)
			{
				Vector3 vector2 = base.transform.position - aimWorldPos;
				float num2 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
				for (int j = 0; j < sp.CountMulti; j++)
				{
					AG = Random.Range(sp.AngleA, 0f - sp.AngleA);
					Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], base.transform.position, Quaternion.Euler(0f, 0f, num2 + AG)).GetComponent<Dicform>();
					component2.sp = sp;
					component2.SetCount(sp.ZY);
					component2.SubType = 0;
					component2.Index = 0;
				}
			}
		}
		timeB += Time.deltaTime;
		if (timeB >= sp.FStime1 * (float)sp.Count_F + 0.1f)
		{
			timeB = 0f;
			LeanPool.Despawn(this);
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
}
