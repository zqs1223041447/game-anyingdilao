using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Universe : MonoBehaviour
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
	public List<SK_Universe_Aixs> AxisList = new List<SK_Universe_Aixs>();

	[HideInInspector]
	public List<SK_Universe_Ball> BallList = new List<SK_Universe_Ball>();

	private float timeA;

	private float timeB;

	private float timeC;

	private bool CanAT;

	private PlayerManager PL;

	private bool initialized;

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
		if (sp.NoTime == 1)
		{
			timeA += Time.deltaTime;
			if (timeA >= sp.BuffTime)
			{
				Stop();
				return;
			}
			if (mg.NeedStop)
			{
				Stop();
				return;
			}
		}
		else if (mg.ORBStop)
		{
			Stop();
			return;
		}
		timeB += Time.deltaTime;
		if (timeB >= sp.FStime1 - sp.FStime1 * PL.Orb_Universe_ATS / 100f)
		{
			timeB = 0f;
			if ((float)GetAliveBallCount() < (float)(sp.Count_F + PL.ORB_FQ_Count + PL.BE_SK_FQ_Count * PL.BE_SK_Count + PL.BE_BS_FQ_Count * PL.BE_BS_Count) * PL.ORB_FQ_Double && CanCasterShoot())
			{
				Fashe();
			}
		}
		timeC += Time.deltaTime;
		if (timeC >= 0.15f)
		{
			if (sp.SpecialType == 10)
			{
				PL.RefreshORB(sp, 3);
			}
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
		return true;
	}

	public void SetStart()
	{
		CanAT = true;
		if (sp.Count_F > 0 && SoundA != null && sp.MainEL >= 0 && sp.MainEL < SoundA.Length && !string.IsNullOrEmpty(SoundA[sp.MainEL]))
		{
			RuntimeManager.PlayOneShot(SoundA[sp.MainEL], base.transform.position);
		}
	}

	private bool CanCasterShoot()
	{
		switch (sp.indexType)
		{
		case 0:
			if ((bool)PL)
			{
				return PL.IsAlive;
			}
			return false;
		case 1:
			if ((bool)sp.cp)
			{
				return sp.cp.IsAlive;
			}
			return false;
		case 2:
			if ((bool)sp.em)
			{
				return sp.em.IsAlive;
			}
			return false;
		default:
			return false;
		}
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
		if ((bool)gameObject)
		{
			SK_Universe_Aixs component = LeanPool.Spawn(Aixs, base.transform.position, Quaternion.identity, base.transform).GetComponent<SK_Universe_Aixs>();
			InitAxis(component);
			AxisList.Add(component);
			SK_Universe_Ball component2 = LeanPool.Spawn(gameObject, component.point.position, Quaternion.identity).GetComponent<SK_Universe_Ball>();
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

	private void InitAxis(SK_Universe_Aixs ax)
	{
		float num = Random.Range(AixsSpeedMin, AixsSpeedMax);
		ax.speed = ((Random.Range(0, 101) < 50) ? num : (0f - num));
		float num2 = Random.Range(sp.Range1, sp.Range2);
		ax.transform.localScale = new Vector3(num2, num2, num2);
		ax.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		ax.CanZhuan = true;
		ax.father = this;
	}

	public void NotifyAxisReleased(SK_Universe_Aixs ax)
	{
		if ((bool)ax)
		{
			AxisList.Remove(ax);
		}
	}

	public void NotifyBallDespawn(SK_Universe_Ball ball)
	{
		if ((bool)ball)
		{
			BallList.Remove(ball);
		}
	}

	public void Stop()
	{
		if (!CanAT)
		{
			return;
		}
		CanAT = false;
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
