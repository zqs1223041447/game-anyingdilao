using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_CP_Universe : MonoBehaviour
{
	public string[] SoundA;

	public Skill_PB_List[] OBJ;

	public GameObject Aixs;

	public float AixsSpeedMin;

	public float AixsSpeedMax;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public SK_BuffA mg;

	[HideInInspector]
	public List<SK_CP_Universe_Aixs> AxisList = new List<SK_CP_Universe_Aixs>();

	[HideInInspector]
	public List<SK_CP_Universe_Ball> BallList = new List<SK_CP_Universe_Ball>();

	private float timeA;

	private float timeB;

	private float timeC;

	private bool CanAT;

	private bool initialized;

	private PlayerManager PL;

	private Companion CP;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		CanAT = false;
		AxisList.Clear();
		BallList.Clear();
		initialized = false;
	}

	private void Update()
	{
		if (!CanAT)
		{
			return;
		}
		if (!CP || !CP.IsAlive)
		{
			Stop();
			return;
		}
		if (sp.NoTime == 1)
		{
			timeA += Time.deltaTime;
			if (timeA >= sp.BuffTime)
			{
				Stop();
				return;
			}
			if ((bool)mg && mg.NeedStop)
			{
				Stop();
				return;
			}
		}
		else if ((bool)mg && mg.ORBStop)
		{
			Stop();
			return;
		}
		float num = sp.FStime1 - sp.FStime1 * PL.Orb_Universe_ATS / 100f;
		if (num <= 0.02f)
		{
			num = 0.02f;
		}
		timeB += Time.deltaTime;
		if (timeB >= num)
		{
			timeB = 0f;
			if (GetAliveBallCount() < GetMaxBallCount() && CanCasterShoot())
			{
				Fashe();
			}
		}
		timeC += Time.deltaTime;
		if (timeC >= 0.15f)
		{
			RefreshCompanionUniverseData();
			timeC = 0f;
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
		if ((bool)sp && (bool)sp.cp)
		{
			return sp.cp.yao;
		}
		return false;
	}

	public void SetStart()
	{
		CP = sp.cp;
		CanAT = true;
		if (sp.Count_F > 0 && SoundA != null && sp.MainEL >= 0 && sp.MainEL < SoundA.Length && !string.IsNullOrEmpty(SoundA[sp.MainEL]))
		{
			RuntimeManager.PlayOneShot(SoundA[sp.MainEL], base.transform.position);
		}
	}

	private bool CanCasterShoot()
	{
		if ((bool)CP)
		{
			return CP.IsAlive;
		}
		return false;
	}

	private int GetMaxBallCount()
	{
		return Mathf.FloorToInt((float)(sp.Count_F + PL.ORB_FQ_Count + PL.BE_SK_FQ_Count * PL.BE_SK_Count + PL.BE_BS_FQ_Count * PL.BE_BS_Count) * PL.ORB_FQ_Double);
	}

	private int GetAliveBallCount()
	{
		for (int num = BallList.Count - 1; num >= 0; num--)
		{
			if (!BallList[num] || !BallList[num].gameObject.activeInHierarchy)
			{
				BallList.RemoveAt(num);
			}
		}
		return BallList.Count;
	}

	public void Fashe()
	{
		if (OBJ == null || sp.ZD_F < 0 || sp.ZD_F >= OBJ.Length || OBJ[sp.ZD_F] == null || OBJ[sp.ZD_F].PB == null || sp.MainEL < 0 || sp.MainEL >= OBJ[sp.ZD_F].PB.Length)
		{
			return;
		}
		GameObject gameObject = OBJ[sp.ZD_F].PB[sp.MainEL];
		if ((bool)gameObject && (bool)Aixs)
		{
			SK_CP_Universe_Aixs component = LeanPool.Spawn(Aixs, base.transform.position, Quaternion.identity, base.transform).GetComponent<SK_CP_Universe_Aixs>();
			InitAxis(component);
			AxisList.Add(component);
			SK_CP_Universe_Ball component2 = LeanPool.Spawn(gameObject, component.point.position, Quaternion.identity).GetComponent<SK_CP_Universe_Ball>();
			component.BindBall(component2, this);
			component2.target = component.point;
			component2.ownerAxis = component;
			component2.father = this;
			BallList.Add(component2);
			Dicform component3 = component2.GetComponent<Dicform>();
			if ((bool)component3)
			{
				component3.sp = sp;
				component3.SetCount(sp.ZY);
				component3.SubType = 0;
				component3.Index = 0;
			}
		}
	}

	private void InitAxis(SK_CP_Universe_Aixs ax)
	{
		float num = Random.Range(AixsSpeedMin, AixsSpeedMax);
		ax.speed = ((Random.Range(0, 101) < 50) ? num : (0f - num));
		float num2 = Random.Range(sp.Range1, sp.Range2);
		ax.transform.localScale = new Vector3(num2, num2, num2);
		ax.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		ax.CanZhuan = true;
		ax.father = this;
	}

	public void NotifyAxisReleased(SK_CP_Universe_Aixs ax)
	{
		if ((bool)ax)
		{
			AxisList.Remove(ax);
		}
	}

	public void NotifyBallDespawn(SK_CP_Universe_Ball ball)
	{
		if ((bool)ball)
		{
			BallList.Remove(ball);
		}
	}

	private void RefreshCompanionUniverseData()
	{
		if ((bool)PL && (bool)CP && (bool)sp)
		{
			float num = PL.ORB_Damage_Last + PL.Orb_Universe_DMG_Last;
			sp.Damage = PL.GiveDamage(sp.damageType) * CP.Damage_Last / 100f * sp.SPC_Damage / 100f * (1f + num / 100f);
			sp.DamageA = PL.GiveDamage(sp.damageType) * CP.Damage_Last / 100f * sp.SPC_DamageA / 100f * (1f + num / 100f);
			sp.DamageB = PL.GiveDamage(sp.damageType) * CP.Damage_Last / 100f * sp.SPC_DamageB / 100f * (1f + num / 100f);
			sp.JYrate = PL.JYrate_Last;
			sp.BJrate = (CP.BJ_NoDot ? 100f : PL.BJrate_Last);
			sp.BJDamage = PL.BJDamage_Last;
			sp.Through = PL.ThroughRate;
			sp.Chuan = PL.GiveChuan(sp.damageType);
			sp.FlySpeed = CP.FlySpeed;
		}
	}

	public void Stop()
	{
		CanAT = false;
		if (!CP && (bool)sp)
		{
			CP = sp.cp;
		}
		if ((bool)CP)
		{
			CP.RemoveUniverse(this);
		}
		for (int num = BallList.Count - 1; num >= 0; num--)
		{
			if ((bool)BallList[num])
			{
				BallList[num].Stop(BallList[num].transform, self: true);
			}
		}
		BallList.Clear();
		for (int num2 = AxisList.Count - 1; num2 >= 0; num2--)
		{
			if ((bool)AxisList[num2])
			{
				AxisList[num2].Stop();
			}
		}
		AxisList.Clear();
		LeanPool.Despawn(this);
	}
}
