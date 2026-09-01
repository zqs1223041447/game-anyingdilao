using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class EnemyTower : MonoBehaviour
{
	public string SoundA;

	public float RDspeed;

	public float FStimeMin;

	public float FStimeMax;

	public int type;

	[HideInInspector]
	public Transform point;

	[HideInInspector]
	public Enemy em;

	[HideInInspector]
	public PlayerManager playerManager;

	[HideInInspector]
	public float JStime;

	[HideInInspector]
	public float JStimeA;

	public GameObject[] OBJ;

	private float ATtimeTmp;

	private bool StartOK;

	public void Awake()
	{
		em = GetComponent<Enemy>();
		point = base.transform.Find("main/point");
		playerManager = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void OnEnable()
	{
		JStimeA = 0f;
		JStime = 0f;
		StartOK = false;
		ATtimeTmp = Random.Range(FStimeMin, FStimeMax);
		this.wait(1E-06f, SetStart);
	}

	private void Update()
	{
		if (!StartOK)
		{
			return;
		}
		if (em.IsAlive)
		{
			JStimeA += Time.deltaTime;
			if (JStimeA >= 0.1f)
			{
				em.Fighting();
				JStimeA = 0f;
			}
			JStime += Time.deltaTime;
			if (!(JStime >= ATtimeTmp))
			{
				return;
			}
			if (em.hadTarget && playerManager.IsAlive)
			{
				switch (type)
				{
				case 0:
					Qiu();
					break;
				case 1:
					Pen();
					break;
				case 2:
					FB();
					break;
				}
			}
			ATtimeTmp = Random.Range(FStimeMin, FStimeMax);
			JStime = 0f;
		}
		else
		{
			StartOK = false;
			em.canvas.alpha = 0f;
		}
	}

	public void SetStart()
	{
		em.path.canMove = false;
		StartOK = true;
	}

	public void Qiu()
	{
		Dicform component = LeanPool.Spawn(OBJ[em.MainElement], base.transform.position, Quaternion.identity).GetComponent<Dicform>();
		component.sp = component.gameObject.AddComponent<SkillOBJ_DT_SP>();
		component.SubType = 0;
		component.Index = 0;
		if ((bool)em.MVTarget)
		{
			component.dic = em.MVTarget.position - base.transform.position;
		}
		component.sp.indexType = 2;
		component.sp.em = em;
		component.sp.ZY = false;
		component.sp.Dot_Infect = false;
		component.sp.Dot_Infect_Layer = 0;
		if ((bool)em.MVTarget)
		{
			component.sp.TargetPos = em.MVTarget.position;
			component.sp.dic = em.MVTarget.position - base.transform.position;
		}
		component.sp.skillName = "0";
		component.sp.damageType = em.MainELType;
		component.sp.MainEL = em.MainElement;
		component.sp.Distance = em.Range_Cur;
		component.sp.AttackType = true;
		component.sp.Damage = em.Damage_Last;
		component.sp.DamageA = 0f;
		component.sp.DamageB = 0f;
		component.sp.BJrate = 0f;
		component.sp.Through = 0f;
		component.sp.FlySpeed = 0f;
		component.sp.Chuan = 40f;
		component.sp.MoveSpeedCut = 0f;
		component.sp.AttackSpeedCut = 0f;
		component.sp.NoTime = 1;
		component.sp.BuffTime = 4f;
		component.sp.Layer_SubA = 0;
		component.sp.Layer_SubB = 0;
		float num = Dis(component);
		component.dic = new Vector2(component.dic.x + Random.Range(RDspeed, 0f - RDspeed), component.dic.y + Random.Range(RDspeed, 0f - RDspeed));
		if (num > 6f)
		{
			num = 6f;
		}
		component.speed = (num + Random.Range(RDspeed, 0f - RDspeed)) * 1.7f;
		component.sp.DotRate = 0f;
		component.sp.DotDamage = 0f;
	}

	public void Pen()
	{
		Dicform component = LeanPool.Spawn(OBJ[em.MainElement], point.position, Quaternion.identity).GetComponent<Dicform>();
		component.sp = component.gameObject.AddComponent<SkillOBJ_DT_SP>();
		component.SubType = 0;
		component.Index = 0;
		if ((bool)em.MVTarget)
		{
			component.dic = em.MVTarget.position - base.transform.position;
		}
		component.sp.indexType = 2;
		component.sp.em = em;
		component.sp.ZY = false;
		component.sp.Dot_Infect = false;
		component.sp.Dot_Infect_Layer = 0;
		component.sp.TargetPos = em.MVTarget.position;
		component.sp.skillName = "0";
		component.sp.dic = em.MVTarget.position - base.transform.position;
		component.sp.damageType = em.MainELType;
		component.sp.MainEL = em.MainElement;
		component.sp.Distance = em.Range_Cur;
		component.sp.AttackType = true;
		component.sp.Damage = em.Damage_Last / 8f;
		component.sp.DamageA = 0f;
		component.sp.DamageB = 0f;
		component.sp.BJrate = 0f;
		component.sp.Through = 0f;
		component.sp.FlySpeed = 0f;
		component.sp.Chuan = 40f;
		component.sp.MoveSpeedCut = 0f;
		component.sp.AttackSpeedCut = 0f;
		component.sp.NoTime = 1;
		component.sp.BuffTime = 4f;
		component.sp.Layer_SubA = 0;
		component.sp.Layer_SubB = 0;
		component.sp.DotRate = 0f;
		component.sp.DotDamage = 0f;
	}

	public void FB()
	{
		for (int i = 0; i < 16; i++)
		{
			Dicform component = LeanPool.Spawn(OBJ[em.MainElement], point.transform.position, Quaternion.Euler(0f, 0f, 22.5f * (float)(i + 1))).GetComponent<Dicform>();
			component.sp = component.gameObject.AddComponent<SkillOBJ_DT_SP>();
			component.SubType = 0;
			component.Index = 0;
			component.sp.indexType = 2;
			component.sp.em = em;
			component.sp.ZY = false;
			component.sp.Dot_Infect = false;
			component.sp.Dot_Infect_Layer = 0;
			if ((bool)em && (bool)em.MVTarget)
			{
				component.sp.TargetPos = em.MVTarget.position;
				component.dic = em.MVTarget.position - base.transform.position;
				component.sp.dic = em.MVTarget.position - base.transform.position;
			}
			component.sp.skillName = "0";
			component.sp.damageType = em.MainELType;
			component.sp.MainEL = em.MainElement;
			component.sp.Distance = em.Range_Cur;
			component.sp.AttackType = true;
			component.sp.Damage = em.Damage_Last / 6f;
			component.sp.DamageA = 0f;
			component.sp.DamageB = 0f;
			component.sp.BJrate = 0f;
			component.sp.Through = 0f;
			component.sp.FlySpeed = 0f;
			component.sp.Chuan = 40f;
			component.sp.MoveSpeedCut = 0f;
			component.sp.AttackSpeedCut = 0f;
			component.sp.NoTime = 1;
			component.sp.BuffTime = 4f;
			component.sp.DebuffTime = 3f;
			component.sp.Layer_SubA = 0;
			component.sp.Layer_SubB = 0;
			component.sp.DotRate = 30f;
			component.sp.DotDamage = em.Damage_Last / 10f;
			component.sp.Count_S = 0;
			component.sp.HasFX = 0;
			component.sp.colEXP = 1;
			component.sp.AllChuan_F = 0;
		}
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, point.position);
		}
	}

	public float Dis(Dicform dd)
	{
		float num = Vector2.Distance(dd.sp.TargetPos, base.transform.position);
		if (num < dd.sp.Distance)
		{
			return num;
		}
		return dd.sp.Distance;
	}
}
