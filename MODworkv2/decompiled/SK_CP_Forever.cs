using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_CP_Forever : MonoBehaviour
{
	private const int MaxTrackedEnemyCount = 6;

	private const float FireIntervalRandomMax = 0.5f;

	public string[] SoundA;

	public GameObject[] ORB;

	public GameObject[] Fire;

	[HideInInspector]
	public List<Enemy> em = new List<Enemy>();

	public Collider2D[] hitEM = new Collider2D[6];

	private readonly List<GameObject> activeFire = new List<GameObject>();

	private GameObject Buff;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	[HideInInspector]
	public SK_BuffA mg;

	private float Range;

	private float timeA;

	private float timeB;

	private float timeC;

	private float fireIntervalRandom;

	private bool ISbattle;

	private bool CanAT;

	private int type;

	private PlayerManager PL;

	private GameDataManager _gameDataManager;

	private Companion CP;

	private bool initialized;

	private Transform point;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		PL = SingletonMonoScope<PlayerManager>.Instance;
		_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
		point = base.transform.Find("point");
	}

	private void OnEnable()
	{
		em.Clear();
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		ResetFireIntervalRandom();
		CanAT = false;
		ISbattle = false;
		initialized = false;
		for (int i = 0; i < hitEM.Length; i++)
		{
			hitEM[i] = null;
		}
		ClearFire();
		ClearBuff();
	}

	private void Update()
	{
		if (!SingletonMonoScope<GameDataManager>.HasInstance || !CanAT)
		{
			return;
		}
		if (!PL)
		{
			PL = SingletonMonoScope<PlayerManager>.Instance;
		}
		if (!PL || !sp)
		{
			return;
		}
		if (!CP || !CP.IsAlive)
		{
			Stop();
			return;
		}
		if (ISbattle)
		{
			if (!PL.IsBattle)
			{
				Range = sp.Range1;
				ISbattle = false;
			}
		}
		else if (PL.IsBattle)
		{
			Range = sp.Range2;
			ISbattle = true;
		}
		if (type == 2)
		{
			UpdateFireDirection();
		}
		timeA += Time.deltaTime;
		if (timeA >= sp.FStime1 + fireIntervalRandom)
		{
			timeA = 0f;
			if (NeedTarget(type))
			{
				RefreshTargetCache();
			}
			Fashe();
			ResetFireIntervalRandom();
		}
		if (sp.NoTime == 1)
		{
			timeB += Time.deltaTime;
			if (timeB >= sp.BuffTime)
			{
				timeB = 0f;
				Stop();
				return;
			}
			if ((bool)mg && mg.NeedStop)
			{
				Stop();
				return;
			}
		}
		else if ((bool)mg && mg.ORBStop)
		{
			Stop();
			return;
		}
		timeC += Time.deltaTime;
		if (timeC >= 0.19f)
		{
			if (SingletonMonoScope<ACTbar>.HasInstance)
			{
				SingletonMonoScope<ACTbar>.Instance.RefreshCompanionUniverseData(sp, CP);
			}
			RefreshTargetCache();
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
		if ((bool)sp)
		{
			return sp.cp;
		}
		return false;
	}

	public void SetStart()
	{
		CP = sp.cp;
		CanAT = true;
		Range = sp.Range1;
		type = sp.TypeORB;
		GameObject orbBuffPrefab = GetOrbBuffPrefab();
		if ((bool)orbBuffPrefab)
		{
			Buff = LeanPool.Spawn(orbBuffPrefab, base.transform.position, Quaternion.identity, base.transform);
		}
		string sound = GetSound();
		if (!string.IsNullOrEmpty(sound))
		{
			RuntimeManager.PlayOneShot(sound, base.transform.position);
		}
	}

	public void Fashe()
	{
		if (NeedTarget(type))
		{
			RefreshTargetCache();
		}
		if (type == 2)
		{
			FireAtNearestTarget();
			return;
		}
		GameObject posPrefab = GetPosPrefab();
		GameObject dicPrefab = GetDicPrefab();
		GameObject anglePrefab = GetAnglePrefab();
		if (em.Count > 0)
		{
			int max = ((em.Count > 3) ? 3 : em.Count);
			Enemy enemy = em[Random.Range(0, max)];
			if ((bool)enemy)
			{
				Vector2 dic = enemy.transform.position - base.transform.position;
				switch (type)
				{
				case 0:
				{
					if (!posPrefab)
					{
						LogPrefabConfigError("Fashe_Type0_POS");
						break;
					}
					GameObject go2 = LeanPool.Spawn(posPrefab, enemy.transform.position, Quaternion.identity);
					SetDicform(go2, Vector2.zero);
					break;
				}
				case 1:
				{
					if (!dicPrefab)
					{
						LogPrefabConfigError("Fashe_Type1_DIC");
						break;
					}
					GameObject go = LeanPool.Spawn(dicPrefab, base.transform.position, Quaternion.identity);
					SetDicform(go, dic);
					break;
				}
				}
			}
		}
		switch (type)
		{
		case 5:
		{
			if (!posPrefab)
			{
				LogPrefabConfigError("Fashe_Type5_POS");
				break;
			}
			GameObject go4 = LeanPool.Spawn(posPrefab, CP.transform.position, Quaternion.identity);
			SetDicform(go4, Vector2.zero);
			break;
		}
		case 6:
		{
			if (sp.Count_F <= 0)
			{
				break;
			}
			if (!anglePrefab)
			{
				LogPrefabConfigError("Fashe_Type6_ANGLE");
				break;
			}
			Transform transform = (CP.yao ? CP.yao.transform : CP.transform);
			for (int i = 0; i < sp.Count_F; i++)
			{
				GameObject go3 = LeanPool.Spawn(anglePrefab, transform.position, Quaternion.Euler(0f, 0f, 360f / (float)sp.Count_F * (float)(i + 1)));
				SetDicform(go3, Vector2.zero);
			}
			break;
		}
		}
	}

	private void FireAtNearestTarget()
	{
		GameObject gameObject = GetFirePrefab();
		if (!gameObject)
		{
			gameObject = GetPosPrefab();
		}
		Enemy nearestFireTarget = GetNearestFireTarget();
		if (!gameObject || !nearestFireTarget || !nearestFireTarget.yao)
		{
			return;
		}
		Transform transform = (point ? point : base.transform);
		Vector2 dic = nearestFireTarget.yao.transform.position - transform.position;
		if (!(dic.sqrMagnitude <= 0.0001f))
		{
			float z = Mathf.Atan2(dic.y, dic.x) * 57.29578f;
			GameObject gameObject2 = LeanPool.Spawn(gameObject, transform.position, Quaternion.Euler(0f, 0f, z), transform);
			activeFire.Add(gameObject2);
			SetDicform(gameObject2, dic);
			SK_Pen component = gameObject2.GetComponent<SK_Pen>();
			if ((bool)component)
			{
				component.InitPen(component.ATtime = 1f);
			}
		}
	}

	private GameObject GetFirePrefab()
	{
		if (Fire == null || !sp)
		{
			return null;
		}
		if (sp.MainEL < 0 || sp.MainEL >= Fire.Length)
		{
			return null;
		}
		return Fire[sp.MainEL];
	}

	private Enemy GetNearestFireTarget()
	{
		Enemy result = null;
		float num = float.MaxValue;
		Vector3 a = (CP ? CP.transform.position : base.transform.position);
		for (int num2 = em.Count - 1; num2 >= 0; num2--)
		{
			Enemy enemy = em[num2];
			if ((bool)enemy && enemy.IsAlive && (bool)enemy.yao)
			{
				float num3 = Vector3.Distance(a, enemy.transform.position);
				if (num3 < num)
				{
					result = enemy;
					num = num3;
				}
			}
		}
		return result;
	}

	private void UpdateFireDirection()
	{
		Enemy nearestFireTarget = GetNearestFireTarget();
		if ((bool)nearestFireTarget && (bool)nearestFireTarget.yao)
		{
			Transform transform = (point ? point : base.transform);
			Vector2 vector = nearestFireTarget.yao.transform.position - transform.position;
			if (!(vector.sqrMagnitude > 0.0001f))
			{
				return;
			}
			float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			Quaternion rotation = Quaternion.Euler(0f, 0f, z);
			for (int num = activeFire.Count - 1; num >= 0; num--)
			{
				GameObject gameObject = activeFire[num];
				if (!gameObject || !gameObject.activeInHierarchy)
				{
					activeFire.RemoveAt(num);
				}
				else
				{
					gameObject.transform.rotation = rotation;
				}
			}
			return;
		}
		for (int num2 = activeFire.Count - 1; num2 >= 0; num2--)
		{
			if (!activeFire[num2] || !activeFire[num2].activeInHierarchy)
			{
				activeFire.RemoveAt(num2);
			}
		}
	}

	private GameObject GetOrbBuffPrefab()
	{
		if (ORB == null || !sp)
		{
			return null;
		}
		if (sp.MainEL < 0 || sp.MainEL >= ORB.Length)
		{
			return null;
		}
		return ORB[sp.MainEL];
	}

	private GameObject GetPosPrefab()
	{
		if (!RefreshGameDataManager() || _gameDataManager.SKPB.POS == null)
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

	private GameObject GetDicPrefab()
	{
		if (!RefreshGameDataManager() || _gameDataManager.SKPB.Dic == null)
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

	private GameObject GetAnglePrefab()
	{
		if (!RefreshGameDataManager() || _gameDataManager.SKPB.Angle == null)
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

	private bool RefreshGameDataManager()
	{
		if (!_gameDataManager)
		{
			_gameDataManager = SingletonMonoScope<GameDataManager>.Instance;
		}
		if ((bool)_gameDataManager)
		{
			return _gameDataManager.SKPB;
		}
		return false;
	}

	private string GetSound()
	{
		if (SoundA == null || !sp)
		{
			return null;
		}
		if (sp.MainEL < 0 || sp.MainEL >= SoundA.Length)
		{
			return null;
		}
		return SoundA[sp.MainEL];
	}

	private void SetDicform(GameObject go, Vector2 dic)
	{
		Dicform component = go.GetComponent<Dicform>();
		if ((bool)component)
		{
			component.sp = sp;
			component.SetCount(sp.ZY);
			component.SubType = 0;
			component.Index = 0;
			component.dic = dic;
		}
	}

	private void ClearFire()
	{
		for (int num = activeFire.Count - 1; num >= 0; num--)
		{
			GameObject gameObject = activeFire[num];
			if ((bool)gameObject)
			{
				LeanPool.Despawn(gameObject);
			}
		}
		activeFire.Clear();
	}

	private void ClearBuff()
	{
		if ((bool)Buff)
		{
			LeanPool.Despawn(Buff);
			Buff = null;
		}
	}

	private bool NeedTarget(int orbType)
	{
		if (orbType != 0 && orbType != 1)
		{
			return orbType == 2;
		}
		return true;
	}

	private void ResetFireIntervalRandom()
	{
		fireIntervalRandom = Random.Range(0f, 0.5f);
	}

	private void RefreshTargetCache()
	{
		Refresh();
		if (em.Count < 6)
		{
			int num = Physics2D.OverlapCircleNonAlloc(base.transform.position, Range, hitEM, LayerMask.GetMask("BodyCOLem"));
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					BodyCOL bodyCOL = (hitEM[i] ? hitEM[i].GetComponent<BodyCOL>() : null);
					if ((bool)bodyCOL && bodyCOL.peo.CharacterType == 2 && bodyCOL.peo.em.IsAlive && !em.Contains(bodyCOL.peo.em) && !bodyCOL.peo.em.IsJump && !bodyCOL.peo.em.IsYS)
					{
						em.Add(bodyCOL.peo.em);
					}
					hitEM[i] = null;
				}
			}
		}
		Refresh();
	}

	private void LogPrefabConfigError(string source)
	{
		Debug.LogWarning("[SK_CP_Forever." + source + "] prefab config missing or out of range | " + $"skill={sp?.skillName}, ZD_F={sp?.ZD_F}, MainEL={sp?.MainEL}, TypeORB={sp?.TypeORB}");
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
				continue;
			}
			if (!enemy.IsAlive || enemy.IsJump || enemy.IsYS)
			{
				em.RemoveAt(i);
				i--;
				continue;
			}
			Vector3 b = (CP ? CP.transform.position : base.transform.position);
			if (Vector3.Distance(enemy.transform.position, b) > Range)
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
			Vector3 b2 = (CP ? CP.transform.position : base.transform.position);
			float num = Vector3.Distance(t1.transform.position, b2);
			float value = Vector3.Distance(t2.transform.position, b2);
			return num.CompareTo(value);
		});
	}

	public void Stop()
	{
		CanAT = false;
		ClearFire();
		ClearBuff();
		em.Clear();
		if (!CP && (bool)sp)
		{
			CP = sp.cp;
		}
		if ((bool)CP)
		{
			CP.RemoveForever(this);
		}
		LeanPool.Despawn(this);
	}
}
