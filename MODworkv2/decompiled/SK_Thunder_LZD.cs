using Lean.Pool;
using UnityEngine;

public class SK_Thunder_LZD : MonoBehaviour
{
	private float time;

	[HideInInspector]
	public SK_Thunder_LZ LZ;

	[HideInInspector]
	public Transform parent;

	[HideInInspector]
	public BodyCOL col;

	public float size;

	[HideInInspector]
	public float range;

	public GameObject EXP;

	[HideInInspector]
	public int DotMulti;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public SK_XJ_lighting LT;

	[HideInInspector]
	public int type;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
	}

	private void OnEnable()
	{
		time = 0f;
	}

	private void OnDisable()
	{
		if ((bool)LZ)
		{
			LZ.Forget(this);
		}
		if ((bool)LT)
		{
			LT.Forget(this);
		}
		LT = null;
		LZ = null;
		col = null;
		parent = null;
		time = 0f;
	}

	private void Release()
	{
		if ((bool)LT)
		{
			LT.RefreshDian(this);
		}
		else if ((bool)LZ)
		{
			LZ.RefreshDian(this);
		}
		else
		{
			LeanPool.Despawn(this);
		}
	}

	private void Update()
	{
		if (!col || !parent)
		{
			return;
		}
		switch (type)
		{
		case 0:
		{
			Transform enemyYao2 = GetEnemyYao();
			if (!enemyYao2)
			{
				Release();
				break;
			}
			Vector3 vector2 = enemyYao2.position - parent.position;
			float z2 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
			base.transform.rotation = Quaternion.Euler(0f, 0f, z2);
			base.transform.position = new Vector3((enemyYao2.position.x + parent.position.x) / 2f, (enemyYao2.position.y + parent.position.y) / 2f, base.transform.position.z);
			base.transform.localScale = new Vector2(size * Vector2.Distance(enemyYao2.position, parent.position), 1f);
			if (Vector3.Distance(parent.position, col.transform.position) > range || !col.peo.pl.IsAlive)
			{
				Release();
				break;
			}
			time += Time.deltaTime;
			if (time >= 0.5f)
			{
				col.peo.PL_Set(dic.sp, dic.SubType);
				if (Random.Range(0, 101) < 40 && dic.sp.DamageA > 0f)
				{
					Dicform component3 = LeanPool.Spawn(EXP, col.peo.pl.yao.transform.position, Quaternion.identity, col.peo.pl.yao.transform).GetComponent<Dicform>();
					component3.sp = dic.sp;
					component3.SetCount(dic.sp.ZY);
					component3.SubType = 1;
					component3.Index += dic.Index + 1;
				}
				time = 0f;
			}
			break;
		}
		case 1:
		{
			Transform enemyYao3 = GetEnemyYao();
			if (!enemyYao3)
			{
				Release();
				break;
			}
			Vector3 vector3 = enemyYao3.position - parent.position;
			float z3 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
			base.transform.rotation = Quaternion.Euler(0f, 0f, z3);
			base.transform.position = new Vector3((enemyYao3.position.x + parent.position.x) / 2f, (enemyYao3.position.y + parent.position.y) / 2f, base.transform.position.z);
			base.transform.localScale = new Vector2(size * Vector2.Distance(enemyYao3.position, parent.position), 1f);
			if (Vector3.Distance(parent.position, col.transform.position) > range || !col.peo.cp.IsAlive)
			{
				Release();
				break;
			}
			time += Time.deltaTime;
			if (time >= 0.5f)
			{
				col.peo.CP_Set(dic.sp, dic.SubType);
				if (Random.Range(0, 101) < 40 && dic.sp.DamageA > 0f)
				{
					Dicform component4 = LeanPool.Spawn(EXP, col.peo.cp.yao.transform.position, Quaternion.identity, col.peo.cp.yao.transform).GetComponent<Dicform>();
					component4.sp = dic.sp;
					component4.SetCount(dic.sp.ZY);
					component4.SubType = 1;
					component4.Index += dic.Index + 1;
				}
				time = 0f;
			}
			break;
		}
		case 2:
		{
			Transform enemyYao = GetEnemyYao();
			if (!enemyYao)
			{
				Release();
				break;
			}
			Vector3 vector = enemyYao.position - parent.position;
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			base.transform.rotation = Quaternion.Euler(0f, 0f, z);
			base.transform.position = new Vector3((enemyYao.position.x + parent.position.x) / 2f, (enemyYao.position.y + parent.position.y) / 2f, base.transform.position.z);
			base.transform.localScale = new Vector2(size * Vector2.Distance(enemyYao.position, parent.position), 1f);
			if (Vector3.Distance(parent.position, col.transform.position) > range || !col.peo.em.IsAlive || col.peo.em.IsYS || col.peo.em.IsJump)
			{
				Release();
				break;
			}
			time += Time.deltaTime;
			if (!(time >= 0.5f))
			{
				break;
			}
			if ((bool)col?.peo && (bool)dic)
			{
				col.peo.EM_Set(dic.sp, DotMulti, dic.SubType, Dot_Infect: false, 0, dic.UPDamage);
			}
			if (Random.Range(0, 101) < 40)
			{
				if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && (bool)EXP)
				{
					Dicform component = LeanPool.Spawn(EXP, col.peo.em.yao.transform.position, Quaternion.identity, col.peo.em.yao.transform).GetComponent<Dicform>();
					component.sp = dic.sp;
					component.SetCount(dic.sp.ZY);
					component.SubType = 1;
					component.Index += dic.Index + 1;
				}
				if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && (bool)EXP)
				{
					Dicform component2 = LeanPool.Spawn(EXP, col.peo.em.yao.transform.position, Quaternion.identity, col.peo.em.yao.transform).GetComponent<Dicform>();
					component2.sp = dic.sp;
					component2.SetCount(dic.sp.ZY);
					component2.SubType = 1;
					component2.Index += dic.Index + 1;
				}
			}
			time = 0f;
			break;
		}
		}
	}

	private Transform GetEnemyYao()
	{
		if (!col)
		{
			return null;
		}
		if (!col.peo)
		{
			return null;
		}
		if (!col.peo.em)
		{
			return null;
		}
		if (!col.peo.em.yao)
		{
			return null;
		}
		return col.peo.em.yao.transform;
	}
}
