using Lean.Pool;
using UnityEngine;

public class SK_LZD : MonoBehaviour
{
	public float size;

	public GameObject EXP;

	private float timeA;

	[HideInInspector]
	public SK_LZ LZ;

	[HideInInspector]
	public Transform parent;

	[HideInInspector]
	public BodyCOL col;

	[HideInInspector]
	public float range;

	[HideInInspector]
	public int DotMulti;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public int type;

	private bool CanAT;

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
	}

	private void OnDisable()
	{
		if ((bool)LZ)
		{
			LZ.Forget(this);
		}
		LZ = null;
		parent = null;
		col = null;
		timeA = 0f;
	}

	private void Release()
	{
		if ((bool)LZ)
		{
			LZ.Del(this);
		}
		else
		{
			LeanPool.Despawn(this);
		}
	}

	private Transform GetTargetYao()
	{
		if (!col || !col.peo)
		{
			return null;
		}
		switch (type)
		{
		case 0:
			if (!(col.peo.pl != null) || !(col.peo.pl.yao != null))
			{
				return null;
			}
			return col.peo.pl.yao.transform;
		case 1:
			if (!(col.peo.cp != null) || !(col.peo.cp.yao != null))
			{
				return null;
			}
			return col.peo.cp.yao.transform;
		case 2:
			if (!(col.peo.em != null) || !(col.peo.em.yao != null))
			{
				return null;
			}
			return col.peo.em.yao.transform;
		default:
			return null;
		}
	}

	private bool TargetInvalid()
	{
		if (!col || !col.peo)
		{
			return true;
		}
		switch (type)
		{
		case 0:
			if (!(col.peo.pl == null))
			{
				return !col.peo.pl.IsAlive;
			}
			return true;
		case 1:
			if (!(col.peo.cp == null))
			{
				return !col.peo.cp.IsAlive;
			}
			return true;
		case 2:
			if (!(col.peo.em == null) && col.peo.em.IsAlive && !col.peo.em.IsYS)
			{
				return col.peo.em.IsJump;
			}
			return true;
		default:
			return true;
		}
	}

	private bool TargetOutOfRange(Vector3 targetPosition)
	{
		return (targetPosition - parent.position).sqrMagnitude > range * range;
	}

	private void ApplyEnemyDamage()
	{
		if (!dic || dic.sp == null)
		{
			return;
		}
		float num = (LZ ? LZ.GetATtarDamageMultiplier() : 1f);
		if (num <= 1f)
		{
			col.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
			return;
		}
		float damage = dic.sp.Damage;
		float damageA = dic.sp.DamageA;
		float damageB = dic.sp.DamageB;
		dic.sp.Damage *= num;
		dic.sp.DamageA *= num;
		dic.sp.DamageB *= num;
		try
		{
			col.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
		}
		finally
		{
			dic.sp.Damage = damage;
			dic.sp.DamageA = damageA;
			dic.sp.DamageB = damageB;
		}
	}

	private void Update()
	{
		if (!LZ || !LZ.CanKeepDian)
		{
			LeanPool.Despawn(this);
		}
		else if ((bool)col && (bool)parent)
		{
			switch (type)
			{
			case 0:
			{
				Transform targetYao3 = GetTargetYao();
				if (!targetYao3 || TargetInvalid() || TargetOutOfRange(targetYao3.position))
				{
					Release();
					break;
				}
				Vector3 vector3 = targetYao3.position - parent.position;
				float z3 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
				base.transform.rotation = Quaternion.Euler(0f, 0f, z3);
				base.transform.position = new Vector2((targetYao3.position.x + parent.position.x) / 2f, (targetYao3.position.y + parent.position.y) / 2f);
				base.transform.localScale = new Vector2(size * Vector2.Distance(targetYao3.position, parent.position), 1f);
				timeA += Time.deltaTime;
				if (!(timeA >= 0.5f))
				{
					break;
				}
				col.peo.PL_Set(dic.sp, dic.SubType);
				if (Random.Range(0, 101) < 40)
				{
					if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && (bool)EXP)
					{
						Dicform component5 = LeanPool.Spawn(EXP, targetYao3.position, Quaternion.identity, targetYao3).GetComponent<Dicform>();
						component5.sp = dic.sp;
						component5.SetCount(dic.sp.ZY);
						component5.SubType = 1;
						component5.Index += dic.Index + 1;
					}
					if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && (bool)EXP)
					{
						Dicform component6 = LeanPool.Spawn(EXP, targetYao3.position, Quaternion.identity, targetYao3).GetComponent<Dicform>();
						component6.sp = dic.sp;
						component6.SetCount(dic.sp.ZY);
						component6.SubType = 2;
						component6.Index += dic.Index + 1;
					}
				}
				timeA = 0f;
				break;
			}
			case 1:
			{
				Transform targetYao2 = GetTargetYao();
				if (!targetYao2 || TargetInvalid() || TargetOutOfRange(targetYao2.position))
				{
					Release();
					break;
				}
				Vector3 vector2 = targetYao2.position - parent.position;
				float z2 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
				base.transform.rotation = Quaternion.Euler(0f, 0f, z2);
				base.transform.position = new Vector2((targetYao2.position.x + parent.position.x) / 2f, (targetYao2.position.y + parent.position.y) / 2f);
				base.transform.localScale = new Vector2(size * Vector2.Distance(targetYao2.position, parent.position), 1f);
				timeA += Time.deltaTime;
				if (!(timeA >= 0.5f))
				{
					break;
				}
				col.peo.CP_Set(dic.sp, dic.SubType);
				if (Random.Range(0, 101) < 40)
				{
					if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && (bool)EXP)
					{
						Dicform component3 = LeanPool.Spawn(EXP, targetYao2.position, Quaternion.identity, targetYao2).GetComponent<Dicform>();
						component3.sp = dic.sp;
						component3.SetCount(dic.sp.ZY);
						component3.SubType = 1;
						component3.Index += dic.Index + 1;
					}
					if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && (bool)EXP)
					{
						Dicform component4 = LeanPool.Spawn(EXP, targetYao2.position, Quaternion.identity, targetYao2).GetComponent<Dicform>();
						component4.sp = dic.sp;
						component4.SetCount(dic.sp.ZY);
						component4.SubType = 2;
						component4.Index += dic.Index + 1;
					}
				}
				timeA = 0f;
				break;
			}
			case 2:
			{
				Transform targetYao = GetTargetYao();
				if (!targetYao || TargetInvalid() || TargetOutOfRange(targetYao.position))
				{
					Release();
					break;
				}
				Vector3 vector = targetYao.position - parent.position;
				float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
				base.transform.rotation = Quaternion.Euler(0f, 0f, z);
				base.transform.position = new Vector2((targetYao.position.x + parent.position.x) / 2f, (targetYao.position.y + parent.position.y) / 2f);
				base.transform.localScale = new Vector2(size * Vector2.Distance(targetYao.position, parent.position), 1f);
				timeA += Time.deltaTime;
				if (!(timeA >= 0.5f))
				{
					break;
				}
				ApplyEnemyDamage();
				if (Random.Range(0, 101) < 40)
				{
					if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && (bool)EXP)
					{
						Dicform component = LeanPool.Spawn(EXP, targetYao.position, Quaternion.identity, targetYao).GetComponent<Dicform>();
						component.sp = dic.sp;
						component.SetCount(dic.sp.ZY);
						component.SubType = 1;
						component.Index += dic.Index + 1;
					}
					if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && (bool)EXP)
					{
						Dicform component2 = LeanPool.Spawn(EXP, targetYao.position, Quaternion.identity, targetYao).GetComponent<Dicform>();
						component2.sp = dic.sp;
						component2.SetCount(dic.sp.ZY);
						component2.SubType = 2;
						component2.Index += dic.Index + 1;
					}
				}
				timeA = 0f;
				break;
			}
			}
		}
		else
		{
			Release();
		}
	}
}
