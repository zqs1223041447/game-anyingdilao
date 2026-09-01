using System;
using System.Collections.Generic;
using FMODUnity;
using FinkFramework.Runtime.Singleton;
using Lean.Pool;
using UnityEngine;

public class SK_FSQ_Sword : MonoBehaviour
{
	public string SoundComp;

	public Skill_SD_List[] SoundFS;

	public Skill_PB_List[] OBJ;

	[HideInInspector]
	public List<SK_FlyS> sowrd = new List<SK_FlyS>();

	private List<Vector2> allPos = new List<Vector2>();

	[HideInInspector]
	public Transform point;

	[HideInInspector]
	public float high;

	[HideInInspector]
	public Transform target;

	[HideInInspector]
	public SkillOBJ_DT_SP sp;

	private float StartFaSheTime;

	private float timeA;

	private float timeB;

	private float timeC;

	private float range;

	private bool CanAT;

	private int CountTmp;

	[HideInInspector]
	public SK_BuffA mg;

	private Gun gun;

	private PlayerManager _playerManager;

	private bool initialized;

	private void Awake()
	{
		sp = GetComponent<SkillOBJ_DT_SP>();
		mg = GetComponent<SK_BuffA>();
		point = base.transform.Find("point");
		gun = SingletonMonoScope<Gun>.Instance;
		_playerManager = SingletonMonoScope<PlayerManager>.Instance;
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		timeA = 0f;
		timeB = 0f;
		timeC = 0f;
		CountTmp = 0;
		CanAT = false;
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
		if (sp.ZY)
		{
			switch (sp.TypeORB)
			{
			case 0:
				StartFaSheTime = (float)sp.Count_F * sp.FStime1;
				break;
			case 1:
				StartFaSheTime = (float)sp.Count_F * sp.FStime1 + 1f;
				break;
			}
		}
		else
		{
			StartFaSheTime = (float)sp.Count_F * sp.FStime1 + 1f;
		}
		switch (sp.TypeORB)
		{
		case 0:
			point.localPosition = new Vector3(0f, sp.High, 0f);
			if (sp.Count_F > 0 && sp.Count_F <= 4)
			{
				range = 0.8f;
			}
			else if (sp.Count_F > 4 && sp.Count_F <= 8)
			{
				range = 0.9f;
			}
			else if (sp.Count_F > 8 && sp.Count_F <= 12)
			{
				range = 1f;
			}
			else if (sp.Count_F > 10 && sp.Count_F <= 16)
			{
				range = 1.1f;
			}
			else if (sp.Count_F > 16 && sp.Count_F <= 20)
			{
				range = 1.2f;
			}
			else if (sp.Count_F > 20 && sp.Count_F <= 25)
			{
				range = 1.3f;
			}
			else if (sp.Count_F > 25 && sp.Count_F <= 30)
			{
				range = 1.4f;
			}
			else if (sp.Count_F > 30 && sp.Count_F <= 36)
			{
				range = 1.5f;
			}
			else
			{
				range = 1.6f;
			}
			break;
		case 1:
			point.localPosition = new Vector3(0f, 0f, 0f);
			if (sp.Count_F > 0 && sp.Count_F <= 4)
			{
				range = 1.2f;
			}
			else if (sp.Count_F > 4 && sp.Count_F <= 7)
			{
				range = 1.4f;
			}
			else if (sp.Count_F > 7 && sp.Count_F <= 10)
			{
				range = 1.6f;
			}
			else if (sp.Count_F > 10 && sp.Count_F <= 13)
			{
				range = 1.8f;
			}
			else if (sp.Count_F > 13 && sp.Count_F <= 16)
			{
				range = 2f;
			}
			else
			{
				range = 2.4f;
			}
			break;
		}
		Drop();
		CanAT = true;
	}

