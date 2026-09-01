using System;
using FMODUnity;
using Lean.Pool;
using UnityEngine;

public class SK_FSQ_sonDIC : MonoBehaviour
{
	public string SoundA;

	public GameObject OBJ;

	public GameObject pen;

	public GameObject[] parOBJ;

	[Header("=========")]
	public float LifeTime;

	public float delDelay;

	public float FStime;

	public float FaSheDelayTime;

	public float range;

	public float angleRange;

	[Header("=========")]
	public bool CanChageFrom;

	[HideInInspector]
	public int FasheType;

	public bool UseFSQ_count;

	public int FasheTypeA;

	public int FasheTypeB;

	public int FSQ_countA;

	public int FSQ_countB;

	[Header("=========")]
	public float SpeedMin;

	public float SpeedMax;

	[Header("=========")]
	public GameObject SubA;

	public GameObject SubB;

	[HideInInspector]
	public Dicform dic;

	private int FScountTMP;

	private bool CanAT;

	private float timeA;

	private float timeB;

	private int FSnumber;

	private bool initialized;

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
		timeB = 0f;
		CanAT = false;
		FScountTMP = 0;
		if (parOBJ.Length != 0)
		{
			for (int i = 0; i < parOBJ.Length; i++)
			{
				parOBJ[i].SetActive(value: true);
			}
		}
		initialized = false;
	}

	private void Update()
	{
		if (CanAT && FScountTMP < FSnumber)
		{
			timeB += Time.deltaTime;
			if (timeB > FStime)
			{
				if (FasheType == 10)
				{
					Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
					component.sp = dic.sp;
					component.SetCount(dic.sp.ZY);
					component.SubType = dic.SubType;
					component.Index = dic.Index;
					component.dic = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
					component.speed = UnityEngine.Random.Range(SpeedMin, SpeedMax);
				}
				timeB = 0f;
				FScountTMP++;
			}
		}
		timeA += Time.deltaTime;
		if (timeA >= LifeTime)
		{
			timeA = 0f;
			Stop();
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
		if (CanChageFrom)
		{
			if (dic.sp.CF_Rate > 0f)
			{
				if ((float)UnityEngine.Random.Range(0, 101) < dic.sp.CF_Rate)
				{
					FasheType = FasheTypeB;
					FSnumber = FSQ_countB;
				}
				else
				{
					FasheType = FasheTypeA;
					FSnumber = FSQ_countA;
				}
			}
			else
			{
				FasheType = FasheTypeA;
				FSnumber = FSQ_countA;
			}
		}
		else
		{
			FasheType = FasheTypeA;
			if (UseFSQ_count)
			{
				FSnumber = FSQ_countA;
			}
			else
			{
				switch (dic.SubType)
				{
				case 0:
					FSnumber = dic.sp.Count_S;
					break;
				case 1:
					FSnumber = dic.sp.Count_AB;
					break;
				case 2:
					FSnumber = dic.sp.Count_AB;
					break;
				}
			}
		}
		if (SoundA != null)
		{
			RuntimeManager.PlayOneShot(SoundA, base.transform.position);
		}
		CanAT = true;
		this.wait(FaSheDelayTime, delegate
		{
			FaShe();
		});
	}

	public void FaShe()
	{
		switch (FasheType)
		{
		case 0:
		{
			for (int m = 0; m < FSQ_countA; m++)
			{
				float angle = (float)m / (float)FSQ_countA * 360f;
				Vector3 vector6 = AnglePOS(base.transform.position, range, angle);
				Dicform component8 = LeanPool.Spawn(OBJ, vector6, Quaternion.identity).GetComponent<Dicform>();
				component8.sp = dic.sp;
				component8.SetCount(dic.sp.ZY);
				component8.SubType = dic.SubType;
				component8.Index = dic.Index;
				component8.dic = vector6 - base.transform.position;
			}
			break;
		}
		case 1:
		{
			for (int num16 = 0; num16 < FSQ_countB; num16++)
			{
				float angle2 = (float)num16 / (float)FSQ_countB * 360f;
				Vector3 vector12 = AnglePOS(base.transform.position, range, angle2);
				Dicform component16 = LeanPool.Spawn(OBJ, vector12, Quaternion.identity).GetComponent<Dicform>();
				component16.sp = dic.sp;
				component16.SetCount(dic.sp.ZY);
				component16.SubType = dic.SubType;
				component16.Index = dic.Index;
				component16.dic = vector12 - base.transform.position;
			}
			break;
		}
		case 2:
		{
			Vector3 vector7 = dic.sp.TargetPos - base.transform.position;
			float num8 = Mathf.Atan2(vector7.y, vector7.x) * 57.29578f;
			if (FSQ_countA % 2 == 1)
			{
				Vector3 position2 = AnglePOS(base.transform.position, range, num8);
				Dicform component10 = LeanPool.Spawn(OBJ, position2, Quaternion.identity).GetComponent<Dicform>();
				component10.sp = dic.sp;
				component10.SetCount(dic.sp.ZY);
				component10.SubType = dic.SubType;
				component10.Index = dic.Index;
				component10.dic = vector7;
				if (FSQ_countA > 1)
				{
					for (int num9 = 0; num9 < FSQ_countA / 2; num9++)
					{
						float num10 = (float)(num9 + 1) * angleRange;
						Vector3 vector8 = AnglePOS(base.transform.position, range, num8 + num10);
						Dicform component11 = LeanPool.Spawn(OBJ, vector8, Quaternion.identity).GetComponent<Dicform>();
						component11.sp = dic.sp;
						component11.SetCount(dic.sp.ZY);
						component11.SubType = dic.SubType;
						component11.Index = dic.Index + 1;
						component11.dic = vector8 - base.transform.position;
						float num11 = 360f - (float)(num9 + 1) * angleRange;
						Vector3 vector9 = AnglePOS(base.transform.position, range, num8 + num11);
						Dicform component12 = LeanPool.Spawn(OBJ, vector9, Quaternion.identity).GetComponent<Dicform>();
						component12.sp = dic.sp;
						component12.SetCount(dic.sp.ZY);
						component12.SubType = dic.SubType;
						component12.Index = dic.Index + 1;
						component12.dic = vector9 - base.transform.position;
					}
				}
			}
			else
			{
				for (int num12 = 0; num12 < FSQ_countA / 2; num12++)
				{
					float num13 = (float)(num12 + 1) * angleRange - angleRange / 2f;
					Vector3 vector10 = AnglePOS(base.transform.position, range, num8 + num13);
					Dicform component13 = LeanPool.Spawn(OBJ, vector10, Quaternion.identity).GetComponent<Dicform>();
					component13.sp = dic.sp;
					component13.SetCount(dic.sp.ZY);
					component13.SubType = dic.SubType;
					component13.Index = dic.Index + 1;
					component13.dic = vector10 - base.transform.position;
					float num14 = 360f - (float)(num12 + 1) * angleRange + angleRange / 2f;
					Vector3 vector11 = AnglePOS(base.transform.position, range, num8 + num14);
					Dicform component14 = LeanPool.Spawn(OBJ, vector11, Quaternion.identity).GetComponent<Dicform>();
					component14.sp = dic.sp;
					component14.SetCount(dic.sp.ZY);
					component14.SubType = dic.SubType;
					component14.Index = dic.Index + 1;
					component14.dic = vector11 - base.transform.position;
				}
			}
			break;
		}
		case 3:
		{
			Vector3 vector = dic.sp.TargetPos - base.transform.position;
			float num2 = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			if (FSQ_countB % 2 == 1)
			{
				Vector3 position = AnglePOS(base.transform.position, range, num2);
				Dicform component2 = LeanPool.Spawn(OBJ, position, Quaternion.identity).GetComponent<Dicform>();
				component2.sp = dic.sp;
				component2.SetCount(dic.sp.ZY);
				component2.SubType = dic.SubType;
				component2.Index = dic.Index + 1;
				component2.dic = vector;
				if (FSQ_countB > 1)
				{
					for (int j = 0; j < FSQ_countB / 2; j++)
					{
						float num3 = (float)(j + 1) * angleRange;
						Vector3 vector2 = AnglePOS(base.transform.position, range, num2 + num3);
						Dicform component3 = LeanPool.Spawn(OBJ, vector2, Quaternion.identity).GetComponent<Dicform>();
						component3.sp = dic.sp;
						component3.SetCount(dic.sp.ZY);
						component3.SubType = dic.SubType;
						component2.Index = dic.Index + 1;
						component3.dic = vector2 - base.transform.position;
						float num4 = 360f - (float)(j + 1) * angleRange;
						Vector3 vector3 = AnglePOS(base.transform.position, range, num2 + num4);
						Dicform component4 = LeanPool.Spawn(OBJ, vector3, Quaternion.identity).GetComponent<Dicform>();
						component4.sp = dic.sp;
						component4.SetCount(dic.sp.ZY);
						component4.SubType = dic.SubType;
						component2.Index = dic.Index + 1;
						component4.dic = vector3 - base.transform.position;
					}
				}
			}
			else
			{
				for (int k = 0; k < FSQ_countB / 2; k++)
				{
					float num5 = (float)(k + 1) * angleRange - angleRange / 2f;
					Vector3 vector4 = AnglePOS(base.transform.position, range, num2 + num5);
					Dicform component5 = LeanPool.Spawn(OBJ, vector4, Quaternion.identity).GetComponent<Dicform>();
					component5.sp = dic.sp;
					component5.SetCount(dic.sp.ZY);
					component5.SubType = dic.SubType;
					component5.Index = dic.Index + 1;
					component5.dic = vector4 - base.transform.position;
					float num6 = 360f - (float)(k + 1) * angleRange + angleRange / 2f;
					Vector3 vector5 = AnglePOS(base.transform.position, range, num2 + num6);
					Dicform component6 = LeanPool.Spawn(OBJ, vector5, Quaternion.identity).GetComponent<Dicform>();
					component6.sp = dic.sp;
					component6.SetCount(dic.sp.ZY);
					component6.SubType = dic.SubType;
					component6.Index = dic.Index + 1;
					component6.dic = vector5 - base.transform.position;
				}
			}
			break;
		}
		case 4:
		{
			for (int num15 = 0; num15 < FSnumber; num15++)
			{
				Dicform component15 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component15.sp = dic.sp;
				component15.SetCount(dic.sp.ZY);
				component15.SubType = dic.SubType;
				component15.Index = dic.Index + 1;
				component15.dic = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
				component15.speed = UnityEngine.Random.Range(SpeedMin, SpeedMax);
			}
			break;
		}
		case 5:
		{
			for (int n = 0; n < FSQ_countA; n++)
			{
				Dicform component9 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component9.sp = dic.sp;
				component9.SetCount(dic.sp.ZY);
				component9.SubType = dic.SubType;
				component9.Index = dic.Index + 1;
				component9.dic = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
				component9.speed = UnityEngine.Random.Range(SpeedMin, SpeedMax);
			}
			break;
		}
		case 6:
		{
			for (int l = 0; l < FSnumber; l++)
			{
				Dicform component7 = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component7.sp = dic.sp;
				component7.SetCount(dic.sp.ZY);
				component7.SubType = dic.SubType;
				component7.Index = dic.Index + 1;
				component7.dic = dic.sp.TargetPos - base.transform.position;
				float num7 = Dis();
				component7.dic = new Vector2(component7.dic.x + UnityEngine.Random.Range(SpeedMin, 0f - SpeedMin), component7.dic.y + UnityEngine.Random.Range(SpeedMin, 0f - SpeedMin));
				if (num7 > 4f)
				{
					num7 = 4f;
				}
				component7.speed = (num7 + UnityEngine.Random.Range(SpeedMin, 0f - SpeedMin)) * 2f;
			}
			break;
		}
		case 7:
		{
			for (int i = 0; i < FSnumber; i++)
			{
				Dicform component = LeanPool.Spawn(OBJ, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
				component.sp = dic.sp;
				component.SetCount(dic.sp.ZY);
				component.SubType = dic.SubType;
				component.Index = dic.Index + 1;
				component.dic = dic.sp.TargetPos - base.transform.position;
				float num = Dis();
				component.dic = new Vector2(component.dic.x + UnityEngine.Random.Range(SpeedMax, 0f - SpeedMax), component.dic.y + UnityEngine.Random.Range(SpeedMax, 0f - SpeedMax));
				if (num > 4f)
				{
					num = 4f;
				}
				component.speed = (num + UnityEngine.Random.Range(SpeedMax, 0f - SpeedMax)) * 2f;
			}
			break;
		}
		}
		if (dic.sp.Layer_SubA == dic.Index && dic.SubType == 0 && dic.sp.DamageA > 0f && SubA != null)
		{
			Dicform component17 = LeanPool.Spawn(SubA, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component17.sp = dic.sp;
			component17.SetCount(dic.sp.ZY);
			component17.SubType = 1;
			component17.Index = dic.Index + 1;
		}
		if (dic.sp.Layer_SubB == dic.Index && dic.SubType == 0 && dic.sp.DamageB > 0f && SubB != null)
		{
			Dicform component18 = LeanPool.Spawn(SubB, base.transform.position, Quaternion.identity).GetComponent<Dicform>();
			component18.sp = dic.sp;
			component18.SetCount(dic.sp.ZY);
			component18.SubType = 2;
			component18.Index = dic.Index + 1;
		}
	}

	public void Stop()
	{
		CanAT = false;
		if (parOBJ.Length != 0)
		{
			for (int i = 0; i < parOBJ.Length; i++)
			{
				parOBJ[i].SetActive(value: false);
			}
		}
		this.wait(delDelay, delegate
		{
			LeanPool.Despawn(this);
		});
	}

	private static Vector3 AnglePOS(Vector3 center, float radius, float angle)
	{
		float f = angle * ((float)Math.PI / 180f);
		float x = center.x + radius * Mathf.Cos(f);
		float y = center.y + radius * Mathf.Sin(f);
		return new Vector3(x, y, 0f);
	}

	public float Dis()
	{
		float num = Vector2.Distance(dic.sp.TargetPos, base.transform.position);
		if (num < dic.sp.Distance)
		{
			return num;
		}
		return dic.sp.Distance;
	}
}
