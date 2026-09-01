using System.Collections.Generic;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_BlackHole : MonoBehaviour
{
	public GameObject OBJ;

	public GameObject FX;

	public bool NOAT;

	public ParticleSystem[] parLoop;

	[HideInInspector]
	public GameObject qiu;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public SK_BuffA mg;

	private float timeA;

	private float timeB;

	private float timeC;

	private bool CanAT;

	[HideInInspector]
	public List<Enemy> em = new List<Enemy>();

	public Collider2D[] hitEM = new Collider2D[8];

	private int MainEL;

	private float range;

	private PlayerManager PL;

	private GameDataManager _gameDataManager;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		qiu = base.transform.Find("Main/qiu").gameObject;
		PL = SingletonMonoScope<PlayerManager>.Instance;
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		CanAT = false;
		qiu.SetActive(value: false);
		em.Clear();
		for (int i = 0; i < hitEM.Length; i++)
		{
			hitEM[i] = null;
		}
		initialized = false;
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance || !CanAT)
		{
			return;
		}
		timeB += Time.deltaTime;
		if (timeB >= sp.FStime1)
		{
			if (sp.ZY)
			{
				if (!NOAT)
				{
					switch (sp.TypeORB)
					{
					case 0:
						if (em.Count > 0)
						{
							Vector3 vector3 = em[0].yao.transform.position - qiu.transform.position;
							float num2 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
							Dicform component4 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[MainEL], qiu.transform.position, Quaternion.Euler(0f, 0f, num2 + Random.Range(0f - sp.AngleA, sp.AngleA))).GetComponent<Dicform>();
							component4.sp = sp;
							component4.SetCount(sp.ZY);
							component4.SubType = 0;
							component4.Index = 1;
						}
						break;
					case 1:
						if (em.Count > 0)
						{
							for (int l = 0; l < sp.Count_F; l++)
							{
								Vector3 vector4 = em[Random.Range(0, em.Count)].yao.transform.position - qiu.transform.position;
								float num3 = Mathf.Atan2(vector4.y, vector4.x) * 57.29578f;
								Dicform component6 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[MainEL], qiu.transform.position, Quaternion.Euler(0f, 0f, num3 + Random.Range(0f - sp.AngleA, sp.AngleA))).GetComponent<Dicform>();
								component6.sp = sp;
								component6.SetCount(sp.ZY);
								component6.SubType = 0;
								component6.Index = 1;
							}
						}
						break;
					case 2:
					{
						for (int j = 0; j < sp.Count_F; j++)
						{
							Vector3 vector2 = Gun.MousePos - qiu.transform.position;
							float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
							Dicform component2 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[MainEL], qiu.transform.position, Quaternion.Euler(0f, 0f, num + Random.Range(0f - sp.AngleA, sp.AngleA))).GetComponent<Dicform>();
							component2.sp = sp;
							component2.SetCount(sp.ZY);
							component2.SubType = 0;
							component2.Index = 1;
						}
						break;
					}
					case 3:
					{
						for (int k = 0; k < sp.Count_F; k++)
						{
							Dicform component5 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[MainEL], PL.yao.transform.position, Quaternion.Euler(0f, 0f, 360 / sp.Count_F * (k + 1))).GetComponent<Dicform>();
							component5.sp = sp;
							component5.SetCount(sp.ZY);
							component5.SubType = 0;
							component5.Index = 1;
						}
						break;
					}
					case 4:
						if (em.Count > 0)
						{
							Dicform component3 = LeanPool.Spawn(_gameDataManager.SKPB.POS[sp.ZD_F].OBJ[MainEL], em[0].transform.position, Quaternion.identity).GetComponent<Dicform>();
							component3.sp = sp;
							component3.SetCount(sp.ZY);
							component3.SubType = 0;
							component3.Index = 1;
						}
						break;
					case 5:
					{
						for (int i = 0; i < sp.Count_F; i++)
						{
							Vector3 vector = Random.insideUnitCircle * range;
							Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.POS[sp.ZD_F].OBJ[MainEL], new Vector3(base.transform.position.x + vector.x, base.transform.position.y + vector.y, base.transform.position.z + vector.z), Quaternion.identity).GetComponent<Dicform>();
							component.sp = sp;
							component.SetCount(sp.ZY);
							component.SubType = 0;
							component.Index = 1;
						}
						break;
					}
					}
				}
				if (sp.Layer_SubA == 0 && sp.DamageA > 0f && em.Count > 0)
				{
					Dicform component7 = LeanPool.Spawn(_gameDataManager.SKPB.POS[sp.ZD_AB].OBJ[MainEL], em[Random.Range(0, em.Count)].transform.position, Quaternion.identity).GetComponent<Dicform>();
					component7.sp = sp;
					component7.SetCount(sp.ZY);
					component7.SubType = 1;
					component7.Index = 1;
				}
			}
			else if (!NOAT)
			{
				switch (sp.TypeORB)
				{
				case 0:
				{
					for (int n = 0; n < sp.Count_F; n++)
					{
						Dicform component9 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[MainEL], qiu.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360))).GetComponent<Dicform>();
						component9.sp = sp;
						component9.SetCount(sp.ZY);
						component9.SubType = 0;
						component9.Index = 1;
					}
					break;
				}
				case 1:
				{
					for (int num4 = 0; num4 < sp.Count_F; num4++)
					{
						Dicform component10 = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[MainEL], sp.em.yao.transform.position, Quaternion.Euler(0f, 0f, 360 / sp.Count_F * (num4 + 1))).GetComponent<Dicform>();
						component10.sp = sp;
						component10.SetCount(sp.ZY);
						component10.SubType = 0;
						component10.Index = 1;
					}
					break;
				}
				case 2:
				{
					for (int m = 0; m < sp.Count_F; m++)
					{
						Vector3 vector5 = Random.insideUnitCircle * range;
						Dicform component8 = LeanPool.Spawn(_gameDataManager.SKPB.POS[sp.ZD_F].OBJ[MainEL], new Vector3(base.transform.position.x + vector5.x, base.transform.position.y + vector5.y, base.transform.position.z + vector5.z), Quaternion.identity).GetComponent<Dicform>();
						component8.sp = sp;
						component8.SetCount(sp.ZY);
						component8.SubType = 0;
						component8.Index = 1;
					}
					break;
				}
				}
			}
			timeB = 0f;
		}
		if (mg.NeedStop)
		{
			Stop();
		}
		timeA += Time.deltaTime;
		if (timeA >= sp.BuffTime)
		{
			Stop();
		}
		timeC += Time.deltaTime;
		if (timeC >= 0.25f)
		{
			Refresh();
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
		qiu.transform.localPosition = new Vector3(0f, sp.High, 0f);
		qiu.SetActive(value: true);
		range = sp.Range1;
		MainEL = sp.MainEL;
		if ((bool)OBJ)
		{
			LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity);
		}
		if (sp.Reborn > 0)
		{
			switch (sp.indexType)
			{
			case 0:
				PL.HealStat.Cur += PL.HealStat.Max * (float)sp.Reborn / 100f;
				break;
			case 1:
				sp.cp.HealthStat.CurrentValue += sp.cp.HealthStat.MaxValue * (float)sp.Reborn / 100f;
				break;
			case 2:
				sp.em.HealthStat.CurrentValue += sp.em.HealthStat.MaxValue * (float)sp.Reborn / 100f;
				break;
			}
		}
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = true;
			}
		}
		if (sp.Layer_SubB == 0 && sp.DamageB > 0f)
		{
			for (int j = 0; j < sp.Count_AB; j++)
			{
				Dicform component = LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_AB].OBJ[MainEL], SingletonMonoScope<PlayerManager>.Instance.yao.transform.position, Quaternion.Euler(0f, 0f, 360f / (float)sp.Count_AB * (float)(j + 1))).GetComponent<Dicform>();
				component.sp = sp;
				component.SetCount(sp.ZY);
				component.SubType = 2;
				component.Index = 1;
			}
		}
	}

	public void Refresh()
	{
		int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, range, hitEM, LayerMask.GetMask("FootCOLem"));
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				FootCOL component = hitEM[i].GetComponent<FootCOL>();
				if ((bool)component)
				{
					if (component.peo.CharacterType == 2 && component.peo.em.IsAlive && !em.Contains(component.peo.em) && !component.peo.em.IsJump && !component.peo.em.IsYS)
					{
						em.Add(component.peo.em);
					}
					hitEM[i] = null;
				}
			}
		}
		if (em.Count <= 0)
		{
			return;
		}
		for (int j = 0; j < em.Count; j++)
		{
			if (!em[j].IsAlive || em[j].IsJump || em[j].IsYS || Vector2.Distance(em[j].transform.position, base.transform.position) > range)
			{
				em.Remove(em[j]);
				j--;
			}
		}
		if (em.Count > 1)
		{
			em.Sort((Enemy t1, Enemy t2) => Vector3.Distance(t1.transform.position, base.transform.position).CompareTo(Vector3.Distance(t2.transform.position, base.transform.position)));
		}
	}

	public void Stop()
	{
		CanAT = false;
		if (FX != null)
		{
			LeanPool.Spawn(FX, base.transform.position, Quaternion.identity);
		}
		if (parLoop.Length != 0)
		{
			for (int i = 0; i < parLoop.Length; i++)
			{
				ParticleSystem.MainModule main = parLoop[i].main;
				main.loop = false;
			}
		}
		if (sp.ChangeSkin == 0)
		{
			SingletonMonoScope<ACTbar>.Instance.HasSameSkillFX(sp.skillName);
		}
		this.wait(1f, delegate
		{
			LeanPool.Despawn(this);
		});
	}
}
