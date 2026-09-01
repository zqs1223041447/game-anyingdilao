using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class EmptyCOL_BF : MonoBehaviour
{
	public float lifeTime;

	public float size;

	public CircleCollider2D col;

	public Dicform dic;

	private PlayerManager PL;

	private float timeA;

	private bool initialized;

	private void Awake()
	{
		col = GetComponent<CircleCollider2D>();
		dic = GetComponent<Dicform>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		if ((bool)col)
		{
			col.enabled = false;
			if (!PL)
			{
				PL = SingletonMonoScope<PlayerManager>.Instance;
			}
			initialized = false;
		}
	}

	private void Update()
	{
		timeA += Time.deltaTime;
		if (timeA >= lifeTime)
		{
			timeA = 0f;
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
		if ((bool)col)
		{
			if (!PL)
			{
				PL = SingletonMonoScope<PlayerManager>.Instance;
			}
			col.radius = (PL ? (size + size * PL.Buff_Range / 100f) : size);
			col.enabled = true;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!collision.CompareTag("FootCOL"))
		{
			return;
		}
		FootCOL component = collision.GetComponent<FootCOL>();
		if (!dic || !dic.sp)
		{
			return;
		}
		if (dic.sp.ZY)
		{
			if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive)
			{
				Buff_PL buff_PL = new Buff_PL();
				buff_PL.type = 1;
				buff_PL.BuffTime = 1f;
				buff_PL.IsSkillBuff = true;
				buff_PL.IndexName = dic.sp.skillName;
				buff_PL.damageType = dic.sp.damageType;
				buff_PL.DotDamage = 0f;
				buff_PL.DotChuan = 0f;
				if (component.peo.BuffPL.HasSameBuff(dic.sp.skillName))
				{
					buff_PL.Damage = dic.sp.C_Damage / PL.TuT_PlayerAllLast / 2f + dic.sp.C_Damage / PL.TuT_PlayerAllLast / 2f * (float)PL.TuT_Buff / 100f;
					buff_PL.AttackSpeed = dic.sp.C_ATspeed / PL.TuT_PlayerAllLast / 2f + dic.sp.C_ATspeed / PL.TuT_PlayerAllLast / 2f * (float)PL.TuT_Buff / 100f;
					buff_PL.MoveSpeed = dic.sp.C_MVspeed / PL.TuT_PlayerAllLast / 2f + dic.sp.C_MVspeed / PL.TuT_PlayerAllLast / 2f * (float)PL.TuT_Buff / 100f;
					buff_PL.Health_Prc = dic.sp.C_Health_Prc / PL.TuT_PlayerAllLast / 2f + dic.sp.C_Health_Prc / PL.TuT_PlayerAllLast / 2f * (float)PL.TuT_Buff / 100f;
				}
				else
				{
					buff_PL.Damage = dic.sp.C_Damage / PL.TuT_PlayerAllLast + dic.sp.C_Damage / PL.TuT_PlayerAllLast * (float)PL.TuT_Buff / 100f;
					buff_PL.AttackSpeed = dic.sp.C_ATspeed / PL.TuT_PlayerAllLast + dic.sp.C_ATspeed / PL.TuT_PlayerAllLast * (float)PL.TuT_Buff / 100f;
					buff_PL.MoveSpeed = dic.sp.C_MVspeed / PL.TuT_PlayerAllLast + dic.sp.C_MVspeed / PL.TuT_PlayerAllLast * (float)PL.TuT_Buff / 100f;
					buff_PL.Health_Prc = dic.sp.C_Health_Prc / PL.TuT_PlayerAllLast + dic.sp.C_Health_Prc / PL.TuT_PlayerAllLast * (float)PL.TuT_Buff / 100f;
				}
				component.peo.BuffPL.AddBuff(buff_PL);
			}
			if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive)
			{
				Buff_CP buff_CP = new Buff_CP();
				buff_CP.type = 1;
				buff_CP.BuffTime = 1f;
				buff_CP.damageType = dic.sp.damageType;
				buff_CP.DotDamage = 0f;
				buff_CP.DotChuan = 0f;
				buff_CP.Damage = dic.sp.C_Damage + dic.sp.C_Damage * (float)PL.TuT_Buff / 100f;
				buff_CP.AttackSpeed = dic.sp.C_ATspeed + dic.sp.C_ATspeed * (float)PL.TuT_Buff / 100f;
				buff_CP.MoveSpeed = dic.sp.C_MVspeed + dic.sp.C_MVspeed * (float)PL.TuT_Buff / 100f;
				buff_CP.Health_Prc = dic.sp.C_Health_Prc + dic.sp.C_Health_Prc * (float)PL.TuT_Buff / 100f;
				component.peo.BuffCP.AddBuff(buff_CP);
			}
		}
		else if (component.peo.CharacterType == 2 && component.peo.em.IsAlive)
		{
			Buff_Enemy buff_Enemy = new Buff_Enemy();
			buff_Enemy.type = 1;
			buff_Enemy.BuffTime = 1f;
			buff_Enemy.damageType = dic.sp.damageType;
			buff_Enemy.Damage = dic.sp.C_Damage;
			buff_Enemy.Chuan = dic.sp.BF_EL_Chuan;
			buff_Enemy.Through = dic.sp.BF_Through;
			buff_Enemy.BJrate = dic.sp.BF_BJrate;
			buff_Enemy.GeDang = dic.sp.BF_GeDang;
			buff_Enemy.AttackSpeed = dic.sp.C_ATspeed;
			buff_Enemy.MoveSpeed = dic.sp.C_MVspeed;
			buff_Enemy.DamageAnti = dic.sp.BF_DamageAnti;
			buff_Enemy.Health_Prc = dic.sp.C_Health_Prc;
			component.peo.BuffEM.AddBuff(buff_Enemy);
		}
	}
}
