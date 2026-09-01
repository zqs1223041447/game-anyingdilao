using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class EmptyCOL : MonoBehaviour
{
	public float lifeTime;

	public bool CanMV;

	public float MoveSpeed;

	public float DotMulti;

	public bool Body;

	public float size;

	public GameObject FX;

	public CircleCollider2D col;

	public Dicform dic;

	private float timeA;

	public bool IsGround;

	private PlayerManager PL;

	private bool initialized;

	private void Awake()
	{
		col = GetComponent<CircleCollider2D>();
		dic = GetComponent<Dicform>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void OnEnable()
	{
		timeA = 0f;
		if ((bool)col)
		{
			col.enabled = false;
		}
		initialized = false;
	}

	private void Update()
	{
		if (CanMV)
		{
			base.transform.Translate(Vector2.right * (MoveSpeed * Time.deltaTime));
		}
		timeA += Time.deltaTime;
		if (timeA >= lifeTime)
		{
			timeA = 0f;
			FX = null;
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
		if (!col)
		{
			return;
		}
		if (!PL)
		{
			PL = SingletonMonoScope<PlayerManager>.Instance;
			if (!PL)
			{
				return;
			}
		}
		col.radius = size + size * PL.EXP_Range / 100f;
		col.enabled = true;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (Body)
		{
			if (collision.CompareTag("BodyCOL"))
			{
				BodyCOL component = collision.GetComponent<BodyCOL>();
				if ((bool)dic && (bool)dic.sp)
				{
					if (dic.sp.ZY)
					{
						if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !component.peo.em.IsJump && !component.peo.em.IsYS)
						{
							component.peo.EM_Set(dic.sp, DotMulti, dic.SubType, dic.sp.Dot_Infect, dic.sp.Dot_Infect_Layer, dic.UPDamage);
							if ((bool)FX && Random.Range(0, 100) < 50)
							{
								LeanPool.Spawn(FX, component.peo.em.yao.transform.position, Quaternion.identity, component.peo.em.yao.transform);
							}
						}
					}
					else
					{
						if (component.peo.CharacterType == 0 && component.peo.pl.IsAlive)
						{
							if (IsGround)
							{
								if (!component.peo.pl.NoGround)
								{
									component.peo.PL_Set(dic.sp, dic.SubType);
									if (FX != null && Random.Range(0, 100) < 50)
									{
										LeanPool.Spawn(FX, component.peo.pl.yao.transform.position, Quaternion.identity, component.peo.pl.yao.transform);
									}
								}
							}
							else
							{
								component.peo.PL_Set(dic.sp, dic.SubType);
								if (FX != null && Random.Range(0, 100) < 50)
								{
									LeanPool.Spawn(FX, component.peo.pl.yao.transform.position, Quaternion.identity, component.peo.pl.yao.transform);
								}
							}
						}
						if (component.peo.CharacterType == 1 && component.peo.cp.IsAlive)
						{
							if (IsGround)
							{
								if (!component.peo.pl.CPNoGround)
								{
									component.peo.CP_Set(dic.sp, dic.SubType);
									if (FX != null && Random.Range(0, 100) < 50)
									{
										LeanPool.Spawn(FX, component.peo.cp.yao.transform.position, Quaternion.identity, component.peo.cp.yao.transform);
									}
								}
							}
							else
							{
								component.peo.CP_Set(dic.sp, dic.SubType);
								if (FX != null && Random.Range(0, 100) < 50)
								{
									LeanPool.Spawn(FX, component.peo.cp.yao.transform.position, Quaternion.identity, component.peo.cp.yao.transform);
								}
							}
						}
					}
				}
			}
		}
		else if (collision.CompareTag("FootCOL"))
		{
			FootCOL component2 = collision.GetComponent<FootCOL>();
			if ((bool)dic && (bool)dic.sp)
			{
				if (dic.sp.ZY)
				{
					if (component2.peo.CharacterType == 2 && component2.peo.em.IsAlive && !component2.peo.em.IsJump && !component2.peo.em.IsYS)
					{
						component2.peo.EM_Set(dic.sp, DotMulti, dic.SubType, dic.sp.Dot_Infect, dic.sp.Dot_Infect_Layer, dic.UPDamage);
					}
				}
				else
				{
					if (component2.peo.CharacterType == 0 && component2.peo.pl.IsAlive)
					{
						component2.peo.PL_Set(dic.sp, dic.SubType);
					}
					if (component2.peo.CharacterType == 1 && component2.peo.cp.IsAlive)
					{
						component2.peo.CP_Set(dic.sp, dic.SubType);
					}
				}
			}
		}
		if (collision.CompareTag("Break"))
		{
			BreakOBJ component3 = collision.GetComponent<BreakOBJ>();
			if ((bool)component3)
			{
				component3.Break();
			}
		}
	}
}