	private void Update()
	{
		if (!CanAT)
		{
			return;
		}
		if (sp.ZY)
		{
			if (_playerManager.IsAlive)
			{
				point.Rotate(new Vector3(0f, 0f, 1f), sp.AngleA * Time.deltaTime);
				if (CountTmp < sp.Count_F)
				{
					timeA += Time.deltaTime;
					if (timeA > sp.FStime1)
					{
						timeA = 0f;
						switch (sp.TypeORB)
						{
						case 0:
						{
							Vector3 vector3 = new Vector3(point.position.x + allPos[CountTmp].x, point.position.y + allPos[CountTmp].y, 0f);
							Vector3 vector4 = Gun.MousePos - vector3;
							float z2 = Mathf.Atan2(vector4.y, vector4.x) * 57.29578f;
							GameObject obj2 = LeanPool.Spawn(OBJ[sp.ZD_F].PB[sp.MainEL], vector3, Quaternion.Euler(0f, 0f, z2), point);
							Dicform component3 = obj2.GetComponent<Dicform>();
							component3.sp = sp;
							component3.SetCount(sp.ZY);
							component3.SubType = 0;
							component3.Index = 0;
							SK_FlyS component4 = obj2.GetComponent<SK_FlyS>();
							component4.MoveSpeed = UnityEngine.Random.Range(sp.Speed1, sp.Speed2);
							component4.ZY = sp.ZY;
							sowrd.Add(component4);
							if (SoundComp != null && UnityEngine.Random.Range(0, 101) < 70)
							{
								RuntimeManager.PlayOneShot(SoundComp, base.transform.position);
							}
							CountTmp++;
							break;
						}
						case 1:
						{
							float angle = (float)CountTmp / (float)sp.Count_F * 360f + point.eulerAngles.z;
							Vector3 vector = AnglePOS(point.position, range, angle);
							Vector3 vector2 = Gun.MousePos - vector;
							float z = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
							GameObject obj = LeanPool.Spawn(OBJ[sp.ZD_F].PB[sp.MainEL], vector, Quaternion.Euler(0f, 0f, z), point);
							Dicform component = obj.GetComponent<Dicform>();
							component.sp = sp;
							component.SetCount(sp.ZY);
							component.SubType = 0;
							component.Index = 0;
							SK_FlyS component2 = obj.GetComponent<SK_FlyS>();
							component2.MoveSpeed = UnityEngine.Random.Range(sp.Speed1, sp.Speed2);
							component2.ZY = sp.ZY;
							sowrd.Add(component2);
							if (SoundComp != null && UnityEngine.Random.Range(0, 101) < 70)
							{
								RuntimeManager.PlayOneShot(SoundComp, base.transform.position);
							}
							CountTmp++;
							break;
						}
						}
					}
				}
			}
			else
			{
				Stop();
			}
		}
		else if (sp.em.IsAlive)
		{
			point.Rotate(new Vector3(0f, 0f, 1f), sp.AngleA * Time.deltaTime);
			if (CountTmp < sp.Count_F)
			{
				timeA += Time.deltaTime;
				if (timeA > sp.FStime1)
				{
					timeA = 0f;
					switch (sp.TypeORB)
					{
					case 0:
					{
						Vector3 vector7 = new Vector3(point.position.x + allPos[CountTmp].x, point.position.y + allPos[CountTmp].y, 0f);
						Vector3 vector8 = ((!(sp.em.ATTarget != null)) ? (_playerManager.yao.transform.position - vector7) : (sp.em.ATTarget.transform.position - vector7));
						float z4 = Mathf.Atan2(vector8.y, vector8.x) * 57.29578f;
						GameObject obj4 = LeanPool.Spawn(OBJ[sp.ZD_F].PB[sp.MainEL], vector7, Quaternion.Euler(0f, 0f, z4), point);
						Dicform component7 = obj4.GetComponent<Dicform>();
						component7.sp = sp;
						component7.SetCount(sp.ZY);
						component7.SubType = 0;
						component7.Index = 0;
						SK_FlyS component8 = obj4.GetComponent<SK_FlyS>();
						component8.MoveSpeed = UnityEngine.Random.Range(sp.Speed1, sp.Speed2);
						component8.ZY = sp.ZY;
						sowrd.Add(component8);
						if (SoundComp != null && UnityEngine.Random.Range(0, 101) < 70)
						{
							RuntimeManager.PlayOneShot(SoundComp, base.transform.position);
						}
						CountTmp++;
						break;
					}
					case 1:
					{
						float angle2 = (float)CountTmp / (float)sp.Count_F * 360f + point.eulerAngles.z;
						Vector3 vector5 = AnglePOS(point.position, range, angle2);
						Vector3 vector6 = ((!sp.em.ATTarget) ? (_playerManager.yao.transform.position - vector5) : (sp.em.ATTarget.transform.position - vector5));
						float z3 = Mathf.Atan2(vector6.y, vector6.x) * 57.29578f;
						GameObject obj3 = LeanPool.Spawn(OBJ[sp.ZD_F].PB[sp.MainEL], vector5, Quaternion.Euler(0f, 0f, z3), point);
						Dicform component5 = obj3.GetComponent<Dicform>();
						component5.sp = sp;
						component5.SetCount(sp.ZY);
						component5.SubType = 0;
						component5.Index = 0;
						SK_FlyS component6 = obj3.GetComponent<SK_FlyS>();
						component6.MoveSpeed = UnityEngine.Random.Range(sp.Speed1, sp.Speed2);
						component6.ZY = sp.ZY;
						sowrd.Add(component6);
						if (SoundComp != null && UnityEngine.Random.Range(0, 101) < 70)
						{
							RuntimeManager.PlayOneShot(SoundComp, base.transform.position);
						}
						CountTmp++;
						break;
					}
					}
				}
			}
		}
		else
		{
			Stop();
		}
		if (sp.NoTime == 1)
		{
			timeB += Time.deltaTime;
			if (timeB > StartFaSheTime)
			{
				for (int i = 0; i < sowrd.Count; i++)
				{
					sowrd[i].FaShe();
				}
				if (SoundFS[sp.ZD_F].SD[sp.MainEL] != null)
				{
					RuntimeManager.PlayOneShot(SoundFS[sp.ZD_F].SD[sp.MainEL], base.transform.position);
				}
				sowrd.Clear();
				timeB = 0f;
				LeanPool.Despawn(this);
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
		if (timeC >= 0.15f)
		{
			if (sp.SpecialType == 10)
			{
				_playerManager.RefreshORB(sp, 0);
			}
			timeC = 0f;
		}
	}

	public void Stop()
	{
		CanAT = false;
		int num;
		for (num = 0; num < sowrd.Count; num++)
		{
			LeanPool.Despawn(sowrd[num]);
			sowrd.Remove(sowrd[num]);
			num--;
		}
		LeanPool.Despawn(this);
	}

	public void Drop()
	{
		allPos.Clear();
		int count_F = sp.Count_F;
		Vector2[] array = new Vector2[4]
		{
			new Vector2(range, range),
			new Vector2(range, 0f - range),
			new Vector2(0f - range, range),
			new Vector2(0f - range, 0f - range)
		};
		for (int i = 0; i < count_F; i++)
		{
			Vector2 vector = UnityEngine.Random.insideUnitCircle * range;
			float x = Mathf.Abs(vector.x);
			float y = Mathf.Abs(vector.y);
			int num = i % array.Length;
			Vector2 item = new Vector2(x, y) * array[num];
			allPos.Add(item);
		}
	}

	private static Vector3 AnglePOS(Vector3 center, float radius, float angle)
	{
		float f = angle * ((float)Math.PI / 180f);
		float x = center.x + radius * Mathf.Cos(f);
		float y = center.y + radius * Mathf.Sin(f);
		return new Vector3(x, y, 0f);
	}
}
