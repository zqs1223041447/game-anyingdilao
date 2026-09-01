using FinkFramework.Runtime.Singleton;
using Inputs.Gamepad;
using Lean.Pool;
using UnityEngine;

public class SK_FSQ_fatherPOS : MonoBehaviour
{
	public GameObject OBJ;

	public bool HasAngle;

	public float FirstDelay;

	public float SecondDelay;

	public GameObject SubA;

	public bool HasAngleA;

	public float DelayA;

	public GameObject SubB;

	public bool HasAngleB;

	public float DelayB;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeD;

	private bool FirstFashe;

	private bool CanFS;

	private bool SubAOK;

	private bool SubBOK;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeD = 0f;
		FirstFashe = false;
		CanFS = false;
		SubAOK = false;
		SubBOK = false;
		initialized = false;
	}

	private void Update()
	{
		if (!CanFS)
		{
			return;
		}
		if (!FirstFashe)
		{
			timeB += Time.deltaTime;
			if (timeB > FirstDelay)
			{
				timeB = 0f;
				FirstFashe = true;
				FaShe();
			}
		}
		timeA += Time.deltaTime;
		if (timeA > sp.BuffTime)
		{
			timeA = 0f;
			LeanPool.Despawn(this);
		}
		if (!SubAOK)
		{
			timeC += Time.deltaTime;
			if (timeC > DelayA)
			{
				timeC = 0f;
				SubAOK = true;
				Vector3 vector = AimProvider.GetAimWorldPos() - SingletonMonoScope<PlayerManager>.Instance.transform.position;
				float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				if (sp.Layer_SubA == 0 && sp.DamageA > 0f && (bool)SubA)
				{
					Dicform component = ((!HasAngleA) ? LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity) : LeanPool.Spawn(SubA, base.transform.position, Quaternion.Euler(0f, 0f, z))).GetComponent<Dicform>();
					component.sp = sp;
					component.SetCount(sp.ZY);
					component.SubType = 1;
					component.Index = 1;
				}
			}
		}
		if (SubBOK)
		{
			return;
		}
		timeD += Time.deltaTime;
		if (timeD > DelayB)
		{
			timeD = 0f;
			SubBOK = true;
			Vector3 vector2 = AimProvider.GetAimWorldPos() - SingletonMonoScope<PlayerManager>.Instance.transform.position;
			float z2 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
			if (sp.Layer_SubB == 0 && sp.DamageB > 0f && SubB != null)
			{
				Dicform component2 = ((!HasAngleB) ? LeanPool.Spawn(SubB, base.transform.position, Quaternion.identity) : LeanPool.Spawn(SubB, base.transform.position, Quaternion.Euler(0f, 0f, z2))).GetComponent<Dicform>();
				component2.sp = sp;
				component2.SetCount(sp.ZY);
				component2.SubType = 2;
				component2.Index = 1;
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
		CanFS = true;
	}

	public void FaShe()
	{
		Vector3 vector = AimProvider.GetAimWorldPos() - SingletonMonoScope<PlayerManager>.Instance.transform.position;
		float ZZZ = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		Dicform component = ((!HasAngle) ? LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity) : LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, ZZZ))).GetComponent<Dicform>();
		component.sp = sp;
		component.SetCount(sp.ZY);
		component.SubType = 0;
		component.Index = 1;
		component.dic = Vector2.zero;
		if (sp.CF_Rate > 0f && (float)Random.Range(0, 101) < sp.CF_Rate)
		{
			this.wait(SecondDelay, delegate
			{
				FaSheB(ZZZ);
			});
		}
	}

	public void FaSheB(float Z)
	{
		Dicform component = ((!HasAngle) ? LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity) : LeanPool.Spawn(OBJ, base.transform.position, Quaternion.Euler(0f, 0f, Z))).GetComponent<Dicform>();
		component.sp = sp;
		component.SetCount(sp.ZY);
		component.SubType = 0;
		component.Index = 1;
		component.dic = Vector2.zero;
	}
}
