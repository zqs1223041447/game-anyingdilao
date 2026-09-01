using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using FinkFramework.Runtime.Utils;
using Inputs.Gamepad;
using Lean.Pool;
using UnityEngine;

public class SK_Orb_Self : MonoBehaviour
{
	public Skill_SD_List[] SoundA;

	public Skill_PB_List[] ORB;

	[HideInInspector]
	public GameObject Buff;

	[HideInInspector]
	public List<Enemy> em = new List<Enemy>();

	public Collider2D[] hitEM = new Collider2D[5];

	private float Range;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public Transform point;

	private float timeA;

	private float timeB;

	private float timeC;

	private float critFaSheTime;

	private bool ISbattle;

	private bool CanAT;

	private int RDA;

	private float AG;

	[HideInInspector]
	public SK_BuffA mg;

	private PlayerManager _playerManager;

	private GameDataManager _gameDataManager;

	private int type;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		point = base.transform.Find("point");
		_playerManager = SingletonMonoScope<PlayerManager>.Instance;
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
		if (!sp)
		{
			LogUtil.Error("[SK_Orb_Self.Awake] 缺少 SkillOBJ_DT_SP");
		}
		if (!mg)
		{
			LogUtil.Error("[SK_Orb_Self.Awake] 缺少 SK_BuffA");
		}
		if (!point)
		{
			LogUtil.Error("[SK_Orb_Self.Awake] 未找到子物体 point");
		}
	}

	private void OnEnable()
	{
		Enemy.OnPlayerCritDamageEnemy -= HandlePlayerCritDamageEnemy;
		Enemy.OnPlayerCritDamageEnemy += HandlePlayerCritDamageEnemy;
		em.Clear();
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		critFaSheTime = 0f;
		CanAT = false;
		RDA = Random.Range(0, 101);
		if ((bool)point)
		{
			point.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		}
		ISbattle = false;
		initialized = false;
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance || !CanAT)
		{
			return;
		}
		if (type == 10 && (bool)sp && (sp.FStime1 <= 0f || critFaSheTime < sp.FStime1))
		{
			critFaSheTime += Time.deltaTime;
		}
		if (type == 8)
		{
			if (RDA < 50)
			{
				point.transform.Rotate(new Vector3(0f, 0f, 1f), 300f * Time.deltaTime);
			}
			else
			{
				point.transform.Rotate(new Vector3(0f, 0f, 1f), -300f * Time.deltaTime);
			}
		}
		if (ISbattle)
		{
			if (!_playerManager.IsBattle)
			{
				Range = sp.Range1;
				ISbattle = false;
			}
		}
		else if (_playerManager.IsBattle)
		{
			Range = sp.Range2;
			ISbattle = true;
		}
		timeA += Time.deltaTime;
		if (timeA >= sp.FStime1)
		{
			timeA = 0f;
			switch (sp.indexType)
			{
			case 0:
				if (_playerManager.IsAlive)
				{
					Fashe();
				}
				break;
			case 1:
				if ((bool)sp.cp && sp.cp.IsAlive)
				{
					Fashe();
				}
				break;
			case 2:
				if ((bool)sp.em && sp.em.IsAlive)
				{
					Fashe();
				}
				break;
			}
		}
		if (sp.NoTime == 1)
		{
			timeB += Time.deltaTime;
			if (timeB >= sp.BuffTime)
			{
				timeB = 0f;
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
		int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, Range, hitEM, LayerMask.GetMask("BodyCOLem"));
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				BodyCOL component = hitEM[i].GetComponent<BodyCOL>();
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
		Refresh();
		timeC = 0f;
	}

	public void CritFaShe()
	{
		if (!CanCritFaShe())
		{
			return;
		}
		critFaSheTime = 0f;
		if (Random.Range(0, 101) >= sp.Count_F)
		{
			return;
		}
		GameObject safeAnglePrefab = GetSafeAnglePrefab();
		if (!safeAnglePrefab)
		{
			LogPrefabConfigError("Fashe_Type10_ANGLE");
			return;
		}
		Vector3 vector = GetCritAimTargetPosition() - point.transform.position;
		float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		for (int i = 0; i < sp.CountMulti; i++)
		{
			AG = Random.Range(0f - sp.AngleA, sp.AngleA);
			Dicform component = LeanPool.Spawn(safeAnglePrefab, point.transform.position, Quaternion.Euler(0f, 0f, num + AG)).GetComponent<Dicform>();
			component.sp = sp;
			component.SetCount(sp.ZY);
			component.SubType = 0;
			component.Index = 0;
		}
	}

	private void HandlePlayerCritDamageEnemy()
	{
		if ((bool)sp && sp.indexType == 0)
		{
			CritFaShe();
		}
	}

	private bool CanCritFaShe()
	{
		if (!CanAT || type != 10 || !sp || !point)
		{
			return false;
		}
		if (!_playerManager || !_playerManager.IsAlive)
		{
			return false;
		}
		if (!(sp.FStime1 <= 0f))
		{
			return critFaSheTime >= sp.FStime1;
		}
		return true;
	}

	private Vector3 GetCritAimTargetPosition()
	{
		if ((bool)_playerManager && _playerManager.IsAutoLockActive() && _playerManager.TryGetAutoLockYaoPosition(out var position))
		{
			return position;
		}
		return AimProvider.GetAimWorldPos();
	}

	private GameObject GetSafeOrbBuffPrefab()
	{
		if (ORB == null)
		{
			return null;
		}
		if (sp.ORB < 0 || sp.ORB >= ORB.Length)
		{
			return null;
		}
		Skill_PB_List skill_PB_List = ORB[sp.ORB];
		if (skill_PB_List?.PB == null)
		{
			return null;
		}
		if (sp.MainEL < 0 || sp.MainEL >= skill_PB_List.PB.Length)
		{
			return null;
		}
		return skill_PB_List.PB[sp.MainEL];
	}

	private string GetSafeOrbSound()
	{
		if (SoundA == null)
		{
			return null;
		}
		if (sp.Sound < 0 || sp.Sound >= SoundA.Length)
		{
			return null;
		}
		Skill_SD_List skill_SD_List = SoundA[sp.Sound];
		if (skill_SD_List == null || skill_SD_List.SD == null)
		{
			return null;
		}
		if (sp.MainEL < 0 || sp.MainEL >= skill_SD_List.SD.Length)
		{
			return null;
		}
		return skill_SD_List.SD[sp.MainEL];
	}

	private GameObject GetSafePosPrefab()
	{
		if (!_gameDataManager || !_gameDataManager.SKPB || _gameDataManager.SKPB.POS == null)
		{
			return null;
		}
		if (sp.ZD_F < 0 || sp.ZD_F >= _gameDataManager.SKPB.POS.Length)
		{
			return null;
		}
		SKprefab_OBJ sKprefab_OBJ = _gameDataManager.SKPB.POS[sp.ZD_F];
		if (sKprefab_OBJ?.OBJ == null)
		{
			return null;
		}
		if (sp.MainEL < 0 || sp.MainEL >= sKprefab_OBJ.OBJ.Length)
		{
			return null;
		}
		return sKprefab_OBJ.OBJ[sp.MainEL];
	}

	private GameObject GetSafeDicPrefab()
	{
		if (!_gameDataManager || !_gameDataManager.SKPB || _gameDataManager.SKPB.Dic == null)
		{
			return null;
		}
		if (sp.ZD_F < 0 || sp.ZD_F >= _gameDataManager.SKPB.Dic.Length)
		{
			return null;
		}
		SKprefab_Multi sKprefab_Multi = _gameDataManager.SKPB.Dic[sp.ZD_F];
		if (sKprefab_Multi?.OBJ == null)
		{
			return null;
		}
		if (sp.MainEL < 0 || sp.MainEL >= sKprefab_Multi.OBJ.Length)
		{
			return null;
		}
		return sKprefab_Multi.OBJ[sp.MainEL];
	}

	private GameObject GetSafeAnglePrefab()
	{
		if (!_gameDataManager || !_gameDataManager.SKPB || _gameDataManager.SKPB.Angle == null)
		{
			return null;
		}
		if (sp.ZD_F < 0 || sp.ZD_F >= _gameDataManager.SKPB.Angle.Length)
		{
			return null;
		}
		SKprefab_Multi sKprefab_Multi = _gameDataManager.SKPB.Angle[sp.ZD_F];
		if (sKprefab_Multi?.OBJ == null)
		{
			return null;
		}
		if (sp.MainEL < 0 || sp.MainEL >= sKprefab_Multi.OBJ.Length)
		{
			return null;
		}
		return sKprefab_Multi.OBJ[sp.MainEL];
	}

	private void LogPrefabConfigError(string source)
	{
		LogUtil.Warn("[SK_Orb_Self." + source + "] 配置缺失或越界 | " + $"skill={sp?.skillName}, sp.ORB={sp?.ORB}, sp.Sound={sp?.Sound}, sp.ZD_F={sp?.ZD_F}, sp.MainEL={sp?.MainEL}");
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
		Range = sp.Range1;
		type = sp.TypeORB;
		if (type == 10)
		{
			critFaSheTime = sp.FStime1;
		}
		GameObject safeOrbBuffPrefab = GetSafeOrbBuffPrefab();
		if ((bool)safeOrbBuffPrefab)
		{
			Buff = LeanPool.Spawn(safeOrbBuffPrefab, base.transform.position, Quaternion.identity, base.transform);
		}
		else
		{
			LogPrefabConfigError("SetStart_ORB");
		}
		string safeOrbSound = GetSafeOrbSound();
		if (!string.IsNullOrEmpty(safeOrbSound))
		{
			RuntimeManager.PlayOneShot(safeOrbSound, base.transform.position);
		}
	}

	public void Fashe()
	{
		GameObject safePosPrefab = GetSafePosPrefab();
		GameObject safeDicPrefab = GetSafeDicPrefab();
		GameObject safeAnglePrefab = GetSafeAnglePrefab();
		if (em.Count > 0)
		{
			int max = ((em.Count > 3) ? 3 : em.Count);
			Enemy enemy = em[Random.Range(0, max)];
			if ((bool)enemy)
			{
				Vector3 vector = enemy.transform.position - base.transform.position;
				switch (type)
				{
				case 0:
				{
					if (!safePosPrefab)
					{
						LogPrefabConfigError("Fashe_Type0_POS");
						break;
					}
					Dicform component2 = LeanPool.Spawn(safePosPrefab, enemy.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component2.sp = sp;
					component2.SetCount(sp.ZY);
					component2.SubType = 0;
					component2.Index = 0;
					break;
				}
				case 1:
				{
					if (!safeDicPrefab)
					{
						LogPrefabConfigError("Fashe_Type1_DIC");
						break;
					}
					Dicform component = LeanPool.Spawn(safeDicPrefab, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component.sp = sp;
					component.SetCount(sp.ZY);
					component.SubType = 0;
					component.Index = 0;
					component.dic = vector;
					break;
				}
				}
			}
		}
		switch (type)
		{
		case 2:
		{
			if (!safePosPrefab)
			{
				LogPrefabConfigError("Fashe_Type2_POS");
				break;
			}
			Vector3 vector3 = Random.insideUnitCircle * sp.Range1;
			Dicform component4 = LeanPool.Spawn(safePosPrefab, new Vector3(base.transform.position.x + vector3.x, base.transform.position.y + vector3.y, base.transform.position.z + vector3.z), Quaternion.identity).GetComponent<Dicform>();
			component4.sp = sp;
			component4.SetCount(sp.ZY);
			component4.SubType = 0;
			component4.Index = 0;
			break;
		}
		case 3:
		{
			if (!safeDicPrefab)
			{
				LogPrefabConfigError("Fashe_Type3_DIC");
				break;
			}
			Dicform component9 = LeanPool.Spawn(safeDicPrefab, _playerManager.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component9.sp = sp;
			component9.SetCount(sp.ZY);
			component9.SubType = 0;
			component9.Index = 0;
			component9.dic = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			break;
		}
		case 4:
		{
			if (!safeDicPrefab)
			{
				LogPrefabConfigError("Fashe_Type4_DIC");
				break;
			}
			Dicform component5 = LeanPool.Spawn(safeDicPrefab, _playerManager.yao.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component5.sp = sp;
			component5.SetCount(sp.ZY);
			component5.SubType = 0;
			component5.Index = 0;
			component5.dic = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			component5.speed = Random.Range(sp.Speed1, sp.Speed2);
			break;
		}
		case 5:
		{
			if (!safePosPrefab)
			{
				LogPrefabConfigError("Fashe_Type5_POS");
				break;
			}
			Dicform component6 = LeanPool.Spawn(safePosPrefab, _playerManager.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component6.sp = sp;
			component6.SetCount(sp.ZY);
			component6.SubType = 0;
			component6.Index = 0;
			break;
		}
		case 6:
		{
			if (!safeAnglePrefab)
			{
				LogPrefabConfigError("Fashe_Type6_ANGLE");
				break;
			}
			for (int j = 0; j < sp.Count_F; j++)
			{
				Dicform component8 = LeanPool.Spawn(safeAnglePrefab, _playerManager.yao.transform.position, Quaternion.Euler(0f, 0f, 360f / (float)sp.Count_F * (float)(j + 1))).GetComponent<Dicform>();
				component8.sp = sp;
				component8.SetCount(sp.ZY);
				component8.SubType = 0;
				component8.Index = 0;
			}
			break;
		}
		case 7:
		{
			if (!safeAnglePrefab)
			{
				LogPrefabConfigError("Fashe_Type7_ANGLE");
				break;
			}
			for (int k = 0; k < sp.CountMulti; k++)
			{
				Dicform component10 = LeanPool.Spawn(safeAnglePrefab, _playerManager.yao.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0, 360))).GetComponent<Dicform>();
				component10.sp = sp;
				component10.SetCount(sp.ZY);
				component10.SubType = 0;
				component10.Index = 0;
			}
			break;
		}
		case 8:
		{
			if (!safeAnglePrefab)
			{
				LogPrefabConfigError("Fashe_Type8_ANGLE");
				break;
			}
			Vector3 right = point.right;
			float z = Mathf.Atan2(right.y, right.x) * 57.29578f;
			Dicform component7 = LeanPool.Spawn(safeAnglePrefab, point.transform.position, Quaternion.Euler(0f, 0f, z)).GetComponent<Dicform>();
			component7.sp = sp;
			component7.SetCount(sp.ZY);
			component7.SubType = 0;
			component7.Index = 0;
			break;
		}
		case 9:
		{
			if (!safeAnglePrefab)
			{
				LogPrefabConfigError("Fashe_Type9_ANGLE");
				break;
			}
			Vector3 vector2 = AimProvider.GetAimWorldPos() - point.transform.position;
			float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
			for (int i = 0; i < sp.CountMulti; i++)
			{
				AG = Random.Range(0f - sp.AngleA, sp.AngleA);
				Dicform component3 = LeanPool.Spawn(safeAnglePrefab, point.transform.position, Quaternion.Euler(0f, 0f, num + AG)).GetComponent<Dicform>();
				component3.sp = sp;
				component3.SetCount(sp.ZY);
				component3.SubType = 0;
				component3.Index = 0;
			}
			break;
		}
		case 10:
			break;
		}
	}

	public void Refresh()
	{
		for (int i = 0; i < em.Count; i++)
		{
			Enemy enemy = em[i];
			if (enemy == null || enemy.yao == null)
			{
				em.RemoveAt(i);
				i--;
			}
			else if (!enemy.IsAlive || enemy.IsJump || enemy.IsYS)
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
			bool flag = t1 == null || t1.yao == null;
			bool flag2 = t2 == null || t2.yao == null;
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

	public void Stop()
	{
		CanAT = false;
		em.Clear();
		if ((bool)Buff)
		{
			LeanPool.Despawn(Buff);
		}
		LeanPool.Despawn(this);
	}

	private void OnDisable()
	{
		Enemy.OnPlayerCritDamageEnemy -= HandlePlayerCritDamageEnemy;
	}
}
