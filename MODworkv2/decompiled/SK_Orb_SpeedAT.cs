using System;
using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_Orb_SpeedAT : MonoBehaviour
{
	public string[] SoundA;

	public Skill_PB_List[] Orb;

	public float RotateSpeed;

	[HideInInspector]
	public Transform point;

	[HideInInspector]
	public List<GameObject> OrbList = new List<GameObject>();

	[HideInInspector]
	public List<Enemy> em = new List<Enemy>();

	public Collider2D[] hitEM = new Collider2D[8];

	private float Range;

	private float timeA;

	private float timeB;

	private float timeC;

	private float timeD;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public GameObject core;

	private bool ISbattle;

	private float ATspeedTmp;

	private int RDA;

	private float RDB;

	private float RDC;

	private bool CanAT;

	private int Cur_Orb;

	[HideInInspector]
	public SK_BuffA mg;

	private PlayerManager _playerManager;

	private GameDataManager _gameDataManager;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		core = base.transform.Find("core").gameObject;
		mg = GetComponent<SK_BuffA>();
		_playerManager = SingletonMonoScope<PlayerManager>.Instance;
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
	}

	private void OnEnable()
	{
		em.Clear();
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		timeD = 0f;
		CanAT = false;
		Cur_Orb = 0;
		core.transform.rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		ISbattle = false;
		RDA = UnityEngine.Random.Range(0, 101);
		RDB = UnityEngine.Random.Range(0f, RotateSpeed / 2f);
		initialized = false;
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance || !CanAT)
		{
			return;
		}
		if (RDA < 50)
		{
			core.transform.Rotate(new Vector3(0f, 0f, 1f), (RotateSpeed + RDB) * Time.deltaTime);
		}
		else
		{
			core.transform.Rotate(new Vector3(0f, 0f, 1f), (0f - RotateSpeed - RDB) * Time.deltaTime);
		}
		if (ISbattle)
		{
			if (!_playerManager.IsBattle)
			{
				Range = sp.Range1;
				ISbattle = false;
			}
			if (em.Count > 0 && ATspeedTmp > sp.FStime2)
			{
				timeB += Time.deltaTime;
				if (timeB >= 0.1f)
				{
					timeB = 0f;
					ATspeedTmp -= 0.01f;
					if (ATspeedTmp < sp.FStime2)
					{
						ATspeedTmp = sp.FStime2;
					}
				}
			}
		}
		else if (_playerManager.IsBattle)
		{
			Range = sp.Range2;
			ISbattle = true;
		}
		if ((!ISbattle || em.Count == 0) && ATspeedTmp < sp.FStime1)
		{
			timeB += Time.deltaTime;
			if (timeB >= 0.1f)
			{
				timeB = 0f;
				ATspeedTmp += 0.01f;
				if (ATspeedTmp > sp.FStime1)
				{
					ATspeedTmp = sp.FStime1;
				}
			}
		}
		if (_playerManager.IsAlive && em.Count > 0)
		{
			timeA += Time.deltaTime;
			if (timeA >= ATspeedTmp)
			{
				timeA = 0f;
				switch (sp.Type_F)
				{
				case 0:
				{
					for (int i = 0; i < OrbList.Count; i++)
					{
						GameObject gameObject2 = OrbList[i];
						if ((bool)gameObject2)
						{
							int num2 = ((em.Count > 3) ? 3 : em.Count);
							if (num2 <= 0)
							{
								break;
							}
							Enemy enemy2 = em[UnityEngine.Random.Range(0, num2)];
							if ((bool)enemy2 && (bool)enemy2.yao)
							{
								Vector3 vector2 = enemy2.yao.transform.position - gameObject2.transform.position;
								float z2 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
								Dicform component2 = ((sp.TypeORB != 0) ? LeanPool.Spawn(_gameDataManager.SKPB.Dic[sp.ZD_F].OBJ[sp.MainEL], gameObject2.transform.position, Quaternion.identity) : LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], gameObject2.transform.position, Quaternion.Euler(0f, 0f, z2))).GetComponent<Dicform>();
								component2.sp = sp;
								component2.SetCount(sp.ZY);
								component2.SubType = 0;
								component2.Index = 0;
								component2.dic = vector2;
							}
						}
					}
					break;
				}
				case 1:
				{
					if (Cur_Orb < 0 || Cur_Orb >= OrbList.Count)
					{
						Cur_Orb = 0;
						break;
					}
					GameObject gameObject = OrbList[Cur_Orb];
					if (!gameObject)
					{
						Cur_Orb = 0;
						break;
					}
					int num = ((em.Count > 3) ? 3 : em.Count);
					if (num <= 0)
					{
						break;
					}
					Enemy enemy = em[UnityEngine.Random.Range(0, num)];
					if ((bool)enemy && (bool)enemy.yao)
					{
						Vector3 vector = enemy.yao.transform.position - gameObject.transform.position;
						float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
						Dicform component = ((sp.TypeORB != 0) ? LeanPool.Spawn(_gameDataManager.SKPB.Dic[sp.ZD_F].OBJ[sp.MainEL], gameObject.transform.position, Quaternion.identity) : LeanPool.Spawn(_gameDataManager.SKPB.Angle[sp.ZD_F].OBJ[sp.MainEL], gameObject.transform.position, Quaternion.Euler(0f, 0f, z))).GetComponent<Dicform>();
						component.sp = sp;
						component.SetCount(sp.ZY);
						component.SubType = 0;
						component.Index = 0;
						component.dic = vector;
						if (Cur_Orb < OrbList.Count - 1)
						{
							Cur_Orb++;
						}
						else
						{
							Cur_Orb = 0;
						}
					}
					break;
				}
				}
			}
		}
		if (sp.NoTime == 1)
		{
			timeD += Time.deltaTime;
			if (timeD >= sp.BuffTime)
			{
				timeD = 0f;
				Stop();
			}
			if (mg.NeedStop)
			{
				Stop();
			}
		}
		else if (mg.ORBStop)
		{
			Stop();
		}
		timeC += Time.deltaTime;
		if (!(timeC >= 0.19f))
		{
			return;
		}
		if (sp.SpecialType == 10)
		{
			_playerManager.RefreshORB(sp, 0);
		}
		int num3 = Physics2D.OverlapCircleNonAlloc(base.transform.position, Range, hitEM, LayerMask.GetMask("BodyCOLem"));
		if (num3 > 0)
		{
			for (int j = 0; j < num3; j++)
			{
				BodyCOL component3 = hitEM[j].GetComponent<BodyCOL>();
				if ((bool)component3)
				{
					if (component3.peo.CharacterType == 2 && component3.peo.em.IsAlive && !em.Contains(component3.peo.em) && !component3.peo.em.IsJump && !component3.peo.em.IsYS)
					{
						em.Add(component3.peo.em);
					}
					hitEM[j] = null;
				}
			}
		}
		Refresh();
		timeC = 0f;
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
		ATspeedTmp = sp.FStime1;
		CanAT = true;
		Range = sp.Range1;
		if (SoundA[sp.MainEL] != null)
		{
			RuntimeManager.PlayOneShot(SoundA[sp.MainEL], base.transform.position);
		}
		for (int i = 0; i < sp.Count_ORB; i++)
		{
			float angle = (float)i / (float)sp.Count_ORB * 360f;
			Vector3 position = AnglePOS(base.transform.position, sp.Size, angle);
			GameObject item = LeanPool.Spawn(Orb[sp.MainEL].PB[sp.ORB], position, Quaternion.identity, core.transform);
			OrbList.Add(item);
		}
		RDC = UnityEngine.Random.Range(0.8f, 1.2f);
		core.transform.localScale = new Vector3(RDC, RDC, RDC);
	}

	public void Refresh()
	{
		for (int i = 0; i < em.Count; i++)
		{
			Enemy enemy = em[i];
			if (!enemy || !enemy.yao || !enemy.IsAlive || enemy.IsJump || enemy.IsYS)
			{
				em.RemoveAt(i);
				i--;
			}
			else if (Vector3.Distance(enemy.yao.transform.position, base.transform.position) > Range)
			{
				em.RemoveAt(i);
				i--;
			}
		}
		em.Sort(delegate(Enemy t1, Enemy t2)
		{
			bool flag = !t1 || !t1.yao;
			bool flag2 = !t2 || !t2.yao;
			if (flag && flag2)
			{
				return 0;
			}
			if (flag)
			{
				return 1;
			}
			if (flag2)
			{
				return -1;
			}
			float num = Vector3.Distance(t1.yao.transform.position, base.transform.position);
			float value = Vector3.Distance(t2.yao.transform.position, base.transform.position);
			return num.CompareTo(value);
		});
	}

	private static Vector3 AnglePOS(Vector3 center, float radius, float angle)
	{
		float f = angle * ((float)Math.PI / 180f);
		float x = center.x + radius * Mathf.Cos(f);
		float y = center.y + radius * Mathf.Sin(f);
		return new Vector3(x, y, 0f);
	}

	public void Stop()
	{
		CanAT = false;
		em.Clear();
		int num;
		for (num = 0; num < OrbList.Count; num++)
		{
			GameObject gameObject = OrbList[num];
			LeanPool.Despawn(gameObject);
			OrbList.Remove(gameObject);
			num--;
		}
		LeanPool.Despawn(this);
	}
}
