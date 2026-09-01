using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Strom : MonoBehaviour
{
	public GameObject OBJ;

	public ParticleSystem[] par;

	public float LifeTime;

	public float starFollowTime;

	public float speed;

	public float size;

	public float DotMulti;

	public float FStime;

	[Header("=========")]
	public float colSizeMAX;

	public float colSizeMin;

	[HideInInspector]
	public bool Follow;

	[HideInInspector]
	public Dicform dic;

	[HideInInspector]
	public List<FootCOL> tarList = new List<FootCOL>();

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeE;

	private Transform target;

	private bool CanAT;

	private float speedTMP;

	private bool hasTarget;

	private bool canFollow;

	private bool startFollow;

	public Collider2D[] hitEM = new Collider2D[5];

	public Collider2D[] hitCP = new Collider2D[3];

	public Collider2D[] hitPL = new Collider2D[1];

	private float rangeTmp;

	private bool initialized;

	public bool stopMove => timeB >= LifeTime - 1f;

	private void Awake()
	{
		dic = GetComponent<Dicform>();
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
		hasTarget = false;
		startFollow = false;
		canFollow = false;
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeE = 0f;
		CanAT = false;
		speedTMP = speed;
		rangeTmp = colSizeMin;
		tarList.Clear();
		if (par.Length != 0)
		{
			ParticleSystem[] array = par;
			foreach (ParticleSystem obj in array)
			{
				obj.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
				ParticleSystem.MainModule main = obj.main;
				main.duration = LifeTime;
				obj.Play();
			}
		}
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
		if (dic.Index == 0)
		{
			if (dic.sp.Follow_F == 0)
			{
				Follow = true;
			}
			else
			{
				Follow = false;
			}
		}
		else if (dic.sp.Follow_S == 0)
		{
			Follow = true;
		}
		else
		{
			Follow = false;
		}
		CanAT = true;
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance)
		{
			return;
		}
		if (CanAT)
		{
			if (!Follow)
			{
				base.transform.Translate(dic.dic.normalized * (speedTMP * Time.deltaTime));
			}
			else if (canFollow)
			{
				if (!startFollow)
				{
					base.transform.Translate(dic.dic.normalized * (speedTMP * Time.deltaTime));
					if (TrySetFirstTarget())
					{
						startFollow = true;
						hasTarget = true;
					}
					else if (stopMove)
					{
						dic.dic = Vector2.zero;
						hasTarget = false;
					}
				}
				else
				{
					if (TrySetFirstTarget())
					{
						startFollow = true;
						hasTarget = true;
					}
					if (hasTarget && (bool)target)
					{
						if (Vector2.Distance(base.transform.position, target.position) < 0.2f)
						{
							speedTMP = speed / 6f;
						}
						else
						{
							speedTMP = speed;
						}
						base.transform.Translate((target.position - base.transform.position).normalized * (speedTMP * Time.deltaTime));
					}
					else
					{
						hasTarget = false;
						base.transform.Translate(dic.dic.normalized * (speedTMP * Time.deltaTime));
					}
				}
			}
			else
			{
				base.transform.Translate(dic.dic.normalized * (speedTMP * Time.deltaTime));
			}
			timeA += Time.deltaTime;
			if (timeA >= 0.5f)
			{
				Refresh();
				if (rangeTmp < colSizeMAX)
				{
					rangeTmp += 0.5f;
				}
				Fashe();
				timeA = 0f;
			}
			if (dic.Index == 0)
			{
				timeE += Time.deltaTime;
				if (timeE >= FStime)
				{
					if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && (bool)OBJ)
					{
						Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
						component.sp = dic.sp;
						component.SetCount(dic.sp.ZY);
						component.SubType = 1;
						component.dic = new Vector2(Random.Range(1f, -1f), Random.Range(1f, -1f));
						component.Index = dic.Index + 1;
					}
					timeE = 0f;
				}
			}
			if (!canFollow)
			{
				timeC += Time.deltaTime;
				if (timeC >= starFollowTime)
				{
					canFollow = true;
					timeC = 0f;
				}
			}
		}
		timeB += Time.deltaTime;
		if (timeB >= LifeTime)
		{
			timeB = 0f;
			Stop();
		}
	}

	public void Fashe()
	{
		if (SingletonMonoScope<GameDataManager>.HasInstance)
		{
			EmptyCOL component = LeanPool.Spawn(SingletonMonoScope<GameDataManager>.Instance.SKPB.EmptyCol, base.transform.position, Quaternion.identity).GetComponent<EmptyCOL>();
			Dicform component2 = component.GetComponent<Dicform>();
			component2.sp = dic.sp;
			component2.SetCount(dic.sp.ZY);
			component2.SubType = dic.SubType;
			component2.Index = dic.Index;
			component.size = size;
			component.Body = false;
			component.DotMulti = DotMulti;
			component.lifeTime = 0.1f;
			component.IsGround = false;
		}
	}

	public void Stop()
	{
		CanAT = false;
		speedTMP = 0f;
		if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f)
		{
			LittleStrom(OBJ);
		}
		tarList.Clear();
		this.wait(1f, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	public void BlockStop()
	{
		speedTMP = 0f;
	}

	public void LittleStrom(GameObject obj)
	{
		Dicform component = LeanPool.Spawn(obj, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
		component.sp = dic.sp;
		component.SetCount(dic.sp.ZY);
		component.SubType = 2;
		component.dic = Vector2.left;
		component.Index = dic.Index + 1;
		Dicform component2 = LeanPool.Spawn(obj, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
		component2.sp = dic.sp;
		component2.SetCount(dic.sp.ZY);
		component2.SubType = 2;
		component2.dic = Vector2.right;
		component.Index = dic.Index + 1;
		Dicform component3 = LeanPool.Spawn(obj, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
		component3.sp = dic.sp;
		component3.SetCount(dic.sp.ZY);
		component3.SubType = 2;
		component3.dic = Vector2.up;
		component.Index = dic.Index + 1;
		Dicform component4 = LeanPool.Spawn(obj, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
		component4.sp = dic.sp;
		component4.SetCount(dic.sp.ZY);
		component4.SubType = 2;
		component4.dic = Vector2.down;
		component.Index = dic.Index + 1;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Break"))
		{
			collision.GetComponent<BreakOBJ>().Break();
		}
		if (collision.CompareTag("blockFLY"))
		{
			BlockStop();
		}
	}

	public void Refresh()
	{
		Vector3 position;
		if (dic.sp.ZY)
		{
			int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, rangeTmp, hitEM, LayerMask.GetMask("FootCOLem"));
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					FootCOL component = hitEM[i].GetComponent<FootCOL>();
					if ((bool)component)
					{
						if (IsValidTarget(component, out position) && component.peo.CharacterType == 2 && !tarList.Contains(component) && tarList.Count < 6)
						{
							tarList.Add(component);
						}
						hitEM[i] = null;
					}
				}
			}
		}
		else
		{
			int num2 = Physics2D.OverlapCircleNonAlloc(base.transform.position, rangeTmp, hitCP, LayerMask.GetMask("FootCOLcp"));
			if (num2 > 0)
			{
				for (int j = 0; j < num2; j++)
				{
					FootCOL component2 = hitCP[j].GetComponent<FootCOL>();
					if ((bool)component2)
					{
						if (IsValidTarget(component2, out position) && component2.peo.CharacterType == 1 && !tarList.Contains(component2))
						{
							tarList.Add(component2);
						}
						hitCP[j] = null;
					}
				}
			}
			int num3 = Physics2D.OverlapCircleNonAlloc(base.transform.position, rangeTmp, hitPL, LayerMask.GetMask("FootCOLpl"));
			if (num3 > 0)
			{
				for (int k = 0; k < num3; k++)
				{
					FootCOL component3 = hitPL[k].GetComponent<FootCOL>();
					if ((bool)component3)
					{
						if (IsValidTarget(component3, out position) && component3.peo.CharacterType == 0 && !tarList.Contains(component3))
						{
							tarList.Add(component3);
						}
						hitPL[k] = null;
					}
				}
			}
		}
		if (dic.sp.ZY)
		{
			for (int l = 0; l < tarList.Count; l++)
			{
				if (!IsValidTarget(tarList[l], out var position2) || Vector3.Distance(position2, base.transform.position) > colSizeMAX)
				{
					tarList.Remove(tarList[l]);
					l--;
				}
			}
			tarList.Sort(CompareTargetDistance);
			return;
		}
		for (int m = 0; m < tarList.Count; m++)
		{
			if (!IsValidTarget(tarList[m], out var position3) || Vector3.Distance(position3, base.transform.position) > colSizeMAX)
			{
				tarList.Remove(tarList[m]);
				m--;
			}
		}
		tarList.Sort(CompareTargetDistance);
	}

	private bool TrySetFirstTarget()
	{
		int num = 0;
		while (num < tarList.Count)
		{
			if (!IsValidTarget(tarList[num], out var _))
			{
				tarList.RemoveAt(num);
				num--;
				num++;
				continue;
			}
			target = tarList[num].transform;
			return target != null;
		}
		target = null;
		return false;
	}

	private int CompareTargetDistance(FootCOL a, FootCOL b)
	{
		Vector3 position;
		float num = (IsValidTarget(a, out position) ? Vector3.Distance(position, base.transform.position) : float.PositiveInfinity);
		Vector3 position2;
		float value = (IsValidTarget(b, out position2) ? Vector3.Distance(position2, base.transform.position) : float.PositiveInfinity);
		return num.CompareTo(value);
	}

	private static bool IsValidTarget(FootCOL foot, out Vector3 position)
	{
		position = Vector3.zero;
		if (!foot || !foot.peo)
		{
			return false;
		}
		switch (foot.peo.CharacterType)
		{
		case 0:
			if (!foot.peo.pl || !foot.peo.pl.IsAlive)
			{
				return false;
			}
			position = foot.peo.pl.transform.position;
			return true;
		case 1:
			if (!foot.peo.cp || !foot.peo.cp.IsAlive)
			{
				return false;
			}
			position = foot.peo.cp.transform.position;
			return true;
		case 2:
			if (!foot.peo.em || !foot.peo.em.IsAlive || foot.peo.em.IsYS || foot.peo.em.IsJump)
			{
				return false;
			}
			position = foot.peo.em.transform.position;
			return true;
		default:
			return false;
		}
	}
}
