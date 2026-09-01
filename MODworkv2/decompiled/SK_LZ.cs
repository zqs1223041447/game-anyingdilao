using System.Collections.Generic;
using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class SK_LZ : MonoBehaviour
{
	public int type;

	public GameObject TD;

	public GameObject FX;

	[Header("=========")]
	public int DotMulti;

	[HideInInspector]
	public Transform qiu;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public float timeA;

	[HideInInspector]
	public float timeB;

	private bool CanAT;

	private bool IsStopping;

	private float SoundFloat;

	public List<SK_LZD> dian = new List<SK_LZD>();

	public List<BodyCOL> FW = new List<BodyCOL>();

	public List<BodyCOL> AT = new List<BodyCOL>();

	[HideInInspector]
	public StudioEventEmitter Emitter;

	public Collider2D[] hitEM = new Collider2D[10];

	public Collider2D[] hitCP = new Collider2D[5];

	public Collider2D[] hitPL = new Collider2D[1];

	private float Min;

	private float Max;

	private float SpeedTmp;

	private float BlockFlyTime;

	private int CountMAX;

	private float tmp;

	private bool initialized;

	public bool CanKeepDian
	{
		get
		{
			if (CanAT && !IsStopping)
			{
				return base.isActiveAndEnabled;
			}
			return false;
		}
	}

	private void Awake()
	{
		dic = GetComponent<Dicform>();
		qiu = base.transform.Find("pivit/qiu");
		Emitter = GetComponent<StudioEventEmitter>();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		for (int i = 0; i < hitEM.Length; i++)
		{
			hitEM[i] = null;
		}
		for (int j = 0; j < hitCP.Length; j++)
		{
			hitCP[j] = null;
		}
		hitPL[0] = null;
		SpeedTmp = 0f;
		BlockFlyTime = Time.time + 0.1f;
		timeA = 0f;
		timeB = 0f;
		CanAT = false;
		IsStopping = false;
		SoundFloat = 0f;
		initialized = false;
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
		CountMAX = dic.sp.Count_ATtarget;
		Min = dic.sp.Range1;
		Max = dic.sp.Range2;
		CanAT = true;
		tmp = AT.Count;
		SpeedTmp = dic.sp.Speed1;
		Refresh();
	}

	private void Update()
	{
		if (CanAT)
		{
			base.transform.Translate(Vector2.right * (SpeedTmp * Time.deltaTime));
			timeA += Time.deltaTime;
			if (timeA >= dic.sp.BuffTime)
			{
				Stop();
				return;
			}
			timeB += Time.deltaTime;
			if (timeB >= 0.17f)
			{
				timeB = 0f;
				Refresh();
			}
		}
		if (AT.Count != 0)
		{
			if (AT.Count > 9)
			{
				SoundFloat = 1f;
			}
			else
			{
				SoundFloat = tmp / 9f;
			}
		}
		else
		{
			SoundFloat = 0f;
		}
		Emitter.SetParameter("CountAT", SoundFloat);
	}

	public void Del(SK_LZD lzd)
	{
		if ((bool)lzd)
		{
			if ((bool)lzd.col)
			{
				AT.Remove(lzd.col);
				FW.Remove(lzd.col);
			}
			DespawnDian(lzd);
			if (CanKeepDian)
			{
				Refresh();
			}
		}
	}

	public void Forget(SK_LZD lzd)
	{
		if ((bool)lzd)
		{
			dian.Remove(lzd);
			if ((bool)lzd.col)
			{
				AT.Remove(lzd.col);
				FW.Remove(lzd.col);
			}
		}
	}

	public void Stop()
	{
		if (IsStopping)
		{
			return;
		}
		IsStopping = true;
		CanAT = false;
		SpeedTmp = 0f;
		FW.Clear();
		AT.Clear();
		ClearDian();
		SoundFloat = 0f;
		if ((bool)Emitter)
		{
			Emitter.SetParameter("CountAT", SoundFloat);
		}
		if ((bool)FX)
		{
			LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
		}
		this.wait(0.2f, delegate
		{
			if (IsStopping)
			{
				LeanPool.Despawn(this);
			}
		});
	}

	private void OnDisable()
	{
		CanAT = false;
		SpeedTmp = 0f;
		FW.Clear();
		AT.Clear();
		ClearDian();
	}

	public float GetATtarDamageMultiplier()
	{
		if (!dic || dic.sp == null || dic.sp.ATtar_DMG <= 0)
		{
			return 1f;
		}
		return 1f + (float)(Mathf.Max(0, AT.Count) * dic.sp.ATtar_DMG) / 100f;
	}

	private void ClearDian()
	{
		for (int num = dian.Count - 1; num >= 0; num--)
		{
			DespawnDian(dian[num]);
		}
		dian.Clear();
	}

	private void DespawnDian(SK_LZD lzd)
	{
		if ((bool)lzd)
		{
			dian.Remove(lzd);
			lzd.LZ = null;
			lzd.col = null;
			lzd.parent = null;
			LeanPool.Despawn(lzd);
		}
	}

	private void DespawnDianFor(BodyCOL col)
	{
		for (int num = dian.Count - 1; num >= 0; num--)
		{
			if ((bool)dian[num] && dian[num].col == col)
			{
				DespawnDian(dian[num]);
			}
		}
	}

	private Vector3 GetTargetPosition(BodyCOL col)
	{
		if (!col || !col.peo)
		{
			return base.transform.position;
		}
		switch (col.peo.CharacterType)
		{
		case 0:
			if (col.peo.pl != null && col.peo.pl.yao != null)
			{
				return col.peo.pl.yao.transform.position;
			}
			break;
		case 1:
			if (col.peo.cp != null && col.peo.cp.yao != null)
			{
				return col.peo.cp.yao.transform.position;
			}
			break;
		case 2:
			if (col.peo.em != null && col.peo.em.yao != null)
			{
				return col.peo.em.yao.transform.position;
			}
			break;
		}
		return col.transform.position;
	}

	private bool IsOutOfMaxRange(BodyCOL col)
	{
		if (!col || !col.peo)
		{
			return true;
		}
		return (GetTargetPosition(col) - base.transform.position).sqrMagnitude > Max * Max;
	}

	private int CompareDistance(BodyCOL t1, BodyCOL t2)
	{
		float sqrMagnitude = (GetTargetPosition(t1) - base.transform.position).sqrMagnitude;
		float sqrMagnitude2 = (GetTargetPosition(t2) - base.transform.position).sqrMagnitude;
		return sqrMagnitude.CompareTo(sqrMagnitude2);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("DoomBall"))
		{
			SK_Doom_Ball component = collision.GetComponent<SK_Doom_Ball>();
			if (dic.sp.ZY)
			{
				component.SetHit(dic, base.transform.right);
			}
		}
		if (collision.CompareTag("blockFLY") && Time.time >= BlockFlyTime)
		{
			SpeedTmp = 0f;
		}
	}

	public void Refresh()
	{
		if (!CanKeepDian)
		{
			return;
		}
		if (dic.sp.ZY)
		{
			int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, Min, hitEM, LayerMask.GetMask("BodyCOLem"));
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					BodyCOL component = hitEM[i].GetComponent<BodyCOL>();
					if ((bool)component)
					{
						if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !AT.Contains(component) && !FW.Contains(component) && !component.peo.em.IsJump && !component.peo.em.IsYS)
						{
							FW.Add(component);
						}
						hitEM[i] = null;
					}
				}
			}
		}
		else
		{
			int num2 = Physics2D.OverlapCircleNonAlloc(base.transform.position, Min, hitCP, LayerMask.GetMask("BodyCOLcp"));
			if (num2 > 0)
			{
				for (int j = 0; j < num2; j++)
				{
					BodyCOL component2 = hitCP[j].GetComponent<BodyCOL>();
					if ((bool)component2)
					{
						if (component2.peo.CharacterType == 1 && component2.peo.cp.IsAlive && !AT.Contains(component2) && !FW.Contains(component2))
						{
							FW.Add(component2);
						}
						hitCP[j] = null;
					}
				}
			}
			int num3 = Physics2D.OverlapCircleNonAlloc(base.transform.position, Min, hitPL, LayerMask.GetMask("BodyCOLpl"));
			if (num3 > 0)
			{
				for (int k = 0; k < num3; k++)
				{
					BodyCOL component3 = hitPL[k].GetComponent<BodyCOL>();
					if ((bool)component3)
					{
						if (component3.peo.CharacterType == 0 && component3.peo.pl.IsAlive && !AT.Contains(component3) && !FW.Contains(component3))
						{
							FW.Add(component3);
						}
						hitPL[k] = null;
					}
				}
			}
		}
		if (dic.sp.ZY)
		{
			if (AT.Count > 0)
			{
				for (int l = 0; l < AT.Count; l++)
				{
					if (!AT[l].peo.em.IsAlive || AT[l].peo.em.IsYS || AT[l].peo.em.IsJump || IsOutOfMaxRange(AT[l]))
					{
						DespawnDianFor(AT[l]);
						AT.Remove(AT[l]);
						l--;
					}
				}
				AT.Sort((BodyCOL t1, BodyCOL t2) => CompareDistance(t1, t2));
			}
			if (FW.Count > 0)
			{
				for (int m = 0; m < FW.Count; m++)
				{
					if (!FW[m].peo.em.IsAlive || FW[m].peo.em.IsYS || FW[m].peo.em.IsJump || IsOutOfMaxRange(FW[m]))
					{
						DespawnDianFor(FW[m]);
						FW.Remove(FW[m]);
						m--;
					}
				}
				FW.Sort((BodyCOL t1, BodyCOL t2) => CompareDistance(t1, t2));
			}
			if (FW.Count > 0 && AT.Count < CountMAX)
			{
				int num4 = ((FW.Count + AT.Count > CountMAX) ? (CountMAX - AT.Count) : ((FW.Count <= CountMAX) ? FW.Count : (CountMAX - AT.Count)));
				for (int n = 0; n < num4; n++)
				{
					BodyCOL bodyCOL = FW[0];
					AT.Add(bodyCOL);
					FW.Remove(bodyCOL);
					SK_LZD component4 = LeanPool.Spawn(TD, bodyCOL.peo.em.yao.transform.position, Quaternion.identity, bodyCOL.peo.em.yao.transform).GetComponent<SK_LZD>();
					Dicform component5 = component4.GetComponent<Dicform>();
					component5.sp = dic.sp;
					component5.SetCount(dic.sp.ZY);
					component5.SubType = dic.SubType;
					component5.Index = dic.Index;
					component4.gameObject.SetActive(value: false);
					component4.LZ = this;
					component4.col = bodyCOL;
					component4.parent = qiu.transform;
					component4.range = Max;
					component4.type = 2;
					component4.DotMulti = DotMulti;
					dian.Add(component4);
					Transform transform = component4.col.peo.em.yao.transform;
					Vector3 vector = transform.position - base.transform.position;
					float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
					component4.transform.rotation = Quaternion.Euler(0f, 0f, z);
					component4.transform.position = new Vector2((transform.position.x + base.transform.position.x) / 2f, (transform.position.y + base.transform.position.y) / 2f);
					component4.transform.localScale = new Vector2(component4.size * Vector2.Distance(transform.position, base.transform.position), 1f);
					component4.gameObject.SetActive(value: true);
				}
			}
			return;
		}
		if (AT.Count > 0)
		{
			for (int num5 = 0; num5 < AT.Count; num5++)
			{
				if (AT[num5].peo.CharacterType == 1)
				{
					if (!AT[num5].peo.cp.IsAlive || IsOutOfMaxRange(AT[num5]))
					{
						DespawnDianFor(AT[num5]);
						AT.Remove(AT[num5]);
						num5--;
					}
				}
				else if (!AT[num5].peo.pl.IsAlive || IsOutOfMaxRange(AT[num5]))
				{
					DespawnDianFor(AT[num5]);
					AT.Remove(AT[num5]);
					num5--;
				}
			}
			AT.Sort((BodyCOL t1, BodyCOL t2) => CompareDistance(t1, t2));
		}
		if (FW.Count > 0)
		{
			for (int num6 = 0; num6 < FW.Count; num6++)
			{
				if (FW[num6].peo.CharacterType == 1)
				{
					if (!FW[num6].peo.cp.IsAlive || IsOutOfMaxRange(FW[num6]))
					{
						DespawnDianFor(FW[num6]);
						FW.Remove(FW[num6]);
						num6--;
					}
				}
				else if (!FW[num6].peo.pl.IsAlive || IsOutOfMaxRange(FW[num6]))
				{
					DespawnDianFor(FW[num6]);
					FW.Remove(FW[num6]);
					num6--;
				}
			}
			FW.Sort((BodyCOL t1, BodyCOL t2) => CompareDistance(t1, t2));
		}
		if (FW.Count <= 0 || AT.Count >= CountMAX)
		{
			return;
		}
		int num7 = ((FW.Count + AT.Count > CountMAX) ? (CountMAX - AT.Count) : ((FW.Count <= CountMAX) ? FW.Count : (CountMAX - AT.Count)));
		for (int num8 = 0; num8 < num7; num8++)
		{
			BodyCOL bodyCOL2 = FW[0];
			AT.Add(bodyCOL2);
			FW.Remove(bodyCOL2);
			SK_LZD component6 = LeanPool.Spawn(TD, base.transform.position, Quaternion.identity).GetComponent<SK_LZD>();
			Dicform component7 = component6.GetComponent<Dicform>();
			component7.sp = dic.sp;
			component7.SetCount(dic.sp.ZY);
			component7.SubType = dic.SubType;
			component7.Index = dic.Index;
			component6.gameObject.SetActive(value: false);
			component6.LZ = this;
			component6.col = bodyCOL2;
			component6.parent = qiu.transform;
			component6.range = Max;
			component6.DotMulti = DotMulti;
			component6.type = bodyCOL2.peo.CharacterType;
			dian.Add(component6);
			Transform transform2;
			switch (bodyCOL2.peo.CharacterType)
			{
			case 0:
				component6.transform.SetParent(component6.col.peo.pl.yao.transform);
				transform2 = component6.col.peo.pl.yao.transform;
				break;
			case 1:
				component6.transform.SetParent(component6.col.peo.cp.yao.transform);
				transform2 = component6.col.peo.cp.yao.transform;
				break;
			default:
				component6.transform.SetParent(component6.col.peo.cp.yao.transform);
				transform2 = component6.col.peo.cp.yao.transform;
				break;
			}
			Vector3 vector2 = transform2.position - base.transform.position;
			float z2 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
			component6.transform.rotation = Quaternion.Euler(0f, 0f, z2);
			component6.transform.position = new Vector2((transform2.position.x + base.transform.position.x) / 2f, (transform2.position.y + base.transform.position.y) / 2f);
			component6.transform.localScale = new Vector2(component6.size * Vector2.Distance(transform2.position, base.transform.position), 1f);
			component6.gameObject.SetActive(value: true);
		}
	}
}
